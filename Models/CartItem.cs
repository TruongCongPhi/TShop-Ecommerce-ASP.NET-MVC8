using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? CartId { get; set; }

    public int? VariantDetailId { get; set; }

    public int? Quantity { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual VariantDetail? VariantDetail { get; set; }
}
