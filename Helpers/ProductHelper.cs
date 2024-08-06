using Microsoft.EntityFrameworkCore;
using TShop.Controllers;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Helpers
{
    public class ProductHelper
    {
        private readonly TshopContext _context;

        public ProductHelper(TshopContext context) { _context = context; }

        public IQueryable<Product> GetProductQuery()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.VariantDetails)
                        .ThenInclude(vd => vd.Size)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.ProductImages)
                .AsQueryable();
        }

        public List<ProductVM> GetProducts(IQueryable<Product> query)
        {
            if( query == null)
            {
                 query = GetProductQuery();
            }

            var products = query.Select(p => new ProductVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Alias = p.Alias,
                Price = p.Price,
                Discount = p.Discount,
                HotDeal = p.HotDeal,
                BestSeller = p.BestSellers,
                CategoryName = p.Category.CategoryName,
                CategoryAlias = p.Category.Alias,
                Variants = p.Variants.Select(v => new VariantVM
                {
                    VariantId = v.VariantId,
                    ColorId = v.ColorId ?? 0,
                    ColorHex = v.Color.ColorHex,
                    Thumb = v.Thumb,
                    VariantDetails = v.VariantDetails.Select(vd => new VariantDetailVM
                    {
                        VariantDetailId = vd.VariantDetailId,
                        SizeId = vd.SizeId ?? 0,
                        SizeName = vd.Size.SizeName,
                        Quantity = vd.Quantity??0
                    }).ToList()
                }).ToList()
            }).ToList();
            return products;
         }
    }
}
