using System.Security.Claims;
using HaloHair.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class BarberVacancyController : Controller
    {
        private readonly MyDbContext _context;

        public BarberVacancyController(MyDbContext context)
        {
            _context = context;
        }


        // GET: BarberVacancyController
        public ActionResult AddVacancy()
        {
            int? barberId = HttpContext.Session.GetInt32("BarberId");

            if (barberId == null)
            {
                return RedirectToAction("Barber", "LoginBarberMen"); // المستخدم مش مسجل دخول
            }

            var barber = _context.Barbers.FirstOrDefault(b => b.Id == barberId);

            if (barber == null || barber.IsOwner == false || barber.SalonId == null)
            {
                return Forbid(); // مش مسموح له يضيف شواغر

            }

            return View();
        }


        [HttpPost]
        public IActionResult AddVacancy(Vacancy vacancy)
        {
            int? barberId = HttpContext.Session.GetInt32("BarberId");

            var barber = _context.Barbers.FirstOrDefault(b=> b.Id == barberId);

            if (barber == null || barber.IsOwner == false || barber.SalonId == null)
            {
                return Forbid();
            }


            // تعبئة القيم الناقصة
            vacancy.SalonId = barber.SalonId;
            vacancy.CreatedAt = DateTime.Now;
            vacancy.UpdatedAt = DateTime.Now;
            vacancy.IsActive = true;

            _context.Vacancies.Add(vacancy);
            _context.SaveChanges();

            return RedirectToAction("MyVacancies"); // مثلاً صفحة عرض الشواغر الخاصة فيه
        }

        [HttpGet]
        public IActionResult MyVacancies()
        {
            int barberId = HttpContext.Session.GetInt32("BarberId") ?? 0;
            var barber = _context.Barbers.Find(barberId);

            if (barber == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int salonId = barber.SalonId.Value;

            var viewModel = new VacanciesViewModel
            {
                Vacancies = _context.Vacancies.Where(v => v.SalonId == salonId).ToList(),
                SalonId = salonId,
                BarberId = barberId
            };

            return View(viewModel);
        }



        [HttpGet]
        public IActionResult EditVacancy(int id)
        {
            var vacancy = _context.Vacancies.Find(id);
            if (vacancy == null)
                return NotFound();

            return View(vacancy);
        }

        [HttpPost]
        public IActionResult EditVacancy(Vacancy model)
        {
            if (ModelState.IsValid)
            {
                model.UpdatedAt = DateTime.Now;
                _context.Vacancies.Update(model);
                _context.SaveChanges();
                return RedirectToAction("MyVacancies");
            }
            return View(model);
        }







        [HttpGet]
        public IActionResult DeleteVacancy(int id)
        {
            var vacancy = _context.Vacancies.Find(id);
            if (vacancy == null)
                return NotFound();

            return View(vacancy);
        }

        [HttpPost]
        public IActionResult DeleteVacancyConfirmed(int id)
        {
            var vacancy = _context.Vacancies.Find(id);
            if (vacancy == null)
                return NotFound();

            _context.Vacancies.Remove(vacancy);
            _context.SaveChanges();

            return RedirectToAction("MyVacancies");
        }



        [HttpPost]
        public async Task<IActionResult> Apply(JobApplication model, IFormFile ResumeFile)
        {
            // تحقق من صحة VacancyId
            var vacancy = await _context.Vacancies.FindAsync(model.VacancyId);
            if (vacancy == null)
            {
                ModelState.AddModelError("VacancyId", "الوظيفة غير موجودة.");
                return RedirectToAction("AllJobs");
            }

            if (ResumeFile != null)
            {
                // حفظ الملف
                var fileName = Guid.NewGuid() + Path.GetExtension(ResumeFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/resumes", fileName);

                // تأكد من وجود المجلد
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ResumeFile.CopyToAsync(stream);
                }
                model.ResumeFilePath = "/resumes/" + fileName;
            }

            model.Status = "Pending";
            model.AppliedAt = DateTime.Now;

            _context.JobApplications.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم تقديم طلبك بنجاح!";
            return RedirectToAction("AllJobs");
        }



        public IActionResult Applications()
        {
            var applications = _context.JobApplications
                .Include(a => a.Vacancy)
                .ToList();
            return View(applications);



        }


        [HttpPost]
        public IActionResult Approve(int id)
        {
            var app = _context.JobApplications.Find(id);
            if (app != null)
            {
                app.Status = "Approved";
                _context.SaveChanges();
            }
            return RedirectToAction("Applications");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var app = _context.JobApplications.Find(id);
            if (app != null)
            {
                app.Status = "Rejected";
                _context.SaveChanges();
            }
            return RedirectToAction("Applications");
        }



        public IActionResult MyApplications(string email)
        {
            var apps = _context.JobApplications
                .Include(a => a.Vacancy)
                .Where(a => a.Email == email)
                .ToList();

            return View(apps);
        }




    }
}
