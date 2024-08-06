using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TShop.Models;

public partial class Color
{
    public int ColorId { get; set; }
    [DisplayName("Tên màu")]
    [Required(ErrorMessage = "Vui lòng nhập tên màu")]
    public string ColorName { get; set; } = null!;

    [DisplayName("Nhóm màu")]
    public int? ParentId { get; set; }

    [DisplayName("Cấp độ")]
    public int? Levels { get; set; }

    [DisplayName("Mã màu")]
    [Required(ErrorMessage = "Vui lòng chọn mã màu")]
    public string? ColorHex { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
