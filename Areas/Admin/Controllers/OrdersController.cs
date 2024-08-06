using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly TshopContext _context;
        private readonly ILogger<OrdersController> _logger;
        public INotyfService _notifyService { get; }
        public OrdersController(ILogger<OrdersController> logger, TshopContext context, INotyfService notifyService)
        {
            _logger = logger;
            _context = context;
            _notifyService = notifyService;
        }

        [HttpGet("admin/orders")]
        public IActionResult Index(string? status)
        {
            try
            {

                var orders = string.IsNullOrEmpty(status)
                   ? _context.Orders
                .Include(o => o.Account)
                .Include(o => o.Payment)
                .Include(o => o.TransactStatus)
                .OrderByDescending(c => c.OrderDate).ToList()
                : _context.Orders
                .Include(o => o.Account)
                .Include(o => o.Payment)
                .Include(o => o.TransactStatus)
                .Where(o => o.TransactStatus.Status == status)
                .OrderByDescending(c => c.OrderDate).ToList();

                return View(orders);
            }
            catch (Exception e)
            {
                _logger.LogInformation("lỗi", e.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("admin/orders/update/status")]
        public IActionResult UpdateStatus(string status, int orderId)
        {
            if(status == "delete")
            {
                var order = _context.Orders.Where(o => o.OrderId == orderId).FirstOrDefault();
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return Json(new { success = true, message = "Đã xóa" });
            }
            else {
            var order = _context.Orders.Where(o => o.OrderId == orderId).FirstOrDefault();

                var transactStatusId = _context.TransactStatuses.Where(t => t.Status == status).FirstOrDefault().TransactStatusId;

                order.TransactStatusId = transactStatusId;
                _context.SaveChanges();

                return Json(new { success = true, message = $"Đã cập nhật {status}" });
            }
        }


        [HttpGet("admin/orders/detail/{orderid:int}")]
        public IActionResult Detail(int orderid)
        {
            try
            {
                var order = _context.Orders
                .Include(c => c.Account)
                .Include(o => o.Payment)
                .Include(o => o.TransactStatus)
                .FirstOrDefault(o => o.OrderId == orderid);
                var orderDetail = _context.OrderDetails.Where(d => d.OrderId == orderid)
                    .ToList();

                if (orderDetail == null)
                {
                    TempData["Message"] = $"Không có hóa đơn";
                    return Redirect("/404");
                }

                var variantIds = orderDetail.Select(o => o.VariantDetailId).ToList();

                List<CartItemVM> cartItemVMs = new List<CartItemVM>();
                foreach (var item in orderDetail)
                {
                    var variantDetail = _context.VariantDetails
                         .Where(vd => vd.VariantDetailId == item.VariantDetailId)
                         .Select(vd => new CartItemVM
                         {
                             Product = vd.Variant.Product,
                             Amount = item.Quantity.Value,
                             Color = vd.Variant.Color,
                             Thumb = vd.Variant.Thumb,
                             Size = vd.Size.SizeName,
                         }).FirstOrDefault();
                    cartItemVMs.Add(variantDetail);

                }
                var orderDetailVM = new OrderDetailVM()
                {
                    Order = order,
                    FullName = order.Account?.FullName,
                    PhoneNumber = order.Account?.Phone,
                    Payment = order.Payment?.PaymentType,
                    CartItems = cartItemVMs.ToList(),
                };
                


                return View(orderDetailVM);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Lỗi " + e.Message);
                return Redirect("/404");
            }
        }
        [HttpGet("admin/orders/invoice/{orderid?}")]
        public IActionResult Invoice(int?orderid)
        {
            try
            {
                var order = _context.Orders
                .Include(c => c.Account)
                .Include(o => o.Payment)
                .Include(o => o.TransactStatus)
                .FirstOrDefault(o => o.OrderId == orderid);
                var orderDetail = _context.OrderDetails.Where(d => d.OrderId == orderid)
                    .ToList();

                if (orderDetail == null)
                {
                    TempData["Message"] = $"Không có hóa đơn";
                    return Redirect("/404");
                }

                var variantIds = orderDetail.Select(o => o.VariantDetailId).ToList();

                List<CartItemVM> cartItemVMs = new List<CartItemVM>();
                foreach (var item in orderDetail)
                {
                    var variantDetail = _context.VariantDetails
                         .Where(vd => vd.VariantDetailId == item.VariantDetailId)
                         .Select(vd => new CartItemVM
                         {
                             Product = vd.Variant.Product,
                             Amount = item.Quantity.Value,
                             Color = vd.Variant.Color,
                             Thumb = vd.Variant.Thumb,
                             Size = vd.Size.SizeName,
                         }).FirstOrDefault();
                    cartItemVMs.Add(variantDetail);

                }
                var orderDetailVM = new OrderDetailVM()
                {
                    Order = order,
                    FullName = order.Account?.FullName,
                    PhoneNumber = order.Account?.Phone,
                    Payment = order.Payment?.PaymentType,
                    CartItems = cartItemVMs.ToList(),
                };



                return View(orderDetailVM);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Lỗi " + e.Message);
                return Redirect("/404");
            }
        }
    }
}
