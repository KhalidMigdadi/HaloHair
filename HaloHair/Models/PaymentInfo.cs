using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class PaymentInfo
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentDetails { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public int BarberId { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Barber Barber { get; set; } = null!;
}
