using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class SelectedService
{
    public int Id { get; set; }

    public int SalonId { get; set; }

    public string ServiceName { get; set; } = null!;

    public int Duration { get; set; }

    public decimal Price { get; set; }

    public int UserId { get; set; }

    public string? BarberName { get; set; }

    public int? BarberId { get; set; }

    public int? ServiceId { get; set; }

    public virtual Service? Service { get; set; }

    public virtual User User { get; set; } = null!;
}
