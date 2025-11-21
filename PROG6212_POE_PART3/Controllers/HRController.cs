using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE_PART3.Data;
using PROG6212_POE_PART3.Models;
using System.IO;
using System.Text;

namespace PROG6212_POE_PART3.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsHR()
        {
            return HttpContext.Session.GetString("Role") == "HR";
        }

        public IActionResult Index()
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var totalUsers = _context.Users.Count();
            var totalLecturers = _context.Users.Count(u => u.Role == "Lecturer");
            var totalClaims = _context.Claims.Count();
            var pendingClaims = _context.Claims.Count(c => c.Status == "Pending");
            var approvedClaims = _context.Claims.Count(c => c.Status == "Approved");

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalLecturers = totalLecturers;
            ViewBag.TotalClaims = totalClaims;
            ViewBag.PendingClaims = pendingClaims;
            ViewBag.ApprovedClaims = approvedClaims;

            return View();
        }

        public IActionResult ManageUsers()
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var users = _context.Users
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FirstName)
                .ToList();

            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var user = new User();
            return View(user);
        }

        [HttpPost]
        public IActionResult CreateUser(User model)
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
                return View(model);

            _context.Users.Add(model);
            _context.SaveChanges();

            TempData["Message"] = "User created successfully.";
            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User model)
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
                return NotFound();

            user.Username = model.Username;
            user.Password = model.Password;
            user.Role = model.Role;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.HourlyRate = model.HourlyRate;

            _context.SaveChanges();

            TempData["Message"] = "User updated successfully.";
            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        public IActionResult DeleteUserConfirmed(int id)
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["Message"] = "User deleted successfully.";
            return RedirectToAction("ManageUsers");
        }

        // ========== NEW PER-CLAIM PDF REPORT ==========

        public IActionResult DownloadClaimReport(int id)
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var claim = _context.Claims
                .Include(c => c.Lecturer)
                .FirstOrDefault(c => c.Id == id);

            if (claim == null)
                return NotFound();

            if (claim.Status != "Approved")
                return BadRequest("Reports can only be generated for approved claims.");

            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40f, 40f, 40f, 40f);
                PdfWriter.GetInstance(doc, ms);

                doc.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);

                doc.Add(new Paragraph("Contract Monthly Claim Report", titleFont));
                doc.Add(new Paragraph($"Generated: {DateTime.Now}", normalFont));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph($"Claim ID: {claim.Id}", boldFont));
                doc.Add(new Paragraph(" "));

                // Table with details
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 10f;
                table.SetWidths(new float[] { 30f, 70f });

                void AddRow(string label, string value)
                {
                    var cellLabel = new PdfPCell(new Phrase(label, boldFont));
                    cellLabel.Border = Rectangle.NO_BORDER;
                    cellLabel.Padding = 4f;

                    var cellValue = new PdfPCell(new Phrase(value, normalFont));
                    cellValue.Border = Rectangle.NO_BORDER;
                    cellValue.Padding = 4f;

                    table.AddCell(cellLabel);
                    table.AddCell(cellValue);
                }

                var lecturerEmail = claim.Lecturer?.Email ?? "N/A";

                AddRow("Lecturer Name:", claim.LecturerName);
                AddRow("Lecturer Email:", lecturerEmail);
                AddRow("Lecturer ID:", claim.LecturerId.ToString());
                AddRow("Hours Worked:", claim.HoursWorked.ToString());
                AddRow("Hourly Rate (R):", claim.HourlyRate.ToString("F2"));
                AddRow("Total Claim Amount (R):", claim.StoredClaimAmount.ToString("F2"));
                AddRow("Status:", claim.Status);
                AddRow("Date Submitted:", claim.DateSubmitted.ToString("yyyy-MM-dd HH:mm"));
                AddRow("Notes:", string.IsNullOrWhiteSpace(claim.Notes) ? "None" : claim.Notes);

                var supportingInfo = string.IsNullOrWhiteSpace(claim.OriginalFileName)
                    ? "No supporting document uploaded."
                    : $"{claim.OriginalFileName} (encrypted on server)";
                AddRow("Supporting Document:", supportingInfo);

                doc.Add(table);

                doc.Add(new Paragraph("This report summarises the approved claim for the specified lecturer.", normalFont));

                doc.Close();

                var fileBytes = ms.ToArray();
                var fileName = $"Claim_{claim.Id}_Report.pdf";
                return File(fileBytes, "application/pdf", fileName);
            }
        }

        // ========== SIMPLE LIST VIEWS FOR CARDS ==========

        public IActionResult AllUsers()
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult LecturerList()
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var lecturers = _context.Users
                .Where(x => x.Role == "Lecturer")
                .ToList();

            return View(lecturers);
        }

        public IActionResult AllClaims()
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var claims = _context.Claims.ToList();
            return View(claims);
        }

        public IActionResult PendingClaims()
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            // Fetch claims where status is "Pending" or "Pending (Awaiting Manager)"
            var claims = _context.Claims
                .Where(x => x.Status == "Pending" || x.Status == "Pending (Awaiting Manager)")
                .ToList();

            return View(claims);
        }



        public IActionResult ApprovedClaims()
        {
            if (!IsHR())
                return RedirectToAction("Login", "Home");

            var claims = _context.Claims
                .Where(x => x.Status == "Approved")
                .ToList();

            return View(claims);
        }
    }
}
