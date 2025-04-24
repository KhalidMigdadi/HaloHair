using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class SelectedServiceTemp
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public int Duration { get; set; }

    public decimal Price { get; set; }

    public string? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? SalonId { get; set; }

    public virtual Service Service { get; set; } = null!;
}
