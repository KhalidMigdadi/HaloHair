namespace HaloHair.Models
{
    public class BarberScheduleViewModel
    {
        public Barber Barber { get; set; }
        public List<Appointment> Appointments { get; set; }
        public DateTime SelectedDate { get; set; }
    }
}
