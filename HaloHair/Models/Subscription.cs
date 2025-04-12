using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? SubscriptionType { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
}
