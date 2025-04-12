using Microsoft.AspNetCore.Mvc;

namespace HaloHair.Controllers
{
    public class BarberSalesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
