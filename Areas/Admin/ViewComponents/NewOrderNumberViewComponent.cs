using Microsoft.AspNetCore.Mvc;
using TShop.Models;

namespace TShop.Areas.Admin.ViewComponents
{
    
    public class NewOrderNumberViewComponent : ViewComponent
    {
        private readonly TshopContext _context;
        private readonly ILogger<NewOrderNumberViewComponent> _logger;

        public NewOrderNumberViewComponent(TshopContext context, ILogger<NewOrderNumberViewComponent> logger) { _context = context; _logger = logger; }
        public IViewComponentResult Invoke()
        {
            var number = _context.Orders.Where(o => o.TransactStatus.Status == "pending").ToList().Count();
            return View(number);
        }
    }
}
