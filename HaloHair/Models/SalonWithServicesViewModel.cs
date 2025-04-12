namespace HaloHair.Models
{
    public class SalonWithServicesViewModel
    {
        public Salon Salon { get; set; }

        public List<SelectedServiceItem> Services { get; set; } = new List<SelectedServiceItem>();
        public List<SelectedServiceItem> SavedServices { get; set; }  // للخدمات المحفوظة

    }

    public class SelectedServiceItem
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
    }
}
