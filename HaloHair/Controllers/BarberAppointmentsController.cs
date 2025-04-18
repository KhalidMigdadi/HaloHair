﻿using HaloHair.Models;
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
                EndTime = startTime.AddMinutes(60) // مدة الساعة
            };

            _context.TimeSlots.Add(slot);
            await _context.SaveChangesAsync();

            return RedirectToAction("MySchedule"); // ترجع الحلاق على جدوله
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
                ModelState.AddModelError("AvailableDays", "يجب تحديد الأيام المتاحة");
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




        //public async Task<IActionResult> BarberSchedule(int barberId, DateTime? date)
        //{
        //    var selectedDate = date ?? DateTime.Today;

        //    var appointments = await _context.Appointments
        //       .Where(a => a.BarberId == barberId &&
        //                   a.StartTime.HasValue &&
        //                   a.StartTime.Value.Date == selectedDate.Date)
        //       .OrderBy(a => a.StartTime)
        //       .ToListAsync();


        //    // Also get information about the barber
        //    var barber = await _context.Barbers
        //        .FirstOrDefaultAsync(b => b.Id == barberId);

        //    if (barber == null)
        //        return NotFound();

        //    var viewModel = new BarberScheduleViewModel
        //    {
        //        Barber = barber,
        //        Appointments = appointments,
        //        SelectedDate = selectedDate
        //    };

        //    return View(viewModel);
        //}


        public async Task<IActionResult> BarberSchedule(int barberId, DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;

            var appointments = await _context.Appointments
               .Where(a => a.BarberId == barberId &&
                           a.AppointmentDate.HasValue &&
                           a.AppointmentDate.Value.Date == selectedDate.Date)
               .OrderBy(a => a.StartTime)
               .Include(a => a.User) // معلومات العميل
               .Include(a => a.AppointmentServices) // علاقة مع الخدمات
                   .ThenInclude(asv => asv.Service) // تفاصيل كل خدمة
               .ToListAsync();



            var barber = await _context.Barbers
                .FirstOrDefaultAsync(b => b.Id == barberId);

            if (barber == null)
                return NotFound();

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
                          bh.BookingDate == DateOnly.FromDateTime(appointment.AppointmentDate.Value) &&
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
