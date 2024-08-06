using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class ProductImage
{
    public int ImageId { get; set; }

    public int? VariantId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public virtual Variant? Variant { get; set; }
}
