using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Appointment
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? SalonId { get; set; }

    public int? ServiceId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? BarberId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? TotalDuration { get; set; }

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual Barber? Barber { get; set; }

    public virtual Salon? Salon { get; set; }

    public virtual Service? Service { get; set; }

    public virtual User? User { get; set; }
}
