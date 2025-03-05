using Microsoft.AspNetCore.Mvc;

namespace HaloHair.Controllers
{
    public class BarberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult LoginBarberMen()
        {
            return View();
        }



        public IActionResult RegisterBarber()
        {
            return View();
        }


        public IActionResult EnterPassword()
        {
            return View();
        }



        public IActionResult ResetPassword()
        {
            return View();
        }


    }
}
