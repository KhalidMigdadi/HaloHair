using System.Diagnostics;
using HaloHair.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}


        private readonly MyDbContext _context;

        public HomeController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            var salons = _context.Salons
                .Include(s => s.Barbers)
                .Where(s => s.IsVisible == true)
                .ToList();

            var recommendedSalons = new List<SalonViewModel>();


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


            var newSalonsList = _context.Salons
    .Include(s => s.Barbers)
    .Where(s => s.IsVisible == true)
    .OrderByDescending(s => s.CreatedAt)
    .Take(10)
    .ToList();

            var newSalonsViewModel = new List<SalonViewModel>();
            foreach (var salon in newSalonsList)
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
                newSalonsViewModel.Add(new SalonViewModel
                {
                    Salon = salon,
                    AverageRating = averageRating,
                    ReviewCount = reviews.Count
                });
            }

            ViewBag.NewSalons = newSalonsViewModel;


            var promotedSalonsList = _context.Salons
                .Where(s => s.IsPromoted)
                .OrderByDescending(s => s.Id)
                .Take(10)
                .ToList();

            var promotedSalonsViewModel = new List<SalonViewModel>();
            foreach (var salon in promotedSalonsList)
            {
                // Calculate ratings similar to how you did for recommendedSalons
                var barberIds = salon.Barbers.Select(b => b.Id).ToList();
                var bookingHistoryIds = _context.BookingsHistories
                    .Where(bh => barberIds.Contains(bh.BarberId))
                    .Select(bh => bh.Id)
                    .ToList();
                var reviews = _context.Reviews
                    .Where(r => bookingHistoryIds.Contains(r.BookingHistoryId))
                    .ToList();

                double averageRating = 0;
                if (reviews.Any())
                {
                    averageRating = reviews.Average(r => r.Rating ?? 0);
                }

                promotedSalonsViewModel.Add(new SalonViewModel
                {
                    Salon = salon,
                    AverageRating = averageRating,
                    ReviewCount = reviews.Count
                });
            }

            ViewBag.PromotedSalons = promotedSalonsViewModel;




            return View(salons);
        }


        public IActionResult BarberOrUser()
        {
            return View();
        }






















        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
