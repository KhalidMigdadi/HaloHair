using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class SalonImage
{
    public int Id { get; set; }

    public int SalonId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public bool IsMainImage { get; set; }

    public virtual Salon Salon { get; set; } = null!;
}
