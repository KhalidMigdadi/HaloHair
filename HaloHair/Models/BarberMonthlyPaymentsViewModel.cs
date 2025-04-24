namespace HaloHair.Models
{
    public class BarberMonthlyPaymentsViewModel
    {

        public List<PaymentInfo> Payments { get; set; } = new List<PaymentInfo>();
        public decimal TotalAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

    }
}
