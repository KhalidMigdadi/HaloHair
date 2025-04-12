using Microsoft.AspNetCore.Mvc;

namespace HaloHair.Controllers
{
    public class MBookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
