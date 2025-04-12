using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BarberId { get; set; }

    public int TimeSlotId { get; set; }

    public DateOnly BookingDate { get; set; }

    public virtual Barber Barber { get; set; } = null!;

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public virtual User User { get; set; } = null!;
}
