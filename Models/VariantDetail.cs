using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class VariantDetail
{
    public int VariantDetailId { get; set; }

    public int? VariantId { get; set; }

    public int? SizeId { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Size? Size { get; set; }

    public virtual Variant? Variant { get; set; }
}
