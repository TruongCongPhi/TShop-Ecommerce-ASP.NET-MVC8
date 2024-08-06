using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TShop.Models;

public partial class Product
{
    public int ProductId { get; set; }

    [DisplayName("Mã sản phẩm")]
    //[Required(ErrorMessage = "Vui lòng nhâp mã sản phẩm")]
    public string? ProductCode { get; set; } = null!;

    [DisplayName("Tên sản phẩm")]
    [Required(ErrorMessage = "Vui lòng nhâp tên sản phẩm ")]
    public string ProductName { get; set; } = null!;

    [DisplayName("Tiêu đề")]
    public string? Title { get; set; }

    [DisplayName("Tên URL")]
    //[Required(ErrorMessage = "Vui lòng nhâp tên URL ")]
    public string? Alias { get; set; }

    [DisplayName("Mô tả")]
    public string? Description { get; set; }

    [DisplayName("Giá gốc")]
    [Required(ErrorMessage = "Vui lòng nhâp giá")]
    public int Price { get; set; }

    [DisplayName("Giá giảm")]
    public int? Discount { get; set; }

    public int? Stock { get; set; }
    [DisplayName("Ngày tạo")]
    public DateTime? DateCreated { get; set; }

    [DisplayName("Cập nhật lần cuối")]
    public DateTime? DateModified { get; set; }

    [DisplayName("Bán chạy")]
    public bool BestSellers { get; set; }
    [DisplayName("Hot Deal")]
    public bool HotDeal { get; set; }

    [DisplayName("Lên trang chủ")]
    public bool HomeFlag { get; set; }

    [DisplayName("Trạng thái")]
    [Required(ErrorMessage = "Vui lòng chọn trạng thái ")]
    public bool Active { get; set; }

    public string? Tags { get; set; }

    [DisplayName("Danh mục")]
    [Required(ErrorMessage = "Vui lòng chọn danh mục")]
    public int CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
