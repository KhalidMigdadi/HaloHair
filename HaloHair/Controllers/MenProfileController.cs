using System.Security.Claims;
using HaloHair.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class MenProfileController : Controller
    {

        private readonly MyDbContext _context;

        public MenProfileController(MyDbContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            var history = _context.BookingsHistories
                .Include(b => b.Barber)
                .ThenInclude(barber => barber.Salon)
                .Where(b => b.UserId == userId && b.BookingDate.Date <= DateTime.Today.AddDays(1))
                .ToList();

            var model = new ProfileViewModel
            {
                User = user,
                BookingHistory = history
            };

            // الصالونات المفضلة
            var favoriteSalons = _context.Favorites
                .Where(f => f.UserId == userId.Value)
                .Include(f => f.Salon)
                .Select(f => f.Salon)
                .ToList();

            ViewBag.FavoriteSalons = favoriteSalons;

            return View(model);
        }





        [HttpPost]
        public async Task<IActionResult> EditProfile(User model, string? NewPassword, string? ConfirmPassword, IFormFile? ProfileImage)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var user = await _context.Users.FindAsync(model.Id);
            if (user == null)
                return NotFound();

            // تحديث البيانات الأساسية
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            // معالجة الصورة الجديدة إن وُجدت
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var fileName = Path.GetFileName(ProfileImage.FileName);
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/ProfilePictures");

                // تأكد أن المجلد موجود
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                var filePath = Path.Combine(imageFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                // تحديث مسار الصورة في قاعدة البيانات
                user.ProfileImagePath = $"Images/ProfilePictures/{fileName}";
            }

            // تغيير كلمة السر في حال تم إدخالها
            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (NewPassword != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return View("Profile", model);
                }

                user.PasswordHash = NewPassword; // تأكد من استخدام تشفير إذا كنت تستخدم هوية ASP.NET Identity
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }









        [HttpPost]
        public async Task<IActionResult> AddReview(int bookingId, int rating, string comment)
        {
            try
            {
                var bookingHistory = await _context.BookingsHistories
                    .Include(b => b.Barber)         // Include the Barber
                    .ThenInclude(b => b.Salon)      // Then include the related Salon
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (bookingHistory == null || bookingHistory.Status != "Completed")
                {
                    return Json(new { success = false, message = "لا يمكنك إضافة التقييم إلا بعد إتمام الخدمة." });
                }

                var existingReview = await _context.Reviews
                    .Where(r => r.BookingHistoryId == bookingId)
                    .FirstOrDefaultAsync();

                if (existingReview != null)
                {
                    return Json(new { success = false, message = "لقد قمت بتقييم هذه الخدمة مسبقاً." });
                }

                var review = new Review
                {
                    BookingHistoryId = bookingId,
                    Rating = rating,
                    Comment = comment,
                    UserId = bookingHistory.UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    SalonId = bookingHistory.Barber?.SalonId // ⬅️ إضافة الـ SalonId
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "تم إرسال التقييم بنجاح!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddReview: {ex.Message}");
                return Json(new { success = false, message = $"حدث خطأ: {ex.Message}" });
            }
        }










    }
}
