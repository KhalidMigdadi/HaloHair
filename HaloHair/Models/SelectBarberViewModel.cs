namespace HaloHair.Models
{
    public class SelectBarberViewModel
    {

        public int SalonId { get; set; }

        public List<SelectedService> SelectedServices { get; set; }
        public List<BarberViewModel> Barbers { get; set; }
        public string SelectedBarber { get; set; }
        public int SelectedBarberId { get; set; }  // إضافة هذه الخاصية

    }

    public class BarberViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } // لازم تكون موجودة
        public string LastName { get; set; }  // هاي لازم تضيفها كمان
                                             
    }
    }
