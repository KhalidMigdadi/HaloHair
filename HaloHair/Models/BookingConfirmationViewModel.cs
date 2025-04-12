namespace HaloHair.Models
{
    public class BookingConfirmationViewModel
    {
        public Booking Booking { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public Barber Barber { get; set; }
        public List<AppointmentService> AppointmentServices { get; set; } = new();

    }
}
