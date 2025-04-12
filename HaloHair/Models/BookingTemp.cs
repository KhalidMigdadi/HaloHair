using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class BookingTemp
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public int? SalonId { get; set; }

    public string? ServiceName { get; set; }

    public int? Duration { get; set; }

    public decimal? Price { get; set; }

    public int? BarberId { get; set; }

    public DateTime? AppointmentTime { get; set; }

    public DateTime? CreatedAt { get; set; }
}
