using Microsoft.AspNetCore.Mvc;
using TShop.Models;

namespace TShop.ViewComponents
{
    public class ColorsViewComponent : ViewComponent
    {
        private readonly TshopContext _context;
        private readonly ILogger<ColorsViewComponent> _logger;

        public ColorsViewComponent(TshopContext context, ILogger<ColorsViewComponent> logger) { _context = context; _logger = logger; }
       public IViewComponentResult Invoke(int? typeCategoryId)
        {

            var colorGroups =  _context.Variants
                .Where(v => v.Color != null && v.Product.Category.CategoryId == typeCategoryId)
                .Select(v => v.Color)
                .Join(
                    _context.Colors,
                    variantColor => variantColor.ParentId,
                    parentColor => parentColor.ColorId,
                    (variantColor, parentColor) => parentColor
                )
                .Distinct()
                .ToList();
            _logger.LogInformation("mau "+ colorGroups.Count);
            return View(colorGroups);
        }
    }
}
