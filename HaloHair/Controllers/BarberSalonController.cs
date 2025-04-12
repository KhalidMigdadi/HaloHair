using HaloHair.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class BarberSalonController : Controller
    {
        private readonly MyDbContext _context;

        public BarberSalonController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }




        public IActionResult UploadSalonImages(int salonId)
        {
            var model = new SalonImagesViewModel
            {
                SalonId = salonId
            };

            return View(model);
        }






        [HttpPost]
        public async Task<IActionResult> UploadSalonImages(SalonImagesViewModel model)
        {
            if (model.Images == null || model.Images.Count == 0)
            {
                TempData["Error"] = "Please select at least one image.";
                return RedirectToAction("EditSalon", new { salonId = model.SalonId });
            }

            foreach (var image in model.Images)
            {
                if (image.Length > 0)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                    // حفظ الصورة في المسار المحدد
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // حفظ البيانات في قاعدة البيانات
                    var salonImage = new SalonImage
                    {
                        SalonId = model.SalonId,
                        ImageUrl = "/Images/" + fileName  // تحديد مسار الصورة في قاعدة البيانات
                    };

                    // إضافة الصورة إلى قاعدة البيانات
                    _context.SalonImages.Add(salonImage);
                }
            }

            // حفظ التغييرات في قاعدة البيانات
            await _context.SaveChangesAsync();
            TempData["Success"] = "Images uploaded successfully!";
            return RedirectToAction("EditSalon", "BarberSalon", new { salonId = model.SalonId });
        }





        [HttpPost]
        public async Task<IActionResult> DeleteSalonImage(int imageId)
        {
            // البحث عن الصورة في قاعدة البيانات باستخدام ID الصورة
            var salonImage = await _context.SalonImages.FindAsync(imageId);

            if (salonImage == null)
            {
                // في حال عدم وجود الصورة
                TempData["Error"] = "Image not found.";
                return RedirectToAction("EditSalon", new { id = imageId });
            }

            // حذف الصورة من الخادم (من المجلد الذي تم حفظ الصور فيه)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", Path.GetFileName(salonImage.ImageUrl.TrimStart('/')));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // حذف الصورة من قاعدة البيانات
            _context.SalonImages.Remove(salonImage);

            // حفظ التغييرات في قاعدة البيانات
            await _context.SaveChangesAsync();

            // تحقق من صحة SalonId
            if (salonImage.SalonId == 0)
            {
                TempData["Error"] = "Salon ID is invalid.";
                return RedirectToAction("Home", "Index");
            }

            TempData["Success"] = "Image deleted successfully!";
            Console.WriteLine(salonImage.SalonId);
            return RedirectToAction("EditSalon", new { salonId = salonImage.SalonId });
        }










        public IActionResult EditSalon(int salonId)
        {
            var salon = _context.Salons
                .Include(s => s.SalonImages) // ✅ جلب الصور من قاعدة البيانات
                .FirstOrDefault(s => s.Id == salonId);

            if (salon == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new SalonImagesViewModel
            {
                SalonId = salon.Id,
                SalonImages = salon.SalonImages.ToList() // ✅ تحميل الصور في الـ ViewModel

            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> AddSalonLocation(int salonId)
        {
            // البحث عن الصالون بواسطة المعرّف
            var salon = await _context.Salons.FindAsync(salonId);

            if (salon == null)
            {
                return NotFound();
            }

            // إرسال الصالون إلى العرض
            return View(salon);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSalonLocation(int salonId, double Latitude, double Longitude)
        {
            // البحث عن الصالون
            var salon = await _context.Salons.FindAsync(salonId);

            if (salon == null)
            {
                return NotFound();
            }

            // تحديث بيانات الموقع فقط
            salon.Latitude = Latitude;
            salon.Longitude = Longitude;

            // حفظ التغييرات
            await _context.SaveChangesAsync();

            // إعادة التوجيه إلى صفحة تفاصيل الصالون أو القائمة
            return RedirectToAction("Index", new { id = salonId });
        }





        // GET: Add Location
        //public IActionResult AddSalonLocation(int salonId)
        //{
        //    var salon = _context.Salons.FirstOrDefault(s => s.Id == salonId);
        //    if (salon == null)
        //    {
        //        TempData["Error"] = "Salon not found!";
        //        return RedirectToAction("Dashboard"); // أو أي صفحة أخرى
        //    }

        //    ViewBag.SalonId = salonId; // تمرير salonId إلى الـ View


        //    return View();
        //}

        //// POST: Add Location
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddSalonLocation(int salonId, double latitude, double longitude)
        //{
        //    var salon = _context.Salons.FirstOrDefault(s => s.Id == salonId);
        //    if (salon == null)
        //    {
        //        TempData["Error"] = "Salon not found!";
        //        return RedirectToAction("Dashboard");
        //    }

        //    // تحديث الموقع في قاعدة البيانات
        //    salon.Latitude = latitude;
        //    salon.Longitude = longitude;
        //    _context.SaveChanges();

        //    TempData["Success"] = "Salon location updated successfully!";
        //    return RedirectToAction("Index", "Babrber"); 
        //}




        public IActionResult EditSalonAbout(int salonId)
        {
            var salon = _context.Salons.FirstOrDefault(s => s.Id == salonId);

            if (salon == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Home");
            }

            // التأكد من صلاحيات المالك
            //if (!User.IsInRole("Owner"))
            //{
            //    TempData["Error"] = "You are not authorized to edit this salon.";
            //    return RedirectToAction("Index", "Home");
            //}

            var viewModel = new EditSalonAboutViewModel
            {
                SalonId = salon.Id,
                About = salon.AboutSalon
            };

            return View(viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSalonAbout(EditSalonAboutViewModel model)
        {
            var salon = _context.Salons.FirstOrDefault(s => s.Id == model.SalonId);

            if (salon == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Home");
            }

            // التحقق إذا كان المستخدم هو المالك
            //if (!User.IsInRole("Owner"))
            //{
            //    TempData["Error"] = "You are not authorized to edit this salon.";
            //    return RedirectToAction("Index", "Home");
            //}

            // تحديث قسم About
            salon.AboutSalon = model.About;

            // حفظ التغييرات
            _context.SaveChanges();

            TempData["Success"] = "Salon About section updated successfully!";
            return RedirectToAction("EditSalonAbout", new { salonId = salon.Id });
        }



        [HttpPost]
        public async Task<IActionResult> EditOpeningHours(List<WorkingHour> openingHours)
        {
            foreach (var hour in openingHours)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC UpdateOpeningHours @Id = {0}, @OpeningTime = {1}, @ClosingTime = {2}, @IsClosed = {3}",
                    hour.Id, hour.OpeningTime, hour.ClosingTime, hour.IsClosed
                );
            }

            TempData["Success"] = "Opening hours updated successfully!";
            return RedirectToAction("Dashboard");
        }



        // عرض صفحة تعديل ساعات العمل
        // عرض الساعات في الـ View
        public IActionResult EditWorkingHours(int salonId)
        {
            var salon = _context.Salons.FirstOrDefault(s => s.Id == salonId);

            if (salon == null)
            {
                TempData["Error"] = "Salon not found!";
                return RedirectToAction("Index", "Home");
            }

            var workingHours = _context.WorkingHours
                .Where(wh => wh.SalonId == salonId)
                .ToDictionary(wh => wh.DayOfWeek, wh => new WorkingHoursModel
                {
                    OpeningTime = wh.OpeningTime.HasValue ? (TimeSpan?)wh.OpeningTime.Value.ToTimeSpan() : null,  // التحويل من TimeOnly إلى TimeSpan
                    ClosingTime = wh.ClosingTime.HasValue ? (TimeSpan?)wh.ClosingTime.Value.ToTimeSpan() : null,  // التحويل من TimeOnly إلى TimeSpan
                    IsClosed = wh.IsClosed
                });
            ViewBag.SalonId = salonId;

            return View(workingHours); // تمرير بيانات ساعات العمل إلى الـ View
        }

        // حفظ التعديلات
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkingHours(int salonId, Dictionary<string, WorkingHoursModel> workingHours)
        {
            if (ModelState.IsValid)
            {
                var salon = await _context.Salons.Include(s => s.WorkingHours)
                    .FirstOrDefaultAsync(s => s.Id == salonId);

                if (salon == null)
                {
                    TempData["Error"] = "Salon not found!";
                    return RedirectToAction("Index", "Home");
                }

                // تحديث ساعات العمل
                foreach (var day in workingHours)
                {
                    var hour = salon.WorkingHours.FirstOrDefault(h => h.DayOfWeek == day.Key);
                    if (hour != null)
                    {
                        hour.OpeningTime = day.Value.OpeningTime.HasValue ? TimeOnly.FromTimeSpan(day.Value.OpeningTime.Value) : (TimeOnly?)null;
                        hour.ClosingTime = day.Value.ClosingTime.HasValue ? TimeOnly.FromTimeSpan(day.Value.ClosingTime.Value) : (TimeOnly?)null;

                        hour.IsClosed = day.Value.IsClosed ?? false;
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Working hours updated successfully!";
                return RedirectToAction("EditWorkingHours", new { salonId = salon.Id });
            }

            return View(workingHours);
        }


        public IActionResult AddBarberWorkImage(int barberId)
        {
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);

            if (barber == null)
            {
                TempData["Error"] = "Barber not found!";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new AddBarberWorkImageViewModel
            {
                BarberId = barber.Id
            };

            return View(viewModel);  // عرض النموذج لإضافة الصورة
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBarberWorkImage(AddBarberWorkImageViewModel model)
        {
            var barber = _context.Barbers.FirstOrDefault(b => b.Id == model.BarberId);

            if (barber == null)
            {
                TempData["Error"] = "Barber not found!";
                return RedirectToAction("Index", "Home");
            }

            // تأكد من أن الصورة تم رفعها
            if (model.WorkImage != null && model.WorkImage.Length > 0)
            {
                // حفظ الصورة في المجلد
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "barber_work", model.WorkImage.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.WorkImage.CopyToAsync(stream);
                }

                // إضافة الرابط إلى قاعدة البيانات
                var workImage = new BarberWorkImage
                {
                    BarberId = model.BarberId,
                    ImageUrl = "/images/barber_work/" + model.WorkImage.FileName
                };

                _context.BarberWorkImages.Add(workImage);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Work image added successfully!";
            }
            else
            {
                TempData["Error"] = "Please select a valid image.";
            }

            return RedirectToAction("Index", "Barber", new { barberId = model.BarberId });
        }


    }
}
