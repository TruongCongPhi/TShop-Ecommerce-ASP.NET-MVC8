using TShop.Models;

namespace TShop.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ProductCode { get; set; }

        public string Alias { get; set; }
        public int Price { get; set; }
        public int? Discount { get; set; }
        public bool HotDeal { get; set; }
        public bool BestSeller { get; set; }
        public string CategoryName { get; set; }
        public string CategoryAlias { get; set; }
        public List<VariantVM> Variants { get; set; }
    }

    public class VariantVM
    {
        public int VariantId { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorHex{ get; set; }
        public string Thumb { get; set; }
        public List<ProductImage> productImages { get; set; }
        public List<VariantDetailVM> VariantDetails { get; set; }

    }
    public class VariantDetailVM
    {
        public int VariantDetailId { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int Quantity { get; set; }

    }
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public List<VariantVM> Variants { get; set; }
        public List<ProductCommentVM> ProductCommentVM { get; set; }
        public List<ProductVM> RecommendedProducts { get; set; } = new List<ProductVM>();

    }
    public class ProductCommentVM
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime? CreateAt { get; set; }                                                                                                                                                                      
    }
}