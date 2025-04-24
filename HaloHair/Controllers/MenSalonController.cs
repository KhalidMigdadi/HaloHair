using System.Security.Claims;
using HaloHair.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{

    public class MenSalonController : Controller
    {
        private readonly MyDbContext _context;

        public MenSalonController(MyDbContext context)
        {
            _context = context;
        }


        public IActionResult SalonMen(int id)
        {
            var salon = _context.Salons
                .Include(s => s.Reviews)
                    .ThenInclude(r => r.User)
                .Include(s => s.SalonImages)
                .Include(s => s.Services)
                .Include(s => s.Barbers)
                    .ThenInclude(b => b.BarberWorkImages)
                .Include(s => s.WorkingHours)
                .FirstOrDefault(s => s.Id == id);

            if (salon == null)
            {
                return NotFound();
            }


            // التحقق مما إذا كان الصالون مفضلاً للمستخدم الحالي
            bool isFavorite = false;
            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                isFavorite = _context.Favorites.Any(f => f.UserId == userId && f.SalonId == id);
            }

            ViewBag.IsFavorite = isFavorite;



            return View(salon);
        }





        [HttpPost]
        public IActionResult ToggleFavorite(int salonId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "لم يتم العثور على معرف المستخدم في الجلسة" });
            }

            var existingFavorite = _context.Favorites
                .FirstOrDefault(f => f.UserId == userId.Value && f.SalonId == salonId);

            bool isFavorite;
            if (existingFavorite != null)
            {
                _context.Favorites.Remove(existingFavorite);
                isFavorite = false;
            }
            else
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId.Value,
                    SalonId = salonId,
                    CreatedAt = DateTime.Now
                });
                isFavorite = true;
            }

            _context.SaveChanges();
            return Json(new { success = true, isFavorite = isFavorite });
        }



        public IActionResult SelectServiceMen()
        {
            return View();
        }


        public IActionResult SelectBarberMen()
        {
            return View();
        }

        public IActionResult SelectTimeMen()
        {
            return View();
        }

        public IActionResult ConfirmBookingMen()
        {
            return View();
        }

    }
}
