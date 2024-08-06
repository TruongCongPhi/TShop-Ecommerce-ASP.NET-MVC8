using TShop.Models;

namespace TShop.ViewModels
{
    public class OrderDetailVM
    {
        public Order Order { get; set; }
        public string FullName {  get; set; }
        public string PhoneNumber { get; set; }

        public string Payment {  get; set; }
        public List<CartItemVM> CartItems { get; set; } = new List<CartItemVM>();

    }

}
