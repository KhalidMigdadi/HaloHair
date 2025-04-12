using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BookingHistoryId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? SalonId { get; set; }

    public virtual BookingsHistory BookingHistory { get; set; } = null!;

    public virtual Salon? Salon { get; set; }

    public virtual User User { get; set; } = null!;
}
