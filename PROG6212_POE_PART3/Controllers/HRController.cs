using Microsoft.AspNetCore.Mvc;

namespace PROG6212_POE_PART3.Controllers
{
    public class HRController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
