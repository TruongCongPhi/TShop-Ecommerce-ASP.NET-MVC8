using Microsoft.AspNetCore.Mvc;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.ViewComponents
{
    public class TypeCategoriesViewComponent : ViewComponent
    {
        private readonly TshopContext _context;

        public TypeCategoriesViewComponent(TshopContext context) => _context = context;

        public IViewComponentResult Invoke(int categoryId)
        {
            var catIdLv2 = _context.Categories.Where(c => c.ParentId == categoryId).Select(c => c.CategoryId).ToList();
            var categoryAlias = _context.Categories.Where(c => c.CategoryId == categoryId).Select(c => c.Alias).FirstOrDefault();


            //lấy ra các kiểu danh mục phải thuộc danh mục
            var data = _context.Categories
                .Where(c => catIdLv2.Contains(c.ParentId??0))
                .Select(tc => new TypeCategoryVM
                {
                    TypeCategoryId = tc.CategoryId,
                    TypeCategoryName = tc.CategoryName,
                    Alias = tc.Alias,
                    CategoryAlias = categoryAlias,
                    Count = tc.Products.Count(p => p.CategoryId == tc.CategoryId)
                })
                .ToList();

            return View(data);
        }

    }
}
