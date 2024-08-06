using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly TshopContext _context;
        public CheckoutController(TshopContext context)
        {
            _context = context;
        } 

        [HttpGet("/checkout")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var AccountIdClaim = User.FindFirst("AccountId")?.Value;
                if (!string.IsNullOrEmpty(AccountIdClaim) && int.TryParse(AccountIdClaim, out int AccountId))
                {
                    var Account = _context.Accounts.FirstOrDefault(c => c.AccountId == AccountId);

                    if (Account == null)
                    {
                        return RedirectToAction("Index","Home");
                    }
                    var sessionCart = HttpContext.Session.GetString("Cart");
                    if (string.IsNullOrEmpty(sessionCart))
                    {
                        return View(new List<CartItemVM>());
                    }
                    var cart = JsonConvert.DeserializeObject<List<CartItemVM>>(sessionCart);

                    int totalPriceBase = 0;
                    int totalDiscount = 0;
                    int totalPriceMain = 0;
                    foreach (var item in cart)
                    {
                        totalPriceBase += item.Amount * item.Product.Price;
                        totalDiscount += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * (item.Product.Price - item.Product.Discount.Value) : 0;
                        totalPriceMain += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * item.Product.Discount.Value : item.Amount * item.Product.Price;
                    };
                    var model = new CheckoutVM()
                    {
                        FullName = Account.FullName,
                        PhoneNumber = Account.Phone,
                        
                    };
                    
                    model.Payments = _context.Payments.Where(p => p.Allowed == true).ToList();
                    
                    ViewData["Citys"] = new SelectList(_context.Locations.Where(l => l.Levels == null).ToList(),"LocationId","Name");
                    ViewBag.TotalPriceBase = totalPriceBase;
                    ViewBag.TotalDiscount = totalDiscount;
                    ViewBag.TotalPriceMain = totalPriceMain;
                    ViewBag.Cart = cart;
                    ViewBag.Account = Account;
                    return View(model);
                }
            }
            
            return RedirectToAction("Index", "Home");

        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Index( CheckoutVM checkout )
        {

            var AccountIdClaim = User.FindFirst("AccountId")?.Value;

            if (!string.IsNullOrEmpty(AccountIdClaim) && int.TryParse(AccountIdClaim, out int AccountId))
            {
                var Account = _context.Accounts.FirstOrDefault(c => c.AccountId == AccountId);
                if (Account == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var sessionCart = HttpContext.Session.GetString("Cart");
                var cart = JsonConvert.DeserializeObject<List<CartItemVM>>(sessionCart);

                if (ModelState.IsValid)
                {
                    Account.FullName = checkout.FullName;
                    Account.Phone = checkout.PhoneNumber;
                    Account.LocationId = checkout.LocationIdCity;
                    _context.Update(Account);


                    var city = _context.Locations.Where(l => l.LocationId == checkout.LocationIdCity).FirstOrDefault().NameWithType;
                    var district = _context.Locations.Where(l => l.LocationId == checkout.LocationIdDistrict).FirstOrDefault().NameWithType;
                    var ward = _context.Locations.Where(l => l.LocationId == checkout.LocationIdWard).FirstOrDefault().NameWithType;

                    var order = new Order()
                    {
                        AccountId = Account.AccountId,
                        TransactStatusId = 1,
                        Deleted = false,
                        Paid = false,
                        Address = checkout.Address,
                        City = city,
                        District = district,
                        Ward = ward,
                        Note = checkout.Note,
                        TotalMoney = Convert.ToInt32(cart.Sum(c => c.TotalMoney)),
                        TotalDiscount = Convert.ToInt32(cart.Sum(c => c.TotalDiscount)),
                        OrderNumber = cart.Sum(c => c.Amount),
                        PaymentId = checkout.PaymentId
                    };
                   
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    foreach (var item in cart)
                    {
                        var orderDetail = new OrderDetail()
                        {
                            OrderId = order.OrderId,
                            VariantDetailId = item.VariantDetailId,
                            Color = item.Color.ColorName,
                            Size = item.Size,
                            Quantity = item.Amount,
                            Discount = item.Product.Discount,
                            Total = item.Product.Price
                        };
                        _context.OrderDetails.Add(orderDetail);

                        var variantDetail = _context.VariantDetails
                            .FirstOrDefault(v => v.VariantDetailId == item.VariantDetailId);

                        if (variantDetail != null)
                        {
                            variantDetail.Quantity -= item.Amount;
                             _context.VariantDetails.Update(variantDetail);
                        }

                    }
                    await _context.SaveChangesAsync();

                    HttpContext.Session.Remove("Cart");


                    if (checkout.PaymentId == 1)
                    {
                        return RedirectToAction("Order", "Accounts");
                    }
                    else if (checkout.PaymentId == 2)
                    {
                        return RedirectToAction("Order", "Accounts");
                    };
                }
                else
                {
                    int totalPriceBase = 0;
                    int totalDiscount = 0;
                    int totalPriceMain = 0;
                    foreach (var item in cart)
                    {
                        totalPriceBase += item.Amount * item.Product.Price;
                        totalDiscount += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * (item.Product.Price - item.Product.Discount.Value) : 0;
                        totalPriceMain += (item.Product.Discount.HasValue && item.Product.Discount > 0) ? item.Amount * item.Product.Discount.Value : item.Amount * item.Product.Price;
                    };
                    var model = new CheckoutVM();

                    model.Payments = _context.Payments.Where(p => p.Allowed == true).ToList();

                    ViewData["Citys"] = new SelectList(_context.Locations.Where(l => l.Levels == null).ToList(), "LocationId", "Name");
                    ViewBag.TotalPriceBase = totalPriceBase;
                    ViewBag.TotalDiscount = totalDiscount;
                    ViewBag.TotalPriceMain = totalPriceMain;
                    ViewBag.Cart = cart;
                    ViewBag.Account = Account;
                    return View(model);
                }
            }
            return View(RedirectToAction("Index","Home"));
        }
        [HttpPost]
        public IActionResult Confirm( )
        {
            // Xác nhận thông tin đặt hàng và thanh toán
            if (ModelState.IsValid)
            {
                // Xử lý thanh toán
                return RedirectToAction("Complete");
            }
            return View();
        }
        public IActionResult Complete()
        {
            return View();
        }
    }
}
