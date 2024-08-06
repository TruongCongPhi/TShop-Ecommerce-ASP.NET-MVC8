using Microsoft.AspNetCore.Mvc;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.ViewComponents
{
    public class SizesViewComponent : ViewComponent
    {
        private readonly TshopContext _context;

        public SizesViewComponent(TshopContext context) => _context = context;

        public IViewComponentResult Invoke(int typeCategoryId)
        {
            var sizes = _context.Products
                .Where(p => p.CategoryId == typeCategoryId)
                .SelectMany(p => p.Variants)
                .SelectMany(v => v.VariantDetails)
                .Where(vd => vd.Quantity > 0)
                .Select(vd => vd.Size)
                .Distinct()
                .ToList();

            return View(sizes);
        }
    }
}
