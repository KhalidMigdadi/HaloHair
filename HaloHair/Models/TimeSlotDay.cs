using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class TimeSlotDay
{
    public int Id { get; set; }

    public int TimeSlotId { get; set; }

    public byte Day { get; set; }

    public virtual TimeSlot TimeSlot { get; set; } = null!;
}
