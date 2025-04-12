namespace HaloHair.Models
{
    public class AddBarberWorkImageViewModel
    {
        public int BarberId { get; set; }
        public IFormFile WorkImage { get; set; }  // لتمثيل الصورة
    }
}
