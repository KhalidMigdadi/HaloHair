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




        public IActionResult SelectService(int salonId)
        {
            var salon = _context.Salons
                                .Include(s => s.Services)
                                .FirstOrDefault(s => s.Id == salonId);

            if (salon == null)
                return NotFound();

            return View(salon); // نفترض إن الموديل يحتوي على قائمة Services
        }


        // add list services to DB

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

            foreach (var service in services)
            {
                // تحديد الـ SalonId و UserId
                service.SalonId = salonId;
                service.UserId = userId.Value;

                _context.SelectedServices.Add(service);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }










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

            // Get all available slots for the day - SIMPLIFIED QUERY
            var availableSlots = await _context.TimeSlots
                .Where(ts => ts.BarberId == barberId
                    && ts.StartTime.Date == date.Date
                    && ts.IsBooked == false)  // Only check IsBooked flag
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();

            // Get existing appointments for the barber on that day that are not cancelled
            var existingAppointments = await _context.Appointments
                .Where(a => a.BarberId == barberId &&
                       a.AppointmentDate.HasValue &&
                       a.AppointmentDate.Value.Date == date.Date &&
                       a.Status != "Cancelled")  // Only consider non-cancelled appointments
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

                    // Check if this slot exists and is available (not booked)
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

            // إزالة التكرار حسب StartTime
            var distinctValidSlots = validSlots
                .GroupBy(slot => slot.StartTime)
                .Select(group => group.First())
                .ToList();

            return View(distinctValidSlots);
        }


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
                Status = "Confirmed", 
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

                // Set the IsBooked flag to true
                slot.IsBooked = true;
                _context.TimeSlots.Update(slot);

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
                Status = "Confirmed",
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
                BarberId = appointment.BarberId.Value,
                PaymentMethod = paymentMethod,
                PaymentDetails = paymentDetails,
                Amount = amount,
                PaymentDate = DateTime.Now,
                Status = "Completed" // Set initial status
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

            // Redirect to BookingSuccess
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






        // Add this method to MenProfileController.cs
        // عند إلغاء الحجز يجب التأكد من أنه يتم إزالة الوقت من الحجز
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            // Find the booking history entry
            var bookingHistory = await _context.BookingsHistories
                .FirstOrDefaultAsync(bh => bh.Id == bookingId && bh.UserId == userId.Value);

            if (bookingHistory == null)
                return NotFound();

            try
            {
                // Start a transaction to ensure all operations succeed or fail together
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // Find related appointment
                    var appointment = await _context.Appointments
                        .FirstOrDefaultAsync(a => a.UserId == userId.Value &&
                                                a.BarberId == bookingHistory.BarberId &&
                                                a.AppointmentDate.HasValue &&
                                                a.AppointmentDate.Value.Date == bookingHistory.BookingDate.Date &&
                                                a.StartTime.HasValue &&
                                                a.StartTime.Value.TimeOfDay == bookingHistory.StartTime.ToTimeSpan());

                    if (appointment != null)
                    {
                        // Find the start and end time of the appointment
                        var appointmentStart = appointment.StartTime.Value;
                        var appointmentEnd = appointment.EndTime.Value;

                        // Find all time slots covered by this appointment
                        var timeSlots = await _context.TimeSlots
                            .Where(ts => ts.BarberId == bookingHistory.BarberId &&
                                   ts.StartTime >= appointmentStart &&
                                   ts.StartTime < appointmentEnd)
                            .ToListAsync();

                        // Log how many slots we found
                        System.Diagnostics.Debug.WriteLine($"Found {timeSlots.Count} time slots to free up");

                        foreach (var slot in timeSlots)
                        {
                            // Mark each time slot as available
                            slot.IsBooked = false;
                            _context.TimeSlots.Update(slot);
                            System.Diagnostics.Debug.WriteLine($"Freed up slot: {slot.Id} at {slot.StartTime}");
                        }

                        // Find and delete ALL related bookings for this appointment
                        var bookings = await _context.Bookings
                            .Where(b => b.BarberId == bookingHistory.BarberId &&
                                    timeSlots.Select(ts => ts.Id).Contains(b.TimeSlotId))
                            .ToListAsync();

                        System.Diagnostics.Debug.WriteLine($"Removing {bookings.Count} booking records");

                        // Remove the bookings
                        _context.Bookings.RemoveRange(bookings);

                        // Mark appointment as cancelled
                        appointment.Status = "Cancelled";
                        appointment.UpdatedAt = DateTime.Now;
                        _context.Appointments.Update(appointment);
                    }
                    else
                    {
                        // If appointment not found, use booking history to find time slots
                        var startTime = bookingHistory.StartTime.ToTimeSpan();
                        var endTime = bookingHistory.EndTime.ToTimeSpan();
                        var bookingDate = bookingHistory.BookingDate;

                        var timeSlots = await _context.TimeSlots
                            .Where(ts => ts.BarberId == bookingHistory.BarberId &&
                                   ts.StartTime.Date == bookingDate.Date &&
                                   ts.StartTime.TimeOfDay >= startTime &&
                                   ts.StartTime.TimeOfDay < endTime)
                            .ToListAsync();

                        System.Diagnostics.Debug.WriteLine($"Found {timeSlots.Count} time slots to free up using booking history");

                        foreach (var slot in timeSlots)
                        {
                            slot.IsBooked = false;
                            _context.TimeSlots.Update(slot);
                            System.Diagnostics.Debug.WriteLine($"Freed up slot: {slot.Id} at {slot.StartTime}");
                        }

                        // Find and delete all related bookings
                        var bookings = await _context.Bookings
                            .Where(b => b.BarberId == bookingHistory.BarberId &&
                                    timeSlots.Select(ts => ts.Id).Contains(b.TimeSlotId))
                            .ToListAsync();

                        System.Diagnostics.Debug.WriteLine($"Removing {bookings.Count} booking records");
                        _context.Bookings.RemoveRange(bookings);
                    }

                    // Update the booking history status
                    bookingHistory.Status = "Cancelled";
                    bookingHistory.UpdatedAt = DateTime.Now;
                    _context.BookingsHistories.Update(bookingHistory);

                    // Save all changes
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();
                }

                TempData["SuccessMessage"] = "Your appointment has been successfully cancelled.";
                return RedirectToAction("Profile", "MenProfile");
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error cancelling booking: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while cancelling your appointment. Please try again.";
                return RedirectToAction("Profile", "MenProfile");
            }
        }










        private async Task<bool> AreTimeSlotsAvailable(int barberId, DateTime startTime, DateTime endTime)
        {
            // Check if any time slot in the range is already booked
            return !await _context.TimeSlots
                .AnyAsync(ts => ts.BarberId == barberId &&
                        ts.StartTime >= startTime &&
                        ts.StartTime < endTime &&
                        ts.IsBooked);
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

    }
}