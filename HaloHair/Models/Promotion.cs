using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Promotion
{
    public int Id { get; set; }

    public int? SalonId { get; set; }

    public string? PromotionTitle { get; set; }

    public string? Description { get; set; }

    public decimal? Discount { get; set; }

    public DateTime? ValidUntil { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Salon? Salon { get; set; }
}
