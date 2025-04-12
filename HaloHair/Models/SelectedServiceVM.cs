namespace HaloHair.Models
{

    // مشان اخزن البيانات من front ل table BookingTemp
    public class SelectedServiceVM
    {
        public int Id { get; set; }
        public int SalonId { get; set; }
        public string ServiceName { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
    }
}
