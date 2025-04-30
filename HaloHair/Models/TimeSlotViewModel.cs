using System.ComponentModel.DataAnnotations;

namespace HaloHair.Models
{
    public class TimeSlotViewModel
    {
        public int BarberId { get; set; }

        [Display(Name = "From")]
        public DateTime StartTime { get; set; }

        [Display(Name = "To")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Duration of each reservation (minutes)")]
        public int DurationInMinutes { get; set; } = 60; // مدة كل Slot

        // خاصية تمثل الأيام المختارة
        public List<DayOfWeek> AvailableDays { get; set; } = new List<DayOfWeek>();
    }

}
