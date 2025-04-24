using HaloHair.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class MenAppointmentsController : Controller
    {

        private readonly MyDbContext _context;

        public MenAppointmentsController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BookingDetails(int salonId, int serviceId)
        {
            var salon = _context.Salons.FirstOrDefault(s => s.Id == salonId);
            var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);

            if (salon == null || service == null)
            {
                TempData["Error"] = "Invalid booking request!";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new BookingDetailsViewModel
            {
                SalonId = salon.Id,
                SalonName = salon.Name,
                ServiceId = service.Id,
                ServiceName = service.ServiceName,
                Price = service.Price ?? 0m, // إذا كان null، استخدم 0
                Duration = service.Duration ?? 0, // إذا كان null، استخدم 0
                Location = salon.Address
            };


            return View(viewModel);
        }


        public IActionResult SelectService(int salonId)
        {
            var salon = _context.Salons
                                .Include(s => s.Services)
                                .FirstOrDefault(s => s.Id == salonId);

            if (salon == null)
                return NotFound();

            return View(salon); // نفترض إن الموديل يحتوي على قائمة Services
        }


        [HttpPost]
        public async Task<IActionResult> SaveSelectedServices(int salonId, [FromBody] List<SelectedService> services)
        {
            if (services == null || !services.Any())
            {
                return Json(new { success = false, message = "No services selected." });
            }

            var salon = await _context.Salons
                .FirstOrDefaultAsync(s => s.Id == salonId);

            if (salon == null)
            {
                return Json(new { success = false, message = "Salon not found." });
            }

            // الحصول على الـ userId من الـ session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            // إضافة الخدمات إلى قاعدة البيانات
            foreach (var service in services)
            {
                // تحديد الـ SalonId و UserId
                service.SalonId = salonId;
                service.UserId = userId.Value;

                // إضافة الخدمة إلى جدول SelectedServices
                _context.SelectedServices.Add(service);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }







        // GET: يعرض الحلاقين
        [HttpGet]
        public async Task<IActionResult> SelectBarberMen()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var selectedServices = await _context.SelectedServices
                .Where(s => s.UserId == userId.Value)
                .ToListAsync();

            if (!selectedServices.Any())
                return RedirectToAction("SelectService", "Service");

            var salonId = selectedServices.FirstOrDefault()?.SalonId;
            var barbers = await _context.Barbers
                .Where(b => b.SalonId == salonId)
                .Select(b => new BarberViewModel
                {
                    Id = b.Id,
                    FirstName = b.FirstName,
                    LastName = b.LastName
                })
                .ToListAsync();

            var viewModel = new SelectBarberViewModel
            {
                SelectedServices = selectedServices,
                Barbers = barbers,
                SelectedBarberId = 0,  // لا يوجد حلاق مختار في البداية
                SelectedBarber = string.Empty  // لا يوجد اسم حلاق مختار في البداية
            };

            return View(viewModel);
        }

        // POST: معالجة البيانات بعد اختيار الحلاق
        [HttpPost]
        public async Task<IActionResult> SelectBarberMen(string selectedBarber, int selectedBarberId)
        {
            // تحقق من البيانات
            Console.WriteLine("Selected Barber Name: " + selectedBarber);
            Console.WriteLine("Selected Barber ID: " + selectedBarberId);

            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var selectedServices = await _context.SelectedServices
                .Where(s => s.UserId == userId.Value)
                .ToListAsync();

            if (!selectedServices.Any())
                return RedirectToAction("SelectService", "Service");

            var salonId = selectedServices.FirstOrDefault()?.SalonId;
            var barbers = await _context.Barbers
                .Where(b => b.SalonId == salonId)
                .Select(b => new BarberViewModel
                {
                    Id = b.Id,
                    FirstName = b.FirstName,
                    LastName = b.LastName
                })
                .ToListAsync();

            if (selectedBarberId != 0)
            {
                // تحديث بيانات الخدمات مع الحلاق الذي تم اختياره
                foreach (var service in selectedServices)
                {
                    service.BarberId = selectedBarberId;
                    service.BarberName = selectedBarber;
                }

                await _context.SaveChangesAsync();
            }

            var viewModel = new SelectBarberViewModel
            {
                SelectedServices = selectedServices,
                Barbers = barbers,
                SelectedBarberId = selectedBarberId,
                SelectedBarber = selectedBarber
            };

            // إعادة إرسال الـ ViewModel إلى الـ View
            return View(viewModel);
        }











        public async Task<IActionResult> ShowAvailableSlots(int barberId, DateTime date)
        {
            var slots = await _context.TimeSlots
                .Where(ts => ts.BarberId == barberId &&
                             ts.StartTime.Date == date.Date &&
                             !_context.Bookings.Any(b => b.TimeSlotId == ts.Id))
                .ToListAsync();

            return View(slots); // صفحة تعرض الأوقات المتاحة
        }





        //public async Task<IActionResult> SelectTime(int barberId, DateTime? selectedDate)
        //{
        //    var date = selectedDate ?? DateTime.Today;

        //    var availableSlots = await _context.TimeSlots
        //        .Where(ts => ts.BarberId == barberId
        //            && ts.StartTime.Date == date.Date
        //            && !_context.Bookings.Any(b => b.TimeSlotId == ts.Id))
        //        .OrderBy(ts => ts.StartTime)
        //        .ToListAsync();

        //    ViewBag.BarberId = barberId;
        //    ViewBag.SelectedDate = date;

        //    return View(availableSlots);
        //}



        //public async Task<IActionResult> SelectTime(int barberId, DateTime? selectedDate)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (!userId.HasValue)
        //        return RedirectToAction("Login", "Account");

        //    var date = selectedDate ?? DateTime.Today;

        //    // Get total duration of all selected services
        //    var selectedServices = await _context.SelectedServices
        //        .Where(s => s.UserId == userId.Value)
        //        .ToListAsync();

        //    var totalDuration = selectedServices.Sum(s => s.Duration);

        //    // Get all available slots for the day
        //    var availableSlots = await _context.TimeSlots
        //        .Where(ts => ts.BarberId == barberId
        //            && ts.StartTime.Date == date.Date
        //            && !_context.Bookings.Any(b => b.TimeSlotId == ts.Id))
        //        .OrderBy(ts => ts.StartTime)
        //        .ToListAsync();

        //    // Find slots that have enough consecutive time
        //    var validSlots = new List<TimeSlot>();

        //    foreach (var slot in availableSlots)
        //    {
        //        // Calculate how many slots we need (each slot is 60 minutes)
        //        int slotsNeeded = (int)Math.Ceiling(totalDuration / 60.0);

        //        // Check if we have enough consecutive slots
        //        bool hasEnoughSlots = true;
        //        DateTime currentTime = slot.StartTime;

        //        for (int i = 0; i < slotsNeeded; i++)
        //        {
        //            // Calculate the start time of the next needed slot
        //            DateTime nextStartTime = slot.StartTime.AddMinutes(i * 60);

        //            // Check if this slot exists and is available
        //            bool slotAvailable = availableSlots.Any(s =>
        //                s.StartTime.Year == nextStartTime.Year &&
        //                s.StartTime.Month == nextStartTime.Month &&
        //                s.StartTime.Day == nextStartTime.Day &&
        //                s.StartTime.Hour == nextStartTime.Hour &&
        //                s.StartTime.Minute == nextStartTime.Minute);

        //            if (!slotAvailable)
        //            {
        //                hasEnoughSlots = false;
        //                break;
        //            }
        //        }

        //        if (hasEnoughSlots)
        //        {
        //            validSlots.Add(slot);
        //        }
        //    }

        //    ViewBag.BarberId = barberId;
        //    ViewBag.SelectedDate = date;
        //    ViewBag.TotalDuration = totalDuration;
        //    ViewBag.SlotsNeeded = (int)Math.Ceiling(totalDuration / 60.0);

        //    return View(validSlots);
        //}

        //[HttpPost]
        //public async Task<IActionResult> BookAppointment(int startingSlotId, int barberId)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (!userId.HasValue)
        //        return RedirectToAction("Login", "Account");

        //    // Get selected services
        //    var selectedServices = await _context.SelectedServices
        //        .Where(s => s.UserId == userId.Value)
        //        .ToListAsync();

        //    if (!selectedServices.Any())
        //        return RedirectToAction("SelectService", "Service");

        //    var totalDuration = selectedServices.Sum(s => s.Duration);
        //    var salonId = selectedServices.First().SalonId; // Get the salon ID from selected services

        //    // Get starting time slot and calculate end time
        //    var startingSlot = await _context.TimeSlots
        //        .FirstOrDefaultAsync(ts => ts.Id == startingSlotId);

        //    if (startingSlot == null)
        //        return NotFound();

        //    DateTime appointmentEndTime = startingSlot.StartTime.AddMinutes(totalDuration);

        //    // Create the appointment with all required fields
        //    var appointment = new Appointment
        //    {
        //        UserId = userId,
        //        BarberId = barberId,
        //        SalonId = salonId, // Include salon ID
        //        StartTime = startingSlot.StartTime,
        //        EndTime = appointmentEndTime,
        //        TotalDuration = totalDuration,
        //        AppointmentDate = startingSlot.StartTime.Date,
        //        Status = "Confirmed",
        //        CreatedAt = DateTime.Now,
        //        UpdatedAt = DateTime.Now
        //    };

        //    // If there's only one service, we can set the ServiceId directly
        //    // Otherwise, we'll need a different approach for multiple services
        //    if (selectedServices.Count == 1)
        //    {
        //        appointment.ServiceId = selectedServices[0].Id;
        //    }

        //    _context.Appointments.Add(appointment);

        //    // Book the time slots
        //    foreach (var slot in await _context.TimeSlots
        //        .Where(ts => ts.BarberId == barberId &&
        //                ts.StartTime >= startingSlot.StartTime &&
        //                ts.StartTime < appointmentEndTime)
        //        .ToListAsync())
        //    {
        //        var booking = new Booking
        //        {
        //            UserId = userId.Value,
        //            TimeSlotId = slot.Id,
        //            BookingDate = DateOnly.FromDateTime(DateTime.Now),
        //            BarberId = barberId
        //        };

        //        _context.Bookings.Add(booking);
        //    }

        //    // Clear selected services
        //    _context.SelectedServices.RemoveRange(selectedServices);

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("BookingSuccess", new { appointmentId = appointment.Id });
        //}



        public async Task<IActionResult> SelectTime(int barberId, DateTime? selectedDate)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var date = selectedDate ?? DateTime.Today;

            // Get total duration of all selected services
            var selectedServices = await _context.SelectedServices
                .Where(s => s.UserId == userId.Value)
                .ToListAsync();

            if (!selectedServices.Any())
                return RedirectToAction("SelectService", "Service");

            var totalDuration = selectedServices.Sum(s => s.Duration);

            // Get all available slots for the day
            var availableSlots = await _context.TimeSlots
                .Where(ts => ts.BarberId == barberId
                    && ts.StartTime.Date == date.Date
                    && !_context.Bookings.Any(b => b.TimeSlotId == ts.Id))
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();

            // Get existing appointments for the barber on that day
            var existingAppointments = await _context.Appointments
                .Where(a => a.BarberId == barberId &&
                       a.AppointmentDate.HasValue &&
                       a.AppointmentDate.Value.Date == date.Date)
                .ToListAsync();

            // Find slots that have enough consecutive time without overlapping existing appointments
            var validSlots = new List<TimeSlot>();

            foreach (var slot in availableSlots)
            {
                // Calculate end time of potential appointment
                DateTime potentialEndTime = slot.StartTime.AddMinutes(totalDuration);

                // Check if this would overlap with any existing appointment
                bool overlapsExisting = existingAppointments.Any(a =>
                    (slot.StartTime < a.EndTime && potentialEndTime > a.StartTime));

                if (overlapsExisting)
                    continue;

                // Check if we have enough consecutive slots
                int slotsNeeded = (int)Math.Ceiling(totalDuration / 60.0);
                bool hasEnoughSlots = true;

                for (int i = 0; i < slotsNeeded; i++)
                {
                    // Calculate the start time of the next needed slot
                    DateTime nextStartTime = slot.StartTime.AddMinutes(i * 60);

                    // Check if this slot exists and is available
                    bool slotAvailable = availableSlots.Any(s =>
                        s.StartTime == nextStartTime);

                    if (!slotAvailable)
                    {
                        hasEnoughSlots = false;
                        break;
                    }
                }

                if (hasEnoughSlots && !overlapsExisting)
                {
                    validSlots.Add(slot);
                }
            }

            ViewBag.BarberId = barberId;
            ViewBag.SelectedDate = date;
            ViewBag.TotalDuration = totalDuration;
            ViewBag.SlotsNeeded = (int)Math.Ceiling(totalDuration / 60.0);

            return View(validSlots);
        }






        //[HttpPost]
        //public async Task<IActionResult> BookAppointment(int startingSlotId, int barberId)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (!userId.HasValue)
        //        return RedirectToAction("Login", "Account");

        //    // Get selected services
        //    var selectedServices = await _context.SelectedServices
        //        .Where(s => s.UserId == userId.Value)
        //        .ToListAsync();

        //    if (!selectedServices.Any())
        //        return RedirectToAction("SelectService", "Service");

        //    var totalDuration = selectedServices.Sum(s => s.Duration);
        //    var totalPrice = selectedServices.Sum(s => s.Price);
        //    var salonId = selectedServices.First().SalonId;

        //    // Get starting time slot and calculate end time
        //    var startingSlot = await _context.TimeSlots
        //        .FirstOrDefaultAsync(ts => ts.Id == startingSlotId);

        //    if (startingSlot == null)
        //        return NotFound();

        //    DateTime appointmentEndTime = startingSlot.StartTime.AddMinutes(totalDuration);

        //    // Create the appointment
        //    var appointment = new Appointment
        //    {
        //        UserId = userId,
        //        BarberId = barberId,
        //        SalonId = salonId,
        //        StartTime = startingSlot.StartTime,
        //        EndTime = appointmentEndTime,
        //        TotalDuration = totalDuration,
        //        AppointmentDate = startingSlot.StartTime.Date,
        //        Status = "Confirmed",
        //        CreatedAt = DateTime.Now,
        //        UpdatedAt = DateTime.Now
        //    };

        //    _context.Appointments.Add(appointment);
        //    await _context.SaveChangesAsync(); // حفظ الموعد أولاً للحصول على رقم الموعد (ID)

        //    // إضافة جميع الخدمات المختارة إلى جدول AppointmentServices
        //    foreach (var selectedService in selectedServices)
        //    {
        //        // تأكد من وجود ServiceId قبل إنشاء AppointmentService
        //        if (selectedService.ServiceId.HasValue)
        //        {
        //            var service = await _context.Services
        //                .FirstOrDefaultAsync(s => s.Id == selectedService.ServiceId.Value);

        //            if (service != null)
        //            {
        //                var appointmentService = new AppointmentService
        //                {
        //                    AppointmentId = appointment.Id,
        //                    ServiceId = service.Id,
        //                    ServiceName = service.ServiceName ?? "Unknown Service",
        //                    Duration = service.Duration ?? 0,
        //                    Price = service.Price ?? 0
        //                };

        //                _context.AppointmentServices.Add(appointmentService);
        //            }
        //        }
        //        else
        //        {
        //            // إذا كان ServiceId فارغًا، ابحث عن الخدمة باستخدام ServiceName
        //            var service = await _context.Services
        //                .FirstOrDefaultAsync(s => s.ServiceName == selectedService.ServiceName);

        //            if (service != null)
        //            {
        //                var appointmentService = new AppointmentService
        //                {
        //                    AppointmentId = appointment.Id,
        //                    ServiceId = service.Id, // استخدم معرف الخدمة التي تم العثور عليها
        //                    ServiceName = selectedService.ServiceName,
        //                    Duration = selectedService.Duration,
        //                    Price = selectedService.Price
        //                };

        //                _context.AppointmentServices.Add(appointmentService);
        //            }
        //            // إذا لم يتم العثور على الخدمة، يمكنك تخطي هذه الخدمة أو التعامل معها بطريقة أخرى
        //        }
        //    }

        //    // Book the time slots
        //    foreach (var slot in await _context.TimeSlots
        //        .Where(ts => ts.BarberId == barberId &&
        //                ts.StartTime >= startingSlot.StartTime &&
        //                ts.StartTime < appointmentEndTime)
        //        .ToListAsync())
        //    {
        //        var booking = new Booking
        //        {
        //            UserId = userId.Value,
        //            TimeSlotId = slot.Id,
        //            BookingDate = DateOnly.FromDateTime(DateTime.Now),
        //            BarberId = barberId
        //        };
        //        _context.Bookings.Add(booking);
        //    }

        //    // إضافة السجل في جدول BookingsHistory
        //    var bookingHistory = new BookingsHistory
        //    {
        //        UserId = userId.Value,
        //        BarberId = barberId,
        //        BookingDate = DateOnly.FromDateTime(startingSlot.StartTime.Date),
        //        StartTime = TimeOnly.FromTimeSpan(startingSlot.StartTime.TimeOfDay),
        //        EndTime = TimeOnly.FromTimeSpan(appointmentEndTime.TimeOfDay),
        //        TotalDuration = totalDuration,
        //        TotalPrice = totalPrice,
        //        Status = "Confirmed",
        //        Notes = $"Appointment made for {selectedServices.Count} services",
        //        CreatedAt = DateTime.Now,
        //        UpdatedAt = DateTime.Now
        //    };

        //    _context.BookingsHistories.Add(bookingHistory);

        //    // Clear selected services
        //    _context.SelectedServices.RemoveRange(selectedServices);

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("BookingSuccess", new { appointmentId = appointment.Id });
        //}




        [HttpPost]
        public async Task<IActionResult> BookAppointment(int startingSlotId, int barberId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            // Get selected services
            var selectedServices = await _context.SelectedServices
                .Where(s => s.UserId == userId.Value)
                .ToListAsync();

            if (!selectedServices.Any())
                return RedirectToAction("SelectService", "Service");

            var totalDuration = selectedServices.Sum(s => s.Duration);
            var totalPrice = selectedServices.Sum(s => s.Price);
            var salonId = selectedServices.First().SalonId;

            // Get starting time slot and calculate end time
            var startingSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == startingSlotId);

            if (startingSlot == null)
                return NotFound();

            DateTime appointmentEndTime = startingSlot.StartTime.AddMinutes(totalDuration);

            // Create the appointment
            var appointment = new Appointment
            {
                UserId = userId,
                BarberId = barberId,
                SalonId = salonId,
                StartTime = startingSlot.StartTime,
                EndTime = appointmentEndTime,
                TotalDuration = totalDuration,
                AppointmentDate = startingSlot.StartTime.Date,
                Status = "Pending", // Set as pending until payment
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(); // Save to get the appointment ID

            // Add all selected services to AppointmentServices table
            foreach (var selectedService in selectedServices)
            {
                if (selectedService.ServiceId.HasValue)
                {
                    var service = await _context.Services
                        .FirstOrDefaultAsync(s => s.Id == selectedService.ServiceId.Value);

                    if (service != null)
                    {
                        var appointmentService = new AppointmentService
                        {
                            AppointmentId = appointment.Id,
                            ServiceId = service.Id,
                            ServiceName = service.ServiceName ?? "Unknown Service",
                            Duration = service.Duration ?? 0,
                            Price = service.Price ?? 0
                        };

                        _context.AppointmentServices.Add(appointmentService);
                    }
                }
                else
                {
                    var service = await _context.Services
                        .FirstOrDefaultAsync(s => s.ServiceName == selectedService.ServiceName);

                    if (service != null)
                    {
                        var appointmentService = new AppointmentService
                        {
                            AppointmentId = appointment.Id,
                            ServiceId = service.Id,
                            ServiceName = selectedService.ServiceName,
                            Duration = selectedService.Duration,
                            Price = selectedService.Price
                        };

                        _context.AppointmentServices.Add(appointmentService);
                    }
                }
            }

            // Book the time slots
            foreach (var slot in await _context.TimeSlots
                .Where(ts => ts.BarberId == barberId &&
                        ts.StartTime >= startingSlot.StartTime &&
                        ts.StartTime < appointmentEndTime)
                .ToListAsync())
            {
                var booking = new Booking
                {
                    UserId = userId.Value,
                    TimeSlotId = slot.Id,
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    BarberId = barberId
                };
                _context.Bookings.Add(booking);
            }

            // Add record to BookingsHistory
            var bookingHistory = new BookingsHistory
            {
                UserId = userId.Value,
                BarberId = barberId,
                BookingDate = startingSlot.StartTime.Date,
                StartTime = TimeOnly.FromTimeSpan(startingSlot.StartTime.TimeOfDay),
                EndTime = TimeOnly.FromTimeSpan(appointmentEndTime.TimeOfDay),
                TotalDuration = totalDuration,
                TotalPrice = totalPrice,
                Status = "Pending",
                Notes = $"Appointment made for {selectedServices.Count} services",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.BookingsHistories.Add(bookingHistory);

            // Clear selected services
            _context.SelectedServices.RemoveRange(selectedServices);

            await _context.SaveChangesAsync();

            // Store appointment data in TempData for the payment page
            TempData["AppointmentId"] = appointment.Id;
            TempData["Amount"] = totalPrice.ToString();



            // Redirect to simple payment page
            return RedirectToAction("PaymentPage");
        }




        // 4. Simple payment page action
        public IActionResult PaymentPage()
        {
            // Get data from TempData
            var appointmentId = TempData["AppointmentId"];
            var amount = TempData["Amount"];

            // Keep the data in TempData for the post action
            TempData.Keep("AppointmentId");
            TempData.Keep("Amount");

            ViewBag.AppointmentId = appointmentId;
            ViewBag.Amount = amount;

            return View();
        }



        // 5. Process payment action
        // تعديل دالة ProcessPayment
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string paymentMethod, string paymentDetails)
        {
            var appointmentId = Convert.ToInt32(TempData["AppointmentId"]);
            var amountString = TempData["Amount"]?.ToString();
            decimal amount = decimal.Parse(amountString);

            // Get the appointment
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Create payment record
            var payment = new PaymentInfo
            {
                AppointmentId = appointmentId,
                BarberId = appointment.BarberId.Value, // أضف معرف الحلاق من الموعد
                PaymentMethod = paymentMethod,
                PaymentDetails = paymentDetails,
                Amount = amount,
                PaymentDate = DateTime.Now // من المهم تضمين تاريخ الدفع أيضًا
            };

            _context.PaymentInfos.Add(payment);

            // Update appointment status
            appointment.Status = "Confirmed";
            appointment.UpdatedAt = DateTime.Now;

            // Update booking history status
            var bookingHistory = await _context.BookingsHistories
                .FirstOrDefaultAsync(bh => bh.UserId == appointment.UserId &&
                                           bh.BarberId == appointment.BarberId &&
                                           bh.BookingDate.Date == (appointment.AppointmentDate ?? DateTime.Now).Date);
            if (bookingHistory != null)
            {
                bookingHistory.Status = "Confirmed";
                bookingHistory.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            // التوجيه إلى BookingSuccess
            return RedirectToAction("BookingSuccess", new { appointmentId = appointmentId });
        }







        public async Task<IActionResult> BookingConfirmation(int bookingId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            // Get the booking
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.TimeSlotId == bookingId);

            if (booking == null)
                return NotFound();

            // Separately get the time slot
            var timeSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == booking.TimeSlotId);

            // Separately get the barber
            var barber = await _context.Barbers
                .FirstOrDefaultAsync(b => b.Id == booking.BarberId);

            // Create a view model to hold all the data
            var viewModel = new BookingConfirmationViewModel
            {
                Booking = booking,
                TimeSlot = timeSlot,
                Barber = barber
            };

            return View(viewModel);
        }



        public async Task<IActionResult> BookingSuccess(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Store appointment details in TempData
            TempData["AppointmentDate"] = appointment.AppointmentDate.HasValue
                ? appointment.AppointmentDate.Value.ToString("dddd, MMMM d, yyyy")
                : "N/A";
            TempData["AppointmentTime"] = $"{appointment.StartTime?.ToShortTimeString()} - {appointment.EndTime?.ToShortTimeString()}";
            TempData["BarberName"] = $"{appointment.Barber.FirstName} {appointment.Barber.LastName}";

            // Fetch payment information for this appointment
            var paymentInfo = await _context.PaymentInfos
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);

            if (paymentInfo != null)
            {
                TempData["PaymentMethod"] = paymentInfo.PaymentMethod;
                TempData["PaymentAmount"] = paymentInfo.Amount.ToString("N2");
                TempData["PaymentDate"] = DateTime.Now.ToString("dddd, MMMM d, yyyy");
                // Only include last 4 digits if payment details contain credit card info
                TempData["PaymentDetails"] = paymentInfo.PaymentDetails;
            }

            // Fetch all services related to this appointment
            var appointmentServices = await _context.AppointmentServices
                .Where(a => a.AppointmentId == appointmentId)
                .Include(a => a.Service)
                .ToListAsync();

            // تسجيل عدد الخدمات المسترجعة للتصحيح
            System.Diagnostics.Debug.WriteLine($"Found {appointmentServices.Count} services for appointment {appointmentId}");

            ViewBag.AppointmentServices = appointmentServices;
            foreach (var a in appointmentServices)
            {
                Console.WriteLine($"ServiceId: {a.ServiceId}, ServiceName: {a.Service?.ServiceName ?? "NULL"}");
            }

            return View();
        }



        public IActionResult CancelAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            // تحقق إذا كان موعد الحجز بعد أكثر من 12 ساعة
            var hoursUntilAppointment = (appointment.AppointmentDate - DateTime.Now)?.TotalHours ?? 0;

            if (hoursUntilAppointment < 12)
            {
                TempData["ErrorMessage"] = "لا يمكنك إلغاء الحجز قبل أقل من 12 ساعة من موعده.";
                return RedirectToAction("MyAppointments"); // أو أي صفحة تروح لها بعد المحاولة
            }

            // نفذ الإلغاء
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "تم إلغاء الحجز بنجاح.";
            return RedirectToAction("MyAppointments");
        }











        //[HttpPost]
        //public async Task<IActionResult> ConfirmBooking(int userId, int barberId, int timeSlotId)
        //{
        //    var booking = new Booking
        //    {
        //        UserId = userId,
        //        BarberId = barberId,
        //        TimeSlotId = timeSlotId,
        //        BookingDate = DateOnly.FromDateTime(DateTime.Now)
        //    };

        //    _context.Bookings.Add(booking);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("MyAppointments");
        //}




        //public async Task<IActionResult> BookingHistory()
        //{
        //    // الحصول على معرف المستخدم الحالي من الجلسة
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (!userId.HasValue)
        //        return RedirectToAction("Login", "Account");

        //    // الحصول على جميع حجوزات المستخدم
        //    var bookings = await _context.Bookings
        //        .Where(b => b.UserId == userId.Value)
        //        .OrderByDescending(b => b.BookingDate)
        //        .ToListAsync();

        //    // إنشاء قائمة لتخزين نماذج العرض
        //    var bookingViewModels = new List<BookingHistoryViewModel>();

        //    foreach (var booking in bookings)
        //    {
        //        // جلب معلومات الفترة الزمنية المحجوزة
        //        var timeSlot = await _context.TimeSlots
        //            .FirstOrDefaultAsync(ts => ts.Id == booking.TimeSlotId);

        //        // جلب معلومات الحلاق
        //        var barber = await _context.Barbers
        //            .FirstOrDefaultAsync(b => b.Id == booking.BarberId);

        //        // جلب الخدمات المرتبطة بالحجز
        //        var bookingServices = await _context.BookingServices
        //            .Where(bs => bs.BookingId == booking.Id)
        //            .ToListAsync();

        //        // إضافة نموذج العرض إلى القائمة
        //        bookingViewModels.Add(new BookingHistoryViewModel
        //        {
        //            Booking = booking,
        //            TimeSlot = timeSlot,
        //            Barber = barber,
        //            Services = bookingServices
        //        });
        //    }

        //    return View(bookingViewModels);
        //}




    }
}
