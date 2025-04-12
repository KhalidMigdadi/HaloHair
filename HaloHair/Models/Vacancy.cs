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

    public virtual Salon? Salon { get; set; }
}
