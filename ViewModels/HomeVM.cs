using TShop.Models;

namespace TShop.ViewModels
{
    public class HomeVM
    {
        public List<ProductCategoryVM> Categories { get; set; } = new List<ProductCategoryVM>();
        public List<ProductTypeCategoryVM> AoPhong { get; set; } = new List<ProductTypeCategoryVM>();
        public List<ProductTypeCategoryVM> AoPolo { get; set; } = new List<ProductTypeCategoryVM>();
        public List<ProductTypeCategoryVM> QuanShorts { get; set; } = new List<ProductTypeCategoryVM>();
        public List<ProductTypeCategoryVM> Vay { get; set; } = new List<ProductTypeCategoryVM>();
        public List<ProductVM> HotDeal { get; set; } = new List<ProductVM>();
        public List<ProductVM> BestSeller { get; set; } = new List<ProductVM>();
        public List<ProductVM> NewArrivals { get; set; } = new List<ProductVM>();
        public List<CartItemVM> CartItems { get; set; }
        public List<Blog> Blogs { get; set; } = new List<Blog>();

    }
    public class ProductCategoryVM
    {
        public Category Category { get; set; }
        public List<ProductVM> ProductCategorys { get; set; } = new List<ProductVM>();
    }

    public class ProductTypeCategoryVM
    {
        public Category TypeCategory{ get; set; }
        public List<ProductVM> ProductTypeCats { get; set; } = new List<ProductVM>();
    }
}
