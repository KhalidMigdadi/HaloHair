using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class WorkingHour
{
    public int Id { get; set; }

    public int SalonId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly? OpeningTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }

    public bool IsClosed { get; set; }

    public virtual Salon Salon { get; set; } = null!;
}
