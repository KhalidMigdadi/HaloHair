using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class BarberService
{
    public int Id { get; set; }

    public int BarberId { get; set; }

    public int ServiceId { get; set; }

    public decimal Price { get; set; }

    public int Duration { get; set; }

    public string BarberFirstName { get; set; } = null!;

    public string SalonName { get; set; } = null!;

    public string BarberLastName { get; set; } = null!;

    public virtual Barber Barber { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
