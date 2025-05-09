using System.Net;
using HaloHair.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;





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
                .Take(10)
                .ToList();

            ViewBag.RecommendedSalons = topSalons;

            var newSalons = _context.Salons
                     .Where(s => s.IsVisible == true)
                     .OrderByDescending(s => s.CreatedAt)
                     .Take(10)
                     .ToList();

            ViewBag.NewSalons = newSalons;



            // ✅ جديد: جلب آخر الوظائف المنشورة
            var latestVacancies = _context.Vacancies
                .Include(v => v.Salon)
                .OrderByDescending(v => v.CreatedAt)
                .Take(6)
                .ToList();

            ViewBag.LatestVacancies = latestVacancies;


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

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, password); // Hash the password

            user.UserType = "RegularUser"; 

            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);
                HttpContext.Session.SetString("Gender", user.Gender);
                HttpContext.Session.SetString("Phone", user.PhoneNumber);

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
            var loggedUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (loggedUser != null)
            {
                if (loggedUser.IsBlocked)
                {
                    TempData["BlockedErrorLogin"] = "Your account has been banned. Please contact support.";
                    return RedirectToAction("LoginUserMen");
                }

                HttpContext.Session.SetString("Email", loggedUser.Email);
                HttpContext.Session.SetString("FirstName", loggedUser.FirstName);
                HttpContext.Session.SetString("LastName", loggedUser.LastName);
                HttpContext.Session.SetString("Phone", loggedUser.PhoneNumber);
                HttpContext.Session.SetInt32("UserId", loggedUser.Id);  // حفظ UserId في الجلسة
                HttpContext.Session.SetString("ProfileImagePath", loggedUser.ProfileImagePath ?? "Images/default_profile.png");


                return RedirectToAction("EnterPassword", "Men");
            }
            else
            {
                TempData["EmailErrorLogin"] = "Email address not found. Please try again.";
                return RedirectToAction("LoginUserMen");
            }
        }



        public IActionResult EnterPassword()
        {
            var email = HttpContext.Session.GetString("Email");  
            var userId = HttpContext.Session.GetInt32("UserId");  

            if (string.IsNullOrEmpty(email) || userId == null)
            {
                return RedirectToAction("LoginUserMen", "Men");
            }

            ViewBag.Email = email;  
            ViewBag.UserId = userId;  

            return View();
        }



 

        [HttpPost]
        public IActionResult EnterPassword(string password)
        {
            var email = HttpContext.Session.GetString("Email");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(email) || userId == null)
            {
                return RedirectToAction("LoginUserMen", "Men");
            }

            var loggedUser = _context.Users.FirstOrDefault(u => u.Email == email && u.Id == userId);

            if(loggedUser != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var passwordHasherResult = passwordHasher.VerifyHashedPassword(loggedUser, loggedUser.PasswordHash, password);

                if(passwordHasherResult == PasswordVerificationResult.Success)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["PasswordErrorLogin"] = "Invalid password. Please try again.";
                    return RedirectToAction("EnterPassword", "Men");
                }
            }

            return RedirectToAction("LoginUserMen", "Men");
        }







        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HandleForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                TempData["ResetError"] = "البريد الإلكتروني غير موجود!";
                return RedirectToAction("ForgotPassword");
            }

            // إنشاء رمز تحقق عشوائي
            Random rand = new Random();
            int verificationCode = rand.Next(100000, 999999);

            // تخزين الرمز والبريد الإلكتروني في الجلسة
            HttpContext.Session.SetString("ResetCode", verificationCode.ToString());
            HttpContext.Session.SetString("ResetEmail", email);

            // إرسال رمز التحقق عبر البريد الإلكتروني
            await SendEmailAsync(email, "رمز إعادة تعيين كلمة المرور", $"رمز إعادة التعيين الخاص بك هو: {verificationCode}");

            TempData["VerificationMessage"] = "A verification code has been sent to your email!";

            return RedirectToAction("VerifyResetCode");
        }

        public IActionResult VerifyResetCode()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HandleVerifyCode(string code)
        {
            var storedCode = HttpContext.Session.GetString("ResetCode");
            var email = HttpContext.Session.GetString("ResetEmail");

            if (string.IsNullOrEmpty(storedCode) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            if (storedCode != code)
            {
                TempData["CodeError"] = "رمز التحقق غير صحيح!";
                return RedirectToAction("VerifyResetCode");
            }

            return RedirectToAction("NewPassword");
        }

        public IActionResult NewPassword()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                TempData["PasswordError"] = "انتهت الجلسة. يرجى المحاولة مرة أخرى.";
                return RedirectToAction("ForgotPassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["PasswordError"] = "كلمات المرور غير متطابقة!";
                return RedirectToAction("NewPassword");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                TempData["PasswordError"] = "المستخدم غير موجود!";
                return RedirectToAction("ForgotPassword");
            }

            // تشفير وتخزين كلمة المرور الجديدة باستخدام PasswordHasher
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

            await _context.SaveChangesAsync();

            TempData["PasswordSuccess"] = "تم تحديث كلمة المرور بنجاح!";
            return RedirectToAction("LoginUserMen");
        }

        // استبدال دالة SendEmail بهذه الدالة المُحدّثة

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // إنشاء رسالة البريد الإلكتروني
                var message = new MimeMessage();

                // تعيين المرسل
                message.From.Add(new MailboxAddress("إعادة تعيين كلمة المرور", "cafeuse18@gmail.com"));

                // تعيين المستلم
                message.To.Add(new MailboxAddress("المستلم", toEmail)); // يمكنك إضافة اسم المستلم هنا

                // تعيين الموضوع والنص
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                // إرسال البريد الإلكتروني
                using (var client = new SmtpClient())
                {
                    // الاتصال بالخادم باستخدام TLS عبر المنفذ 587
                    await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                    // مصادقة البريد الإلكتروني باستخدام كلمة مرور التطبيق
                    await client.AuthenticateAsync("cafeuse18@gmail.com", "mecd idlj jnxt vrrw");

                    // إرسال البريد الإلكتروني
                    await client.SendAsync(message);

                    // قطع الاتصال
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine("تم إرسال البريد الإلكتروني بنجاح!");
            }
            catch (Exception ex)
            {
                // طباعة رسالة الخطأ
                Console.WriteLine($"خطأ في إرسال البريد الإلكتروني: {ex.Message}");
                Console.WriteLine($"تفاصيل الاستثناء: {ex}");
            }
        }





        public IActionResult AboutUs()
        {
            return View();
        }




        public IActionResult ContaciUs()
        {
            return View();
        }


        [HttpPost]

        public IActionResult ContaciUs(Contact contact)
        {

            _context.Contacts.Add(contact);
            _context.SaveChanges();
            ViewBag.Message = "Your message has been sent successfully!";


            return View();
        }





    }
}
