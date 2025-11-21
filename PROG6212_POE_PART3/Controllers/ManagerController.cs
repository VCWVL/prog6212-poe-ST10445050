using Microsoft.AspNetCore.Mvc;

using PROG6212_POE_PART3.Models;
using PROG6212_POE_PART3.Data;

namespace PROG6212_POE_PART3.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Main Dashboard
        public IActionResult ManagerDashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            return View();
        }

        // Pending claims – those approved by coordinator
        public IActionResult PendingClaims()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            var pending = _context.Claims
                .Where(c => c.Status == "CoordinatorApproved")
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();

            ViewBag.ReadOnly = false;
            return View("Dashboard", pending);
        }

        // Verified (final approved) claims
        public IActionResult VerifiedClaims()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            // ONLY APPROVED — not rejected
            var verified = _context.Claims
                .Where(c => c.Status == "Approved")
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();

            ViewBag.ReadOnly = true;
            return View("VerifiedClaims", verified);
        }


        [HttpGet]
        public IActionResult Approve(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            var claim = _context.Claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = "Approved";
                _context.SaveChanges();
                TempData["Message"] = $"✅ Claim ID {id} approved by Academic Manager.";
            }
            return RedirectToAction("PendingClaims");
        }

        [HttpGet]
        public IActionResult Reject(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            var claim = _context.Claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
                _context.SaveChanges();
                TempData["Message"] = $"❌ Claim ID {id} rejected by Academic Manager.";
            }
            return RedirectToAction("PendingClaims");
        }

        // Details view
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
                return RedirectToAction("Login", "Home");

            var claim = _context.Claims.FirstOrDefault(c => c.Id == id);
            if (claim == null)
                return NotFound();

            return View(claim);
        }
    }
}
