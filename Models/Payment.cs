using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string? PaymentType { get; set; }

    public bool? Allowed { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
