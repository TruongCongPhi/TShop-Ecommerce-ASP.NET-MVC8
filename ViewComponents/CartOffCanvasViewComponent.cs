using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.ViewComponents
{
    public class CartOffCanvasViewComponent: ViewComponent
    {

        public IViewComponentResult Invoke(int customerId)
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            List<CartItemVM> cartItems = string.IsNullOrEmpty(sessionCart) ? new List<CartItemVM>() : JsonConvert.DeserializeObject<List<CartItemVM>>(sessionCart);
            return View(cartItems);
        }
    }
}
