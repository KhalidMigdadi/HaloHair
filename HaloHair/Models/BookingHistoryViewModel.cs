namespace HaloHair.Models
{
    public class BookingHistoryViewModel
    {
        public Booking Booking { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public Barber Barber { get; set; }
        public List<BookingService> Services { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }
}
