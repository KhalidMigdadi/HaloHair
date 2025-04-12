using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class AppointmentService
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int? ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public int Duration { get; set; }

    public decimal Price { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Service? Service { get; set; }
}
