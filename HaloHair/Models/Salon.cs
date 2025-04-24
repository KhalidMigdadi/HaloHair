using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Salon
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? OpeningTime { get; set; }

    public string? ClosingTime { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? AboutSalon { get; set; }

    public bool IsVisible { get; set; }

    public int? BarberOwnerId { get; set; }

    public bool OwnerId { get; set; }

    public bool IsPromoted { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Barber? BarberOwner { get; set; }

    public virtual ICollection<Barber> Barbers { get; set; } = new List<Barber>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<SalonBarber> SalonBarbers { get; set; } = new List<SalonBarber>();

    public virtual ICollection<SalonImage> SalonImages { get; set; } = new List<SalonImage>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<TrainingCourse> TrainingCourses { get; set; } = new List<TrainingCourse>();

    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();

    public virtual ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
}
