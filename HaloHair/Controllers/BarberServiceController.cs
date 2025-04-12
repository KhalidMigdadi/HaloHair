using HaloHair.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class BarberServiceController : Controller
    {
        private readonly MyDbContext _context;

        public BarberServiceController(MyDbContext context)
        {
            _context = context;
        }



        public IActionResult AddServiceBefore()
        {
            var barberId = HttpContext.Session.GetInt32("BarberId");

            // تحقق من وجود الـ Barber في القاعدة
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);

            if (barber == null || barber.SalonId == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Barber", new { barberId });
            }

            // تعيين الـ ViewBag
            ViewBag.BarberId = barberId;
            ViewBag.SalonId = barber.SalonId;

            // إنشاء نموذج جديد من Service
            var model = new Service();

            // تمرير النموذج إلى الـ View
            return View(model);
        }




        // POST: حفظ الخدمة الجديدة في قاعدة البيانات
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServiceBefore(Service model, int barberId, int SalonId)
        {

            // get the barber Id and name
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);
            if (barber == null)
            {
                TempData["Error"] = "Barber not found!";
                return RedirectToAction("Index", "Barber");
            }

            // get the salon and the name
            var salon = _context.Salons.FirstOrDefault(s => s.Id == SalonId);
            if (salon == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Barber");
            }

            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    ServiceName = model.ServiceName,
                    Description = model.Description,
                    Price = model.Price,
                    Duration = model.Duration,
                    SalonId = SalonId, // link service with salon 
                    BarberId = barberId,
                    CreatedAt = DateTime.Now
                };


                ViewBag.SalonId = SalonId;
                ViewBag.BarberId = barberId;


                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                // بعد إضافة الخدمة، نربطها بالحلاق عبر جدول BarberServices
                var barberService = new BarberService
                {
                    BarberId = barberId,
                    ServiceId = service.Id,
                    BarberFirstName = barber.FirstName,
                    BarberLastName = barber.LastName,
                    SalonName = salon.Name,    // اسم الصالون
                    Price = service.Price ?? 0m,  // إذا كان null، اجعلها 0
                    Duration = service.Duration ?? 0,  // إذا كان null، اجعلها 0
                };

                _context.BarberServices.Add(barberService);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Service added successfully!";
                return RedirectToAction("Index", "Barber", new { barberId = barberId });
            }

            return View(model);
        }






        public IActionResult AddService()
        {
            var barberId = HttpContext.Session.GetInt32("BarberId");

            // تحقق من وجود الـ Barber في القاعدة
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);

            if (barber == null || barber.SalonId == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Barber", new { barberId });
            }

            // تعيين الـ ViewBag
            ViewBag.BarberId = barberId;
            ViewBag.SalonId = barber.SalonId;

            // إنشاء نموذج جديد من Service
            var model = new Service();

            // تمرير النموذج إلى الـ View
            return View(model);
        }




        // POST: حفظ الخدمة الجديدة في قاعدة البيانات
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddService(Service model, int barberId, int SalonId)
        {

            // get the barber Id and name
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);
            if (barber == null)
            {
                TempData["Error"] = "Barber not found!";
                return RedirectToAction("Index", "Barber");
            }

            // get the salon and the name
            var salon = _context.Salons.FirstOrDefault(s => s.Id == SalonId);
            if (salon == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Barber");
            }

            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    ServiceName = model.ServiceName,
                    Description = model.Description,
                    Price = model.Price,
                    Duration = model.Duration,
                    SalonId = SalonId, // link service with salon 
                    BarberId = barberId,
                    CreatedAt = DateTime.Now
                };


                ViewBag.SalonId = SalonId;
                ViewBag.BarberId = barberId;


                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                // بعد إضافة الخدمة، نربطها بالحلاق عبر جدول BarberServices
                var barberService = new BarberService
                {
                    BarberId = barberId,
                    ServiceId = service.Id,
                    BarberFirstName = barber.FirstName,
                    BarberLastName = barber.LastName,
                    SalonName = salon.Name,    // اسم الصالون
                    Price = service.Price ?? 0m,  // إذا كان null، اجعلها 0
                    Duration = service.Duration ?? 0,  // إذا كان null، اجعلها 0
                };

                _context.BarberServices.Add(barberService);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Service added successfully!";
                return RedirectToAction("Index", "Barber", new { barberId = barberId });
            }

            return View(model);
        }

        public IActionResult MyService()
        {
            int? barberId = HttpContext.Session.GetInt32("BarberId"); // get the id from session

            if (barberId == null)
            {
                TempData["Error"] = "Barber Not Found";
                return RedirectToAction("LoginBarberMen", "Barber");
            }
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);
            if (barber == null)
            {
                TempData["Error"] = "Barber Not Found";
                return RedirectToAction("LoginBarberMen", "Barber");
            }

            // i made all the below code to save the salonId to make navegate from EditService to Edit My Salon


            // الحصول على الـ SalonId من الـ Barber
            int salonId = barber.SalonId ?? 0;

            // تخزين الـ SalonId في ViewData
            ViewData["SalonId"] = salonId;



            var services = _context.Services.Where(s => s.BarberId == barberId).ToList(); // give hte all services for this barber

            return View(services);
        }







        public IActionResult EditService(int serviceId)
        {

            var service = _context.Services.FirstOrDefault(s => s.Id == serviceId); // check if the service Id is find in DB


            if (service == null)
            {
                TempData["Error"] = "Service not found!";
                return RedirectToAction("MyServices");
            }



            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(Service model) // service is the services details that sent from view 
        {

            if (ModelState.IsValid)
            {
                var service = await _context.Services.FindAsync(model.Id);

                if (service == null)
                {
                    TempData["Error"] = "Service not found!";
                    return RedirectToAction("MyServices");
                }

                // update the new services detail that sent from view with the old one

                service.ServiceName = model.ServiceName; // ServiceName in model => is the asp-for in the html (view)
                service.Description = model.Description;
                service.Price = model.Price;
                service.Duration = model.Duration;


                await _context.SaveChangesAsync();
                TempData["Success"] = "Service Updated Successfully";
                return RedirectToAction("MyService");
            }


            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);

            if (service == null)
            {
                TempData["Error"] = "Service not found!";
                return RedirectToAction("MyService");
            }


            // حذف جميع العلاقات المرتبطة
            var relatedAppointments = _context.Appointments.Where(a => a.ServiceId == serviceId);
            _context.Appointments.RemoveRange(relatedAppointments);

            var relatedBarberServices = _context.BarberServices.Where(bs => bs.ServiceId == serviceId);
            _context.BarberServices.RemoveRange(relatedBarberServices);

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();


            TempData["Success"] = "Service deleted successfully!";
            return RedirectToAction("MyService");

        }


    }
}
