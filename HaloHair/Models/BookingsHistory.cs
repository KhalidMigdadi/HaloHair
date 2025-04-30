using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class BookingsHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BarberId { get; set; }

    public DateTime BookingDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int TotalDuration { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? TimeSlotId { get; set; }

    public virtual Barber Barber { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual TimeSlot? TimeSlot { get; set; }

    public virtual User User { get; set; } = null!;
}
