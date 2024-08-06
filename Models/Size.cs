using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TShop.Models;

public partial class Size
{
    public int SizeId { get; set; }

    [DisplayName("Tên size")]
    [Required(ErrorMessage = "Vui lòng nhập tên size")]
    public string SizeName { get; set; } = null!;

    [DisplayName("Nhóm size")]
    public int? ParentId { get; set; }

    [DisplayName("Cấp độ")]
    public int? Levels { get; set; }

    public virtual ICollection<VariantDetail> VariantDetails { get; set; } = new List<VariantDetail>();
}
