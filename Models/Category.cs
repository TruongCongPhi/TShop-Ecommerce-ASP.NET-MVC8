using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TShop.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    [DisplayName("Tiêu đề")]
    public string? Title { get; set; }

    [DisplayName("Tên danh mục")]
    [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
    public string CategoryName { get; set; } = null!;

    [DisplayName("Mô tả")]
    public string? Description { get; set; }

    [DisplayName("URL")]
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ ALIAS")]
    public string? Alias { get; set; }

    [DisplayName("Danh mục cha")]
    public int? ParentId { get; set; }

    public int? Levels { get; set; }

    [DisplayName("Sắp xếp")]
    public int? Ordering { get; set; }

    [DisplayName("Ảnh")]
    public string? Thumb { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
