namespace HaloHair.Models
{
    public class WorkingHoursModel
    {
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public bool? IsClosed { get; set; }  // تغيير النوع إلى bool?
    }
}
