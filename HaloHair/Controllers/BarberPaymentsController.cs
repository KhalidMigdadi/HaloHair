using HaloHair.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class BarberPaymentsController : Controller
    {
        private readonly MyDbContext _context;

        public BarberPaymentsController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult MonthlyPayments(int? month, int? year)
        {
            // تحديد الشهر والسنة الحاليين إذا لم يتم توفيرهما
            int currentMonth = month ?? DateTime.Now.Month;
            int currentYear = year ?? DateTime.Now.Year;

            // الحصول على معرف الحلاق المسجل دخول (حسب نظام التسجيل الخاص بك)
            var barberId = HttpContext.Session.GetInt32("BarberId");

            // استعلام عن المدفوعات
            var payments = _context.PaymentInfos
                .Include(p => p.Appointment)
               .Where(p =>
                p.Appointment.BarberId == barberId &&
                p.PaymentDate.Month == currentMonth &&
                p.PaymentDate.Year == currentYear)
                .ToList();



            // حساب المجموع
            decimal totalAmount = payments.Sum(p => p.Amount);

            // إنشاء نموذج عرض
            var viewModel = new BarberMonthlyPaymentsViewModel
            {
                Payments = payments,
                TotalAmount = totalAmount,
                Month = currentMonth,
                Year = currentYear
            };

            return View(viewModel);
        }
    }
}
