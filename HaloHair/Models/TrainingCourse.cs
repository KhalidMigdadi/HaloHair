using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class TrainingCourse
{
    public int Id { get; set; }

    public int? SalonId { get; set; }

    public string? CourseName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Salon? Salon { get; set; }
}
