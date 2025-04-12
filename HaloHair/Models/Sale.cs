using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Sale
{
    public int Id { get; set; }

    public int? SalonId { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? SaleDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Salon? Salon { get; set; }
}
