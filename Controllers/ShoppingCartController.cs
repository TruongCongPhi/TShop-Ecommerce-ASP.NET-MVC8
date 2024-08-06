using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly TshopContext _context;
        private INotyfService _notifyService;
        private ILogger<ShoppingCartController> _logger;
        public ShoppingCartController(TshopContext context, INotyfService notifyService, ILogger<ShoppingCartController> logger)
        {
            _context = context;
            _notifyService = notifyService;
            _logger = logger;
        }
        private List<CartItemVM> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(sessionCart))
            {
                return new List<CartItemVM>();
            }
            return JsonConvert.DeserializeObject<List<CartItemVM>>(sessionCart);
        }

        // Save cart to session
        private void SaveCart(List<CartItemVM> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
        [HttpPost("/api/add/cart")]
        public IActionResult AddToCart(int variantDetailId)
        {
            _logger.LogInformation("id variDetail: "+ variantDetailId);
            var cart = GetCart();
            // Tìm variant detail từ variantDetailId
            var variantDetail = _context.VariantDetails
                .Where(vd => vd.VariantDetailId == variantDetailId)
                .Select(vd => new
                {
                    vd.Variant.Product,
                    vd.Variant.Color,
                    vd.Variant.Thumb,
                    Size = vd.Size.SizeName,
                    vd.Quantity
                })
                .FirstOrDefault();

            if (variantDetail == null)
            {
                _notifyService.Error("Không tìm thấy chi tiết biến thể.");
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            var cartItem = cart.FirstOrDefault(c =>
                            c.Product.ProductId == variantDetail.Product.ProductId &&
                            c.VariantDetailId == variantDetailId &&
                            c.Color.ColorId == variantDetail.Color.ColorId &&
                            c.Size == variantDetail.Size); 
            if (cartItem != null)
            {
                cartItem.Amount++;
            }
            else
            {
                cart.Add(new CartItemVM
                {
                    Product = variantDetail.Product,
                    VariantDetailId = variantDetailId,
                    Amount = 1,
                    Color = variantDetail.Color,
                    Size = variantDetail.Size,
                    Thumb = variantDetail.Thumb
                });
            }
            SaveCart(cart);
            _notifyService.Success("Sản phẩm đã được thêm vào giỏ hàng!");
            return ViewComponent("CartOffCanvas", cart);
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPost("/api/cart/update")]
        public IActionResult UpdateCart(int productId, int colorId, string size, int amount)
        {
            int totalPriceBase = 0;
            int totalDiscount = 0;
            int totalPriceMain = 0;

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c =>
                c.Product.ProductId == productId &&
                c.Color.ColorId == colorId &&
                c.Size == size);

            if (cartItem != null)
            {
                if (amount > 0)
                {
                    cartItem.Amount = amount;
                    SaveCart(cart);
                    foreach (var item in cart)
                    {
                        totalPriceBase += item.Amount * item.Product.Price;
                        totalDiscount += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * (item.Product.Price - item.Product.Discount.Value) : 0;
                        totalPriceMain += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * item.Product.Discount.Value : item.Amount * item.Product.Price;
                    };
                    _logger.LogInformation("giá "+ totalDiscount + " -" + totalPriceBase+"-" + totalPriceMain);
                    return Json(new { success = true, message = "Cập nhật số lượng thành công!", totalPriceBase = totalPriceBase, totalDiscount = totalDiscount, totalPriceMain = totalPriceMain });

                }
                else
                {
                    cart.Remove(cartItem);
                    SaveCart(cart);
                    foreach (var item in cart)
                    {
                        totalPriceBase += item.Amount * item.Product.Price;
                        totalDiscount += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * (item.Product.Price - item.Product.Discount.Value) : 0;
                        totalPriceMain += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * item.Product.Discount.Value : item.Amount * item.Product.Price;
                    };
                    return Json(new { success = true, message = "Đã xóa sản phẩm khỏi giỏ hàng!", totalPriceBase = totalPriceBase, totalDiscount = totalDiscount, totalPriceMain = totalPriceMain,count= cart.Count });

                }
            }
            else
            {
                _notifyService.Error("Không tìm thấy sản phẩm trong giỏ hàng.");
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });

            }

        }
        // Xóa toàn bộ giỏ hàng
        [HttpPost]
        public IActionResult ClearCart()
        {
            SaveCart(new List<CartItemVM>());
            return RedirectToAction("Index");
        }

        // Xóa một sản phẩm khỏi giỏ hàng
        [HttpPost]
        public IActionResult RemoveCartItem(int productId, int colorId, string size)
        {
            _logger.LogInformation("data :"+ productId + colorId + size);
            int totalPriceBase = 0;
            int totalDiscount = 0;
            int totalPriceMain = 0;

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.Product.ProductId == productId && c.Color.ColorId == colorId && c.Size == size);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
                foreach (var item in cart)
                {
                    totalPriceBase += item.Amount * item.Product.Price;
                    totalDiscount += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * (item.Product.Price - item.Product.Discount.Value) : 0;
                    totalPriceMain += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * item.Product.Discount.Value : item.Amount * item.Product.Price;
                };
                return Json(new { success = true, message = "Đã xóa sản phẩm khỏi giỏ hàng", totalPriceBase = totalPriceBase, totalDiscount = totalDiscount, totalPriceMain = totalPriceMain, count = cart.Count });
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng", totalPriceBase = totalPriceBase, totalDiscount = totalDiscount, totalPriceMain = totalPriceMain, count = cart.Count });
            }
        }

        // Display cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }
        [HttpGet("api/cart/count")]
        public IActionResult GetNumberCart()
        {
            return Json(new { success = true, count = GetCart().Count });
        }

    }
}
