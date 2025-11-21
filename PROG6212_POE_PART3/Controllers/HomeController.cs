using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_PART3.Data;
using PROG6212_POE_PART3.Models;


namespace PROG6212_POE_PART3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Home page action - Setting ViewData for the Home page
        [HttpGet]
        public IActionResult Index()
        {
            // Setting the title to "Welcome" to make sure Logout is hidden in the layout
            ViewData["Title"] = "Welcome";
            return View();
        }

        // Login page action - Get request
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";  // Setting Title for Login page
            return View();
        }

        // Login page action - Post request to handle user login
        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                ViewBag.Error = "Please fill in all fields and select a role.";
                return View();
            }

            // Look up user in the database
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password &&
                u.Role == role);

            if (user == null)
            {
                ViewBag.Error = "Invalid username, password, or role.";
                return View();
            }

            // Store session info
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");

            if (user.Role == "Lecturer")
            {
                HttpContext.Session.SetString("LecturerHourlyRate", user.HourlyRate.ToString());
            }

            // Redirect based on role
            return user.Role switch
            {
                "Lecturer" => RedirectToAction("MainDashboard", "Lecturer"),
                "Coordinator" => RedirectToAction("MainDashboard", "Coordinator"),
                "Manager" => RedirectToAction("ManagerDashboard", "Manager"),
                "HR" => RedirectToAction("Index", "HR"), // HR controller coming next
                _ => View()
            };
        }

        // Logout action to clear the session and redirect to Home page
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");  // Redirect back to Home/Index
        }
    }
}
