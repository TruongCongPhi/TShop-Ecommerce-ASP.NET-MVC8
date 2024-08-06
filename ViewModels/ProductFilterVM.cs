namespace TShop.ViewModels
{
    public class ProductFilterVM
    {
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 24;

    }
}
