namespace HaloHair.Models
{
    public class SalonBarbersViewModel
    {
        public int Id { get; set; }
        public string SalonName { get; set; }
        public Barber Owner { get; set; } // الحلاق المالك
        public List<Barber> Barbers { get; set; } // باقي الحلاقين
        public bool IsCurrentUserOwner { get; set; } // ✅ إضافة هذا المتغير

        public int SalonId { get; set; }



    }
}
