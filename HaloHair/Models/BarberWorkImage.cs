using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class BarberWorkImage
{
    public int Id { get; set; }

    public int BarberId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? SalonId { get; set; }

    public virtual Barber Barber { get; set; } = null!;
}
