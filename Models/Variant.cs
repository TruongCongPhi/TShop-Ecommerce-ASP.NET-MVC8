using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class Variant
{
    public int VariantId { get; set; }

    public int? ProductId { get; set; }

    public int? ColorId { get; set; }

    public string? Thumb { get; set; }

    public virtual Color? Color { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<VariantDetail> VariantDetails { get; set; } = new List<VariantDetail>();
}
