using HaloHair.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class MenController : Controller
    {

        private readonly MyDbContext _context;

        public MenController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Men()
        {
            // First, get all salons with their barbers
            var salons = _context.Salons
                .Include(s => s.Barbers)
                .Where(s => s.IsVisible == true)
                .ToList();

            // Create a list to store our results
            var recommendedSalons = new List<SalonViewModel>();

            // Calculate ratings for each salon
            foreach (var salon in salons)
            {
                // Get all barber IDs for this salon
                var barberIds = salon.Barbers.Select(b => b.Id).ToList();

                // Get all booking history IDs associated with these barbers
                var bookingHistoryIds = _context.BookingsHistories
                    .Where(bh => barberIds.Contains(bh.BarberId))
                    .Select(bh => bh.Id)
                    .ToList();

                // Get all reviews for these booking histories
                var reviews = _context.Reviews
                    .Where(r => bookingHistoryIds.Contains(r.BookingHistoryId))
                    .ToList();

                // Calculate average rating
                double averageRating = 0;
                if (reviews.Any())
                {
                    averageRating = reviews.Average(r => r.Rating ?? 0);
                }

                // Add to our view model list
                recommendedSalons.Add(new SalonViewModel
                {
                    Salon = salon,
                    AverageRating = averageRating,
                    ReviewCount = reviews.Count
                });
            }

            // Sort by rating (highest first) and take top 5
            var topSalons = recommendedSalons
                .OrderByDescending(s => s.AverageRating)
                .Take(5)
                .ToList();

            ViewBag.RecommendedSalons = topSalons;

            var newSalons = _context.Salons
                     .Where(s => s.IsVisible == true)
                     .OrderByDescending(s => s.CreatedAt)
                     .Take(10)
                     .ToList();

            ViewBag.NewSalons = newSalons;



            return View(salons);
        }



        // صفحة التسجيل للمستخدم العادي
        public IActionResult RegisterUserMen()
        {
            return View();
        }

        // عند إرسال نموذج التسجيل للمستخدم العادي
        [HttpPost]
        public IActionResult RegisterUserMen(User user, string password, string confirmPassword, string gender)
        {
            // تحقق من أن النموذج صالح
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "يرجى ملء جميع الحقول بشكل صحيح!";
                return RedirectToAction("RegisterUserMen");
            }

            // تعيين الجنس للمستخدم
            user.Gender = gender;

            // تحقق من أن البريد الإلكتروني غير مسجل
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                TempData["Error"] = "هذا البريد الإلكتروني مسجل بالفعل!";
                return RedirectToAction("RegisterUserMen");
            }

            // تحقق من تطابق كلمة المرور
            if (password != confirmPassword)
            {
                TempData["Error"] = "كلمة المرور غير متطابقة!";
                return RedirectToAction("RegisterUserMen");
            }

            // تشفير كلمة المرور قبل الحفظ
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, password); // Hash the password

            // تعيين نوع المستخدم كـ "مستخدم عادي"
            user.UserType = "RegularUser"; // تحديد نوع المستخدم

            try
            {
                // إضافة المستخدم إلى قاعدة البيانات
                _context.Users.Add(user);
                _context.SaveChanges();

                // حفظ معلومات المستخدم في الجلسة لاستخدامها لاحقًا
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);
                HttpContext.Session.SetString("Gender", user.Gender);
                HttpContext.Session.SetString("Phone", user.PhoneNumber);

                // إعادة التوجيه إلى صفحة تسجيل الدخول
                return RedirectToAction("LoginUserMen");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "حدث خطأ أثناء التسجيل: " + ex.Message;
                return RedirectToAction("RegisterUserMen");
            }
        }



        public IActionResult LoginUserMen()
        {
            return View();
        }



        [HttpPost]
        public IActionResult LoginUserMen(string email)
        {
            // البحث عن المستخدم باستخدام البريد الإلكتروني
            var loggedUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (loggedUser != null)
            {
                // التحقق إذا كان المستخدم محظور
                if (loggedUser.IsBlocked)
                {
                    TempData["BlockedError"] = "Your account has been banned. Please contact support.";
                    return RedirectToAction("LoginUserMen");
                }

                // حفظ معلومات المستخدم في الجلسة
                HttpContext.Session.SetString("Email", loggedUser.Email);
                HttpContext.Session.SetString("FirstName", loggedUser.FirstName);
                HttpContext.Session.SetString("LastName", loggedUser.LastName);
                HttpContext.Session.SetString("Phone", loggedUser.PhoneNumber);
                HttpContext.Session.SetInt32("UserId", loggedUser.Id);  // حفظ UserId في الجلسة

                // إعادة التوجيه إلى الصفحة لتأكيد كلمة المرور (إذا كنت بحاجة لذلك)
                return RedirectToAction("EnterPassword", "Men");
            }
            else
            {
                // إذا كان البريد الإلكتروني غير موجود
                TempData["EmailError"] = "البريد الإلكتروني غير موجود. يرجى المحاولة مرة أخرى.";
                return RedirectToAction("LoginUserMen");
            }
        }



        //public IActionResult EnterPassword()
        //{
        //    var email = HttpContext.Session.GetString("Email");  // Retrieve the email from Session

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return RedirectToAction("LoginUserMen", "Men");
        //    }

        //    ViewBag.Email = email;  // Pass the email to the view using ViewBag

        //    return View();
        //}


        public IActionResult EnterPassword()
        {
            var email = HttpContext.Session.GetString("Email");  // Retrieve the email from Session
            var userId = HttpContext.Session.GetInt32("UserId");  // Retrieve the UserId from Session

            if (string.IsNullOrEmpty(email) || userId == null)
            {
                return RedirectToAction("LoginUserMen", "Men");
            }

            ViewBag.Email = email;  // Pass the email to the view using ViewBag
            ViewBag.UserId = userId;  // Pass the UserId to the view using ViewBag if needed

            return View();
        }



        //[HttpPost]
        //public IActionResult EnterPassword(string password)
        //{
        //    var email = HttpContext.Session.GetString("Email");

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return RedirectToAction("LoginUserMen", "Men");
        //    }

        //    // Fetch the user from the database using the email stored in the session
        //    var loggedUser = _context.Users.FirstOrDefault(u => u.Email == email);

        //    if (loggedUser != null)
        //    {
        //        // Verify the plain-text password against the hashed password
        //        var passwordHasher = new PasswordHasher<User>();
        //        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(loggedUser, loggedUser.PasswordHash, password);

        //        if (passwordVerificationResult == PasswordVerificationResult.Success)
        //        {
        //            // Password is correct, redirect to the desired page
        //            return RedirectToAction("Men", "Men");  // Redirect to a user dashboard page or appropriate location
        //        }
        //        else
        //        {
        //            TempData["PasswordError"] = "Invalid password. Please try again.";
        //            return RedirectToAction("EnterPassword", "Men");
        //        }
        //    }

        //    // If user is not found or any error, redirect to the login page
        //    return RedirectToAction("LoginUserMen", "Men");
        //}


        [HttpPost]
        public IActionResult EnterPassword(string password)
        {
            var email = HttpContext.Session.GetString("Email");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(email) || userId == null)
            {
                return RedirectToAction("LoginUserMen", "Men");
            }

            // Fetch the user from the database using the email stored in the session
            var loggedUser = _context.Users.FirstOrDefault(u => u.Email == email && u.Id == userId);

            if (loggedUser != null)
            {
                // Verify the plain-text password against the hashed password
                var passwordHasher = new PasswordHasher<User>();
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(loggedUser, loggedUser.PasswordHash, password);

                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    // Password is correct, redirect to the desired page
                    return RedirectToAction("Men", "Men");  // Redirect to a user dashboard page or appropriate location
                }
                else
                {
                    TempData["PasswordError"] = "Invalid password. Please try again.";
                    return RedirectToAction("EnterPassword", "Men");
                }
            }

            // If user is not found or any error, redirect to the login page
            return RedirectToAction("LoginUserMen", "Men");
        }



    }
}
