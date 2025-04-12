using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class TimeSlot
{
    public int Id { get; set; }

    public int BarberId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public virtual Barber Barber { get; set; } = null!;
}
