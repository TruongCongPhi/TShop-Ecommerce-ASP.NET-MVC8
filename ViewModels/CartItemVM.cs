using TShop.Models;

namespace TShop.ViewModels
{
    public class CartItemVM
    {
        public Product Product { get; set; }
        public int Amount { get; set; }
        public int TotalMoney => Amount * ((Product.Discount.HasValue && Product.Discount.Value > 0) ? Product.Discount.Value : Product.Price);
        public int TotalDiscount => Amount * ((Product.Discount.HasValue && Product.Discount.Value > 0) ? (Product.Price - Product.Discount.Value) : 0);
        public int VariantDetailId {  get; set; }
        public Color Color { get; set; }
        public string Size { get; set; }
        public string Thumb { get; set; }
    }

        
}
