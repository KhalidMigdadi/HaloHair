using HaloHair.Models;
using Microsoft.AspNetCore.Mvc;

namespace HaloHair.Controllers
{
    public class SuperAdminController : Controller
    {

        private readonly MyDbContext _context;

        public SuperAdminController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            var email = HttpContext.Session.GetString("Email");

            if (email != "admin@gmail.com")
            {
                return RedirectToAction("LoginBarberMen", "Barber");
            }

            int totalUsers = _context.Users.Count();
            ViewBag.TotalUsers = totalUsers;


            int totalBarbers = _context.Barbers.Count();
            ViewBag.TotalBarbers = totalBarbers;

            int totalSalons = _context.Salons.Count();
            ViewBag.TotalSalons = totalSalons;

            return View();
        }

        public IActionResult ViewAllUsers()
        {
            var users = _context.Users.ToList();
            return View(users);
        }



        public IActionResult BlockUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.IsBlocked = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ViewAllUsers");
        }

        public IActionResult UnblockUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.IsBlocked = false;
                _context.SaveChanges();
            }
            return RedirectToAction("ViewAllUsers");
        }




        public IActionResult ViewAllBarbers()
        {
            var barbers = _context.Barbers.ToList();
            return View(barbers);
        }



        public IActionResult BlockUserB(int barberId)
        {
            var barber = _context.Barbers.FirstOrDefault(u => u.Id == barberId);
            if (barber != null)
            {
                barber.IsBlocked = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ViewAllBarbers");
        }

        public IActionResult UnblockUserB(int barberId)
        {
            var barber = _context.Barbers.FirstOrDefault(u => u.Id == barberId);
            if (barber != null)
            {
                barber.IsBlocked = false;
                _context.SaveChanges();
            }
            return RedirectToAction("ViewAllBarbers");
        }



        public IActionResult ViewAllSalons()
        {
            var salons = _context.Salons.ToList();
            return View(salons);
        }


        public IActionResult MakeSalonInvisible(int salonId)
        {
            var salon = _context.Salons.FirstOrDefault(s => s.Id == salonId);
            if (salon != null)
            {
                salon.IsVisible = false;
                _context.SaveChanges();
            }
            return RedirectToAction("ViewAllSalons");
        }

        public IActionResult MakeSalonVisible(int salonId)
        {
            var salon = _context.Salons.FirstOrDefault(s => s.Id == salonId);
            if (salon != null)
            {
                salon.IsVisible = true; // تعيينه إلى true لجعل الصالون مرئي
                _context.SaveChanges();
            }
            return RedirectToAction("ViewAllSalons");
        }




        public IActionResult ContactMassages()
        {
            return View(_context.Contacts.ToList());
        }



    }
}
