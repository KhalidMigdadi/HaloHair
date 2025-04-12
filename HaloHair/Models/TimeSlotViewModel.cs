using System.ComponentModel.DataAnnotations;

namespace HaloHair.Models
{
    public class TimeSlotViewModel
    {
        public int BarberId { get; set; }

        [Display(Name = "من الساعة")]
        public DateTime StartTime { get; set; }

        [Display(Name = "إلى الساعة")]
        public DateTime EndTime { get; set; }

        [Display(Name = "مدة كل حجز (دقائق)")]
        public int DurationInMinutes { get; set; } = 60; // مدة كل Slot

        // خاصية تمثل الأيام المختارة
        public List<DayOfWeek> AvailableDays { get; set; } = new List<DayOfWeek>();
    }

}
