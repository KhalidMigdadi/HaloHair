using System.Security.Claims;
using HaloHair.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;


namespace HaloHair.Controllers
{
    public class BarberController : Controller
    {

        private readonly MyDbContext _context;

        public BarberController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var barberId = HttpContext.Session.GetInt32("BarberId");


            // Use the parameter if provided, otherwise fall back to session
            int barberIdToUse = barberId ?? HttpContext.Session.GetInt32("BarberId") ?? 0;

            if (barberIdToUse == 0)
            {
                TempData["Error"] = "Session expired! Please log in.";
                return RedirectToAction("LoginBarberMen");
            }




            if (barberId == null || barberId == 0)
            {
                TempData["Error"] = "Session expired! Please log in.";
                return RedirectToAction("LoginBarberMen");
            }

            var barber = _context.Barbers
                .Include(b => b.Salon)
                .FirstOrDefault(b => b.Id == barberId);

            if (barber == null)
            {
                TempData["Error"] = "Barber not found!";
                return RedirectToAction("RegisterBarber");
            }

            ViewBag.BarberId = barberId; // to sent the barberId to AddService action

            // تأكد من أن SalonId ليس فارغًا أو صفرًا
            ViewBag.SalonId = barber.Salon?.Id;


            ViewBag.IsPromoted = barber.Salon?.IsPromoted ?? false;


            var payments = _context.PaymentInfos
                     .Include(p => p.Appointment)
                     .Where(p => p.Appointment.BarberId == barberId)
                     .ToList();

            // حساب المجموع الكلي
            decimal totalAmount = payments.Sum(p => p.Amount);

            // حساب مجموع الشهر الحالي (للمدفوعات التي لها تاريخ)
            var currentDate = DateTime.Now;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            decimal monthlyTotal = payments
                .Where(p =>
                p.PaymentDate >= firstDayOfMonth &&
                p.PaymentDate <= lastDayOfMonth)
                .Sum(p => p.Amount);




            // آخر المدفوعات
            var recentPayments = payments
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .ToList();


         



            ViewBag.TotalAmount = totalAmount;
            ViewBag.MonthlyTotal = monthlyTotal;
            ViewBag.RecentPayments = recentPayments;
            ViewBag.CurrentMonth = currentDate.ToString("MMMM yyyy");

            // to make the total booking in dashbaord dynamic 
            var totalBookings = _context.BookingsHistories
                        .Where(b => b.BarberId == barberId)
                        .Count();

            ViewBag.TotalBookings = totalBookings;

            //get the total users that make register in this barber 
            var uniqueCustomers = _context.BookingsHistories
                       .Where(b => b.BarberId == barberId)
                       .Select(b => b.UserId)
                       .Distinct()
                       .Count();
            ViewBag.UniqueCustomers = uniqueCustomers;


            // the next booking 
            var upcomingBookings = _context.BookingsHistories
                        .Where(b => b.BarberId == barberId && b.BookingDate > DateTime.Now && b.Status == "Confirmed")
                        .Count();
            ViewBag.UpcomingBookings = upcomingBookings;




            // إضافة قائمة العملاء (Clients) إلى ViewBag
            var clients = _context.Appointments
                .Include(a => a.User)
                .Where(a => a.BarberId == barberId && a.UserId != null)
                .GroupBy(a => a.UserId)
                .Select(g => new ClientViewModel
                {
                    UserId = g.Key.Value,
                    FullName = g.First().User.FirstName + " " + g.First().User.LastName,
                    PhoneNumber = g.First().User.PhoneNumber,
                    LastAppointmentDate = g.Max(a => a.AppointmentDate),
                    AppointmentsCount = g.Count()
                })
                .ToList();


            ViewBag.Clients = clients;




            return View(barber);
        }


        // GET: RegisterBarber
        public IActionResult RegisterBarber()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterBarber(Barber model, string Password, string ConfirmPassword)
        {
            var existingBarber = _context.Barbers.FirstOrDefault(u => u.Email == model.Email);
            if (existingBarber != null)
            {
                TempData["Error"] = "Barber already exists!";
                return View(model);
            }

            if (Password != ConfirmPassword)
            {
                TempData["Error"] = "Passwords do not match!";
                return View(model);
            }

            // Hash password
            var passwordHasher = new PasswordHasher<Barber>();
            model.PasswordHash = passwordHasher.HashPassword(model, Password);
            model.CreatedAt = DateTime.Now;
            model.SalonId = null;  // SalonId will be null initially

            // Ensure IsOwner is set correctly
            //model.IsOwner = true; // or true if needed

            // Add barber to the database
            _context.Barbers.Add(model);
            _context.SaveChanges();

            // Save barber data to session after registration
            HttpContext.Session.SetInt32("BarberId", model.Id);
            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetString("FName", model.FirstName);
            HttpContext.Session.SetString("LName", model.LastName);
            HttpContext.Session.SetString("Phone", model.PhoneNumber);
            HttpContext.Session.SetString("Gender", model.Gender);

            TempData["Success"] = "Registration successful! Now add your salon.";
            return RedirectToAction("AddYourSalon", new { barberId = model.Id });
        }


        public IActionResult AddYourSalon(int barberId)
        {
            var barber = _context.Barbers.FirstOrDefault(u => u.Id == barberId);
            if (barber == null)
            {
                TempData["Error"] = "Invalid Barber ID!";
                return RedirectToAction("RegisterBarber");
            }


            var salonModel = new Salon
            {
                BarberOwnerId = barberId  // تحديد OwnerId كـ barberId
            };

            return View(salonModel);
        }









        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddYourSalon(Salon model, int barberId, IFormFile salonImage, Dictionary<string, WorkingHoursModel> workingHours)
        {
            // التحقق من الجلسة
            if (HttpContext.Session.GetInt32("BarberId") != barberId)
            {
                TempData["Error"] = "Session expired! Please register again.";
                return RedirectToAction("RegisterBarber");
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Error in {modelState.Key}: {error.ErrorMessage}");
                    }
                }

                TempData["Error"] = "Check required fields!";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // التعامل مع الصورة
                if (salonImage != null && salonImage.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", salonImage.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await salonImage.CopyToAsync(stream);
                    }
                    model.ImageUrl = "/images/" + salonImage.FileName;
                }

                // ربط الصالون بالحلاق
                model.BarberOwnerId = barberId;

                model.IsVisible = true;


                // إضافة الصالون إلى قاعدة البيانات
                _context.Salons.Add(model);
                await _context.SaveChangesAsync(); // الآن يتم حفظ الصالون في قاعدة البيانات أولاً، و يتم تعيين Id

                // التأكد من أن الـ Id تم تعيينه
                if (model.Id == 0)
                {
                    TempData["Error"] = "Error saving the salon!";
                    return RedirectToAction("AddYourSalon");
                }

                // تحديث الحلاق وتعيين SalonId
                var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.Id == barberId);
                if (barber != null)
                {
                    barber.SalonId = model.Id;  // ربط الحلاق بالصالون
                    barber.IsOwner = true;  // تعيين IsOwner كـ true للحلاق الذي يملك الصالون

                    await _context.SaveChangesAsync();  // حفظ التحديثات في جدول الحلاق
                }

                // إضافة ساعات العمل لكل يوم
                foreach (var day in workingHours)
                {
                    var hour = day.Value;
                    bool isClosed = false; // تعيين قيمة افتراضية
                    if (hour.IsClosed != null) // إذا كانت القيمة موجودة
                    {
                        bool.TryParse(hour.IsClosed.ToString(), out isClosed); // تحويل القيمة إلى bool
                    }

                    // إضافة ساعات العمل للصالون
                    _context.WorkingHours.Add(new WorkingHour
                    {
                        SalonId = model.Id, // تعيين الـ SalonId الصحيح هنا بعد إضافة الصالون
                        DayOfWeek = day.Key,
                        OpeningTime = hour.OpeningTime.HasValue ? TimeOnly.FromTimeSpan(hour.OpeningTime.Value) : (TimeOnly?)null,
                        ClosingTime = hour.ClosingTime.HasValue ? TimeOnly.FromTimeSpan(hour.ClosingTime.Value) : (TimeOnly?)null,
                        IsClosed = isClosed // استخدام القيمة المحولة
                    });
                }

                await _context.SaveChangesAsync(); // حفظ ساعات العمل بعد ربطها بالصّالون

                // عرض رسالة النجاح
                TempData["Success"] = "Salon added successfully! Now add your services.";
                return RedirectToAction("AddServiceBefore", "BarberService", new { barberId = barberId });
            }

            return View(model);
        }










        [HttpGet]
        public IActionResult LoginBarberMen()
        {
            return View();
        }

        public IActionResult LoginBarberMen(Barber barber)
        {

            if (barber.Email == "admin@gmail.com")
            {
                // تخزين معلومات المشرف في الجلسة
                HttpContext.Session.SetString("Email", barber.Email);
                return RedirectToAction("EnterPasswordB", "Barber");
            }



            var loggedBabrber = _context.Barbers.FirstOrDefault(u => u.Email == barber.Email);

            if (loggedBabrber == null)
            {
                TempData["EmailError"] = "Email not found. Please try again.";
                return RedirectToAction("RegisterBarber");
            }

            // Now loggedBabrber is not null, it's safe to access its properties
            if (loggedBabrber.Email == "admin@gmail.com")
            {
                HttpContext.Session.SetString("Email", loggedBabrber.Email);
            }

            // Set session data
            HttpContext.Session.SetString("Email", loggedBabrber.Email);
            HttpContext.Session.SetString("FName", loggedBabrber.FirstName);
            HttpContext.Session.SetString("LName", loggedBabrber.LastName);
            HttpContext.Session.SetString("Phone", loggedBabrber.PhoneNumber);

            return RedirectToAction("EnterPasswordB", "Barber");
        }


        public IActionResult EnterPasswordB()
        {
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("LoginBarberMen", "Barber");
            }

            ViewBag.Email = email;
            return View();
        }

  



        [HttpPost]
        public IActionResult EnterPasswordB(Barber barber, string password)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("LoginBarberMen", "Barber");
            }
            // Check if Super Admin
            if (email == "admin@gmail.com" && password == "admin")
            {
                HttpContext.Session.GetString("Email");
                return RedirectToAction("Dashboard", "SuperAdmin");
            }



            var loggedBarber = _context.Barbers.FirstOrDefault(u => u.Email == email);
            if (loggedBarber != null)
            {
                var passwordHasher = new PasswordHasher<Barber>();
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(loggedBarber, loggedBarber.PasswordHash, password);
                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetInt32("BarberId", loggedBarber.Id); // حفظ barberId في Session
                    HttpContext.Session.SetInt32("SalonId", loggedBarber.SalonId.Value); // حفظ SalonId في Session
                    return RedirectToAction("Index", "Barber", new { barberId = loggedBarber.Id });
                }
                else
                {
                    TempData["PasswordError"] = "Invalid password. Please try again.";
                    return RedirectToAction("EnterPasswordB", "Barber");
                }
            }
            return RedirectToAction("LoginBarberMen", "Barber");
        }










        // to check if the barber is owner or normal barber
        private bool UserIsOwner()
        {
            var barberId = HttpContext.Session.GetInt32("BarberId"); // جلب معرف الحلاق من الجلسة
            if (barberId == null)
            {
                return false;
            }

            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);

            return barber != null && barber.IsOwner; // التحقق مما إذا كان المستخدم مالك الصالون
        }



        public IActionResult CreateBarber()
        {
            // الحصول على الـ BarberId من الجلسة
            var loggedInBarberId = HttpContext.Session.GetInt32("BarberId");
            if (loggedInBarberId == null || loggedInBarberId == 0)
            {
                return RedirectToAction("LoginBarberMen");
            }

            var loggedInBarber = _context.Barbers.Find(loggedInBarberId);
            if (loggedInBarber == null)
            {
                return Unauthorized();
            }

            // إذا كان الحلاق عادي وليس مالك الصالون، يتم منعه من الوصول إلى هذه الصفحة
            if (loggedInBarber.IsOwner == false)
            {
                TempData["ErrorMessage"] = "Only salon owners can create barbers.";
                return RedirectToAction("Index", "Barber"); // أو أي صفحة أخرى تريد توجيه المستخدم إليها
            }

            // تخزين الـ SalonId في ViewData لتمريره إلى الـ View
            ViewData["SalonId"] = loggedInBarber.SalonId;

            return View(new Barber());
        }





        [HttpPost]
        public async Task<IActionResult> CreateBarber(Barber barber, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Password is required.";
                return View();
            }

            var existingBarber = _context.Barbers.FirstOrDefault(b => b.Email == barber.Email);
            if (existingBarber != null)
            {
                TempData["Error"] = "This email is already taken.";
                return View();
            }

            var loggedInBarberId = HttpContext.Session.GetInt32("BarberId");
            if (loggedInBarberId == null || loggedInBarberId == 0)
            {
                TempData["Error"] = "Session expired or invalid Barber ID.";
                return RedirectToAction("LoginBarberMen");
            }

            var loggedInBarber = await _context.Barbers.FindAsync(loggedInBarberId);
            if (loggedInBarber == null || loggedInBarber.IsOwner == false)
            {
                return Unauthorized();
            }

            barber.SalonId = loggedInBarber.SalonId;


            // تشفير كلمة المرور وتخزينها في PasswordHash مباشرةً
            var passwordHasher = new PasswordHasher<Barber>();
            barber.PasswordHash = passwordHasher.HashPassword(barber, password);

            // استخدم الـ barberId الموجود في الجلسة مباشرةً
            barber.SalonId = loggedInBarber.SalonId;

            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();

            TempData["Success"] = "New barber added successfully!";
            return RedirectToAction("Index", "Barber");

        }




        public IActionResult Team(int salonId)
        {
            var salon = _context.Salons
                .Include(s => s.Barbers)
                .FirstOrDefault(s => s.Id == salonId);

            if (salon == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Home");
            }

            var barberId = HttpContext.Session.GetInt32("BarberId"); // جلب BarberId من الـ Session
            var owner = salon.Barbers.FirstOrDefault(b => b.IsOwner);

            var viewModel = new SalonBarbersViewModel
            {
                SalonName = salon.Name,
                Owner = owner,
                Barbers = salon.Barbers.ToList(),
                IsCurrentUserOwner = owner != null && owner.Id == barberId, // ✅ التأكد أن المستخدم الحالي هو المالك
                SalonId = salon.Id // i save it to send it to the delete by button on team view 

            };



            return View(viewModel);
        }


        [HttpPost]
        public IActionResult DeleteBarber(int barberId, int salonId)
        {
            // البحث عن الحلاق في قاعدة البيانات
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);

            // التحقق إذا كان الحلاق غير موجود
            if (barber == null)
            {
                TempData["Error"] = "Barber not found!";
                return RedirectToAction("Team", new { salonId });
            }

            // التحقق إذا كان الحلاق مرتبط بصالون معين
            if (barber.SalonId != salonId)
            {
                TempData["Error"] = "Barber does not belong to this salon!";
                return RedirectToAction("Team", new { salonId });
            }

            // حذف جميع الحجوزات المتعلقة بالحلاق
            var appointments = _context.Appointments.Where(a => a.BarberId == barber.Id).ToList();
            _context.Appointments.RemoveRange(appointments);

            // حذف جميع الخدمات المرتبطة بالحلاق
            var services = _context.Services.Where(s => s.BarberId == barber.Id).ToList();
            _context.Services.RemoveRange(services);

            // حذف الخدمات التي تخص الحلاق من جدول BarberServices
            var barberServices = _context.BarberServices.Where(bs => bs.BarberId == barber.Id).ToList();
            _context.BarberServices.RemoveRange(barberServices);

            // حذف الحلاق نفسه من جدول Barbers
            _context.Barbers.Remove(barber);

            // حفظ التغييرات في قاعدة البيانات
            _context.SaveChanges();

            // عرض رسالة النجاح
            TempData["Success"] = "Barber deleted successfully!";

            // إعادة التوجيه إلى صفحة الفريق في الصالون
            return RedirectToAction("Team", new { salonId });
        }





        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Redirect to home page after logout
            return RedirectToAction("Index", "Home");
        }
































        // 1. عرض صفحة نسيان كلمة المرور
        public IActionResult ForgotPasswordBarber()
        {
            return View();
        }

        // 2. التعامل مع الإيميل المدخل وإرسال الرمز
        [HttpPost]
        public async Task<IActionResult> HandleForgotPasswordBarber(string email)
        {
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.Email == email);
            if (barber == null)
            {
                TempData["ResetError"] = "البريد الإلكتروني غير موجود!";
                return RedirectToAction("ForgotPasswordBarber");
            }

            // إنشاء رمز تحقق
            var verificationCode = new Random().Next(100000, 999999);
            HttpContext.Session.SetString("ResetCodeBarber", verificationCode.ToString());
            HttpContext.Session.SetString("ResetEmailBarber", email);

            await SendEmailAsync(email, "رمز إعادة تعيين كلمة المرور", $"رمز إعادة التعيين الخاص بك هو: {verificationCode}");

            TempData["VerificationMessage"] = "تم إرسال رمز التحقق إلى بريدك الإلكتروني!";
            return RedirectToAction("VerifyResetCodeBarber");
        }







        // 3. عرض صفحة التحقق من الرمز
        public IActionResult VerifyResetCodeBarber()
        {
            var email = HttpContext.Session.GetString("ResetEmailBarber");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPasswordBarber");
            }

            ViewBag.Email = email;
            return View();
        }





        // 4. التحقق من الرمز المدخل
        [HttpPost]
        public IActionResult HandleVerifyCodeBarber(string code)
        {
            var storedCode = HttpContext.Session.GetString("ResetCodeBarber");
            var email = HttpContext.Session.GetString("ResetEmailBarber");

            if (storedCode != code)
            {
                TempData["CodeError"] = "رمز التحقق غير صحيح!";
                return RedirectToAction("VerifyResetCodeBarber");
            }

            return RedirectToAction("NewPasswordBarber");
        }





        // 5. عرض صفحة تعيين كلمة مرور جديدة
        public IActionResult NewPasswordBarber()
        {
            var email = HttpContext.Session.GetString("ResetEmailBarber");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPasswordBarber");
            }

            ViewBag.Email = email;
            return View();
        }

        // 6. تحديث كلمة المرور في قاعدة البيانات
        [HttpPost]
        public async Task<IActionResult> UpdatePasswordBarber(string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("ResetEmailBarber");
            if (string.IsNullOrEmpty(email))
            {
                TempData["PasswordError"] = "انتهت الجلسة. يرجى المحاولة مرة أخرى.";
                return RedirectToAction("ForgotPasswordBarber");
            }

            if (newPassword != confirmPassword)
            {
                TempData["PasswordError"] = "كلمات المرور غير متطابقة!";
                return RedirectToAction("NewPasswordBarber");
            }

            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.Email == email);
            if (barber == null)
            {
                TempData["PasswordError"] = "الحلاق غير موجود!";
                return RedirectToAction("ForgotPasswordBarber");
            }

            var passwordHasher = new PasswordHasher<Barber>();
            barber.PasswordHash = passwordHasher.HashPassword(barber, newPassword);
            await _context.SaveChangesAsync();

            TempData["PasswordSuccess"] = "تم تحديث كلمة المرور بنجاح!";
            return RedirectToAction("LoginBarberMen");
        }




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


    }
}
