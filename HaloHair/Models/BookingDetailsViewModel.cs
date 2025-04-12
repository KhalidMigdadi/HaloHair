namespace HaloHair.Models
{
    public class BookingDetailsViewModel
    {
        public int SalonId { get; set; }
        public string SalonName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Location { get; set; }
    }
}
