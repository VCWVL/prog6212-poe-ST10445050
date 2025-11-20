using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_PART3.Models;
using PROG6212_POE_PART3.Data;

namespace PROG6212_POE_PART3.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoordinatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Main Dashboard - Coordinator
        public IActionResult MainDashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Home");

            return View();
        }

        // Pending claims (status = Pending)
        public IActionResult PendingClaims()
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Home");

            var pending = _context.Claims
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();

            ViewData["Title"] = "Pending Claims";

            return View("Dashboard", pending);
        }

        // Claims already verified by coordinator
        public IActionResult VerifiedClaims()
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Home");

            // Fetch claims that are only "Approved" (final status after Manager approval)
            var verified = _context.Claims
                .Where(c => c.Status == "Approved") // Now filtering by "Approved" status
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();

            ViewData["Title"] = "Verified Claims";

            return View("ClaimsView", verified);
        }


        // Approve a claim
        [HttpGet]
        public IActionResult Approve(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Home");

            var claim = _context.Claims.FirstOrDefault(c => c.Id == id);

            if (claim != null)
            {
                claim.Status = "CoordinatorApproved";
                _context.SaveChanges();

                TempData["Message"] = $"✔ Claim ID {id} approved by Programme Coordinator.";
            }

            return RedirectToAction("PendingClaims");
        }

        // Reject a claim
        [HttpGet]
        public IActionResult Reject(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Coordinator")
                return RedirectToAction("Login", "Home");

            var claim = _context.Claims.FirstOrDefault(c => c.Id == id);

            if (claim != null)
            {
                claim.Status = "Rejected";
                _context.SaveChanges();

                TempData["Message"] = $"❌ Claim ID {id} rejected by Programme Coordinator.";
            }

            return RedirectToAction("PendingClaims");
        }
    }
}
