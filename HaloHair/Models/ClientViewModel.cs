namespace HaloHair.Models
{
    public class ClientViewModel
    {

        public int UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? LastAppointmentDate { get; set; }
        public int AppointmentsCount { get; set; }


    }
}
