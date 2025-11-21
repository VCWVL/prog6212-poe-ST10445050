using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_PART3.Data;
using PROG6212_POE_PART3.Models;
using System.IO;

namespace PROG6212_POE_PART2.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Paths for data storage (still used for encrypted files)
        private static readonly string DataFolder = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
        private static readonly string UploadFolder = Path.Combine(DataFolder, "Uploads");

        public LecturerController(ApplicationDbContext context)
        {
            _context = context;

            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
            if (!Directory.Exists(UploadFolder))
                Directory.CreateDirectory(UploadFolder);
        }

        // Dashboard view – show all claims for the logged-in lecturer
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            var claims = _context.Claims
                .Where(c => c.LecturerId == userId.Value)
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();

            return View(claims);
        }

        // Main dashboard (menu style)
        public IActionResult MainDashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        // View all claims for this lecturer
        public IActionResult ViewClaims()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            var claims = _context.Claims
                .Where(c => c.LecturerId == userId.Value)
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();

            return View(claims);
        }

        // POST: Handle claim submission
        [HttpPost]
        public IActionResult Create(Claim model, IFormFile supportingDocument)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null || user.Role != "Lecturer")
                return RedirectToAction("Login", "Home");

            // Ensure lecturer + rate are taken from DB, not from posted values
            model.LecturerId = user.Id;
            model.LecturerName = $"{user.FirstName} {user.LastName}";
            model.HourlyRate = user.HourlyRate;

            // Validate the model but ignore SupportingDocument field
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Automatic approval logic based on HoursWorked
                if (model.HoursWorked >= 40 && model.HoursWorked <= 180)
                {
                    // Automatically approve if hours are within the range
                    model.Status = "Approved";
                }
                else
                {
                    // Otherwise, it goes through the normal approval process
                    model.Status = "Pending";
                }

                // Set submission date and calculate the total claim amount
                model.DateSubmitted = DateTime.UtcNow;
                model.StoredClaimAmount = model.HoursWorked * model.HourlyRate;

                // Handle supporting document upload if it exists
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    string ext = Path.GetExtension(supportingDocument.FileName).ToLower();
                    string[] allowed = { ".pdf", ".docx", ".xlsx" };

                    if (!allowed.Contains(ext))
                    {
                        ModelState.AddModelError("", "Invalid file type. Allowed types: .pdf, .docx, .xlsx");
                        return View(model);
                    }

                    if (supportingDocument.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        ModelState.AddModelError("", "File size exceeds 5MB limit.");
                        return View(model);
                    }

                    // Save temp file
                    string tempFile = Path.Combine(UploadFolder, supportingDocument.FileName);
                    using (var stream = new FileStream(tempFile, FileMode.Create))
                        supportingDocument.CopyTo(stream);

                    // Encrypt file
                    string encryptedFileName = $"{Guid.NewGuid()}{ext}.enc";
                    string encryptedFilePath = Path.Combine(UploadFolder, encryptedFileName);
                    EncryptionHelper.EncryptFile(tempFile, encryptedFilePath);
                    System.IO.File.Delete(tempFile);

                    model.SupportingDocument = encryptedFileName;
                    model.OriginalFileName = supportingDocument.FileName;
                }

                // Save claim data to the database
                _context.Claims.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                return View(model);
            }
        }


        // Open supporting document inline (view in browser)
        public IActionResult OpenDocument(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.Id == id);
            if (claim == null || string.IsNullOrEmpty(claim.SupportingDocument))
                return NotFound();

            string encryptedPath = Path.Combine(UploadFolder, claim.SupportingDocument);
            if (!System.IO.File.Exists(encryptedPath))
                return NotFound();

            var ms = new MemoryStream();
            EncryptionHelper.DecryptFileToStream(encryptedPath, ms);
            ms.Position = 0;

            string contentType = claim.OriginalFileName.EndsWith(".pdf") ? "application/pdf" :
                                 claim.OriginalFileName.EndsWith(".docx") ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" :
                                 claim.OriginalFileName.EndsWith(".xlsx") ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" :
                                 "application/octet-stream";

            string safeFileName = SanitizeFileName(claim.OriginalFileName);
            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{safeFileName}\"");

            return File(ms, contentType);
        }

        // Download supporting document
        public IActionResult Download(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.Id == id);
            if (claim == null || string.IsNullOrEmpty(claim.SupportingDocument))
                return NotFound();

            string encryptedPath = Path.Combine(UploadFolder, claim.SupportingDocument);
            if (!System.IO.File.Exists(encryptedPath))
                return NotFound();

            var ms = new MemoryStream();
            EncryptionHelper.DecryptFileToStream(encryptedPath, ms);
            ms.Position = 0;

            string contentType = claim.OriginalFileName.EndsWith(".pdf") ? "application/pdf" :
                                 claim.OriginalFileName.EndsWith(".docx") ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" :
                                 claim.OriginalFileName.EndsWith(".xlsx") ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" :
                                 "application/octet-stream";

            string safeFileName = SanitizeFileName(claim.OriginalFileName);
            return File(ms, contentType, safeFileName);
        }

        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "file";

            var normalized = fileName.Normalize(System.Text.NormalizationForm.FormKD);
            var sb = new System.Text.StringBuilder();
            foreach (char c in normalized)
            {
                if (c <= 127) sb.Append(c);
            }

            return sb.Length > 0 ? sb.ToString() : "file";
        }
    }
}
