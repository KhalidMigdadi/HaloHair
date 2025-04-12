namespace HaloHair.Models
{
    public class SalonImagesViewModel
    {
        public int Id { get; set; }  // إضافة خاصية Id

        public int SalonId { get; set; }
        public List<IFormFile> Images { get; set; }  // دعم رفع عدة صور
        public List<SalonImage> SalonImages { get; set; }  // لعرض الصور الحالية
        public string About { get; set; }  // الخاصية الجديدة


        public ICollection<BarberWorkImage> BarberWorkImages { get; set; }

    }
}
