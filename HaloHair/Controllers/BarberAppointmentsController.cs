using HaloHair.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class BarberAppointmentsController : Controller
    {

        private readonly MyDbContext _context;

        public BarberAppointmentsController(MyDbContext context)
        {
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> AddTimeSlot(int barberId, DateTime startTime)
        {
            var slot = new TimeSlot
            {
                BarberId = barberId,
                StartTime = startTime,
                EndTime = startTime.AddMinutes(60) 
            };

            _context.TimeSlots.Add(slot);
            await _context.SaveChangesAsync();

            return RedirectToAction("MySchedule"); 
        }






        // عرض النموذج  add time
        public IActionResult Add()
        {
            var barberId = HttpContext.Session.GetInt32("BarberId");

            var model = new TimeSlotViewModel
            {
                BarberId = barberId ?? 0
            };

            return View(model);
        }


        // استلام البيانات وحفظها
        [HttpPost]
        public async Task<IActionResult> Add(TimeSlotViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // إذا كان هناك أيام مختارة
            if (model.AvailableDays == null || !model.AvailableDays.Any())
            {
                ModelState.AddModelError("AvailableDays", "Available days must be specified.");
                return View(model);
            }

            var current = model.StartTime;
            var end = model.EndTime;
            var duration = model.DurationInMinutes;

            foreach (var day in model.AvailableDays)
            {
                var currentDay = current.AddDays(((int)day - (int)current.DayOfWeek + 7) % 7); // تعديل التاريخ ليكون على يوم هذا الأسبوع

                while (currentDay < end)
                {
                    var slot = new TimeSlot
                    {
                        BarberId = model.BarberId,
                        StartTime = currentDay,
                        EndTime = currentDay.AddMinutes(duration)
                    };

                    _context.TimeSlots.Add(slot);
                    currentDay = currentDay.AddMinutes(duration); // ننتقل للوقت التالي
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("MySchedule", new { barberId = model.BarberId });
        }







        public async Task<IActionResult> MySchedule(int barberId)
        {
            var slots = await _context.TimeSlots
                .Where(t => t.BarberId == barberId)
                .OrderBy(t => t.StartTime)
                .ToListAsync();

            return View(slots);
        }






        public async Task<IActionResult> BarberSchedule(int barberId, DateTime? date)
        {
            // إذا كان barberId يساوي صفر، نحاول الحصول عليه من الجلسة
            if (barberId == 0)
            {
                barberId = HttpContext.Session.GetInt32("BarberId") ?? 0;
                // إذا كان لا يزال صفراً، قد نحتاج لتحويل المستخدم إلى صفحة تسجيل الدخول
                if (barberId == 0)
                {
                    // يمكنك توجيهه لصفحة تسجيل الدخول
                    return RedirectToAction("Login", "Account");
                    // أو إعادة رسالة خطأ
                    // return NotFound("Barber ID not found. Please login again.");
                }
            }

            var selectedDate = date ?? DateTime.Today;
            var appointments = await _context.Appointments
                .Where(a => a.BarberId == barberId &&
                           a.AppointmentDate.HasValue &&
                           a.AppointmentDate.Value.Date == selectedDate.Date)
                .OrderBy(a => a.StartTime)
                .Include(a => a.User)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(asv => asv.Service)
                .ToListAsync();

            var barber = await _context.Barbers
                .FirstOrDefaultAsync(b => b.Id == barberId);

            if (barber == null)
                return NotFound();

            // حفظ القيم في الجلسة لاستخدامها في الصفحات الأخرى
            HttpContext.Session.SetInt32("BarberId", barberId);
            HttpContext.Session.SetInt32("SalonId", barber.SalonId.Value);

            // إضافة القيم المطلوبة للـ ViewBag
            ViewBag.BarberId = barberId;
            ViewBag.SalonId = barber.SalonId;

            var viewModel = new BarberScheduleViewModel
            {
                Barber = barber,
                Appointments = appointments,
                SelectedDate = selectedDate
            };

            return View(viewModel);
        }







        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, int barberId, string status, DateTime date)
        {
            // التحقق من وجود الحجز
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.BarberId == barberId);

            if (appointment == null)
            {
                return NotFound();
            }

            // تحديث حالة الحجز
            appointment.Status = status;
            appointment.UpdatedAt = DateTime.Now;

            // إذا تم تغيير الحالة إلى Completed، قم بتحديث BookingsHistory
            if (status == "Completed")
            {
                var bookingHistory = await _context.BookingsHistories
                    .Where(bh => bh.UserId == appointment.UserId &&
                          bh.BarberId == appointment.BarberId &&
                          bh.BookingDate.Date == appointment.AppointmentDate.Value.Date &&
                          bh.Status != "Completed")
                    .FirstOrDefaultAsync();

                if (bookingHistory != null)
                {
                    bookingHistory.Status = "Completed";
                    bookingHistory.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم تحديث حالة الحجز بنجاح!";

            // العودة إلى جدول مواعيد الحلاق لنفس اليوم
            return RedirectToAction("BarberSchedule", new { barberId, date });
        }

    }
}
