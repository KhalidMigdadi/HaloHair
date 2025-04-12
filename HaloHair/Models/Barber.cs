using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Barber
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public string PasswordHash { get; set; } = null!;

    public int? SalonId { get; set; }

    public DateTime? HireDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UserType { get; set; }

    public bool IsOwner { get; set; }

    public bool IsBlocked { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<BarberService> BarberServices { get; set; } = new List<BarberService>();

    public virtual ICollection<BarberWorkImage> BarberWorkImages { get; set; } = new List<BarberWorkImage>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<BookingsHistory> BookingsHistories { get; set; } = new List<BookingsHistory>();

    public virtual Salon? Salon { get; set; }

    public virtual ICollection<SalonBarber> SalonBarbers { get; set; } = new List<SalonBarber>();

    public virtual ICollection<Salon> Salons { get; set; } = new List<Salon>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
