using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Service
{
    public int Id { get; set; }

    public int? SalonId { get; set; }

    public string? ServiceName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? Duration { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? BarberId { get; set; }

    public string? BarberFirstName { get; set; }

    public string? BarberLastName { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Barber? Barber { get; set; }

    public virtual ICollection<BarberService> BarberServices { get; set; } = new List<BarberService>();

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public virtual Salon? Salon { get; set; }

    public virtual ICollection<SelectedServiceTemp> SelectedServiceTemps { get; set; } = new List<SelectedServiceTemp>();

    public virtual ICollection<SelectedService> SelectedServices { get; set; } = new List<SelectedService>();
}
