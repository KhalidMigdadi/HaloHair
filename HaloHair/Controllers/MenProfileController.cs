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
        //public async Task<IActionResult> Profile()
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (!userId.HasValue)
        //        return RedirectToAction("Login", "Account");

        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        //    if (user == null)
        //        return NotFound();

        //    return View(user);

        //}


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
                .Where(b => b.UserId == userId && b.BookingDate < DateOnly.FromDateTime(DateTime.Today))
                .ToList();

            var model = new ProfileViewModel
            {
                User = user,
                BookingHistory = history
            };


            // الحصول على الصالونات المفضلة للمستخدم
            var favoriteSalons = _context.Favorites
                .Where(f => f.UserId == userId.Value)
                .Include(f => f.Salon) // تضمين بيانات الصالون
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










        //[HttpGet]
        //public async Task<IActionResult> BookingHistory()
        //{
        //    // الحصول على معرف المستخدم الحالي من الجلسة (بنفس طريقة حفظ الحجوزات)
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (!userId.HasValue)
        //        return RedirectToAction("Login", "Account");

        //    // استرجاع سجلات الحجوزات مع الخدمات المرتبطة بها
        //    var bookingHistories = await _context.BookingsHistories
        //        .Where(b => b.UserId == userId.Value)
        //        .OrderByDescending(b => b.BookingDate)
        //        .ThenByDescending(b => b.StartTime)
        //        .ToListAsync();

        //    var viewModels = new List<BookingHistoryViewModel>();

        //    foreach (var history in bookingHistories)
        //    {
        //        // البحث عن الحلاق وصالونه
        //        var barber = await _context.Barbers
        //            .Include(b => b.Salon)
        //            .FirstOrDefaultAsync(b => b.Id == history.BarberId);

        //        // البحث عن الموعد المرتبط بهذا السجل
        //        // سنفترض أن هناك موعد مرتبط بنفس التاريخ والوقت للمستخدم والحلاق
        //        var appointment = await _context.Appointments
        //            .FirstOrDefaultAsync(a =>
        //                a.UserId == userId.Value &&
        //                a.BarberId == history.BarberId &&
        //                a.AppointmentDate.Equals(history.BookingDate.ToDateTime(TimeOnly.MinValue)) &&
        //                a.StartTime.TimeOfDay.Equals(history.StartTime.ToTimeSpan()));

        //        List<BookingServiceViewModel> services = new List<BookingServiceViewModel>();

        //        // إذا وجدنا الموعد، نستعرض الخدمات المرتبطة به
        //        if (appointment != null)
        //        {
        //            var appointmentServices = await _context.AppointmentServices
        //                .Where(s => s.AppointmentId == appointment.Id)
        //                .ToListAsync();

        //            services = appointmentServices.Select(s => new BookingServiceViewModel
        //            {
        //                ServiceId = s.ServiceId,
        //                ServiceName = s.ServiceName,
        //                Duration = s.Duration,
        //                Price = s.Price
        //            }).ToList();
        //        }

        //        viewModels.Add(new BookingHistoryViewModel
        //        {
        //            BookingHistoryId = history.Id,
        //            UserId = history.UserId,
        //            BarberId = history.BarberId,
        //            BarberName = barber?.FirstName ?? "Unknown Barber",
        //            SalonName = barber?.Salon?.Name ?? "Unknown Salon",
        //            BookingDate = history.BookingDate,
        //            StartTime = history.StartTime,
        //            EndTime = history.EndTime,
        //            TotalDuration = history.TotalDuration,
        //            TotalPrice = history.TotalPrice,
        //            Status = history.Status,
        //            CreatedAt = history.CreatedAt,
        //            Services = services
        //        });
        //    }

        //    return View(viewModels);
        //}



    }
}
