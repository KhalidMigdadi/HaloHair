using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class BookingService
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public decimal Price { get; set; }

    public int Duration { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
