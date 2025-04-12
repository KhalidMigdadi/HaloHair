using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class SalonBarber
{
    public int Id { get; set; }

    public int SalonId { get; set; }

    public int BarberId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual Barber Barber { get; set; } = null!;

    public virtual Salon Salon { get; set; } = null!;
}
