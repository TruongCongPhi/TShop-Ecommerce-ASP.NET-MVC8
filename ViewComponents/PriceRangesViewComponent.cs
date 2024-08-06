using Microsoft.AspNetCore.Mvc;
using TShop.Models;

namespace TShop.ViewComponents
{
    public class PriceRangesViewComponent : ViewComponent
    {
        private readonly TshopContext _context;
        public PriceRangesViewComponent(TshopContext context) { _context = context; }
        public IViewComponentResult Invoke(int typeCategoryId)
        {

            var priceRange = _context.Products
                .Where(p => p.CategoryId == typeCategoryId)
                .GroupBy(p => 1)
                .Select(g => new { MinPrice = g.Min(p => p.Discount?? p.Price), MaxPrice = g.Max(p => p.Discount?? p.Price) })
                .FirstOrDefault();

            if( priceRange == null)
            {
                
                var minPrice = 0;
                var maxPrice = 0;
                return View((minPrice, maxPrice));

            }
            else
            {
                var minPrice = Convert.ToInt32(priceRange.MinPrice);
                var maxPrice = Convert.ToInt32(priceRange.MaxPrice);
                return View((minPrice, maxPrice));
            }



        }
    }
}
