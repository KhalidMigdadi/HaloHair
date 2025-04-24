using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Vacancy
{
    public int Id { get; set; }

    public int? SalonId { get; set; }

    public string? Position { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Location { get; set; } = null!;

    public string Salary { get; set; } = null!;

    public string Requirements { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

    public virtual Salon? Salon { get; set; }
}
