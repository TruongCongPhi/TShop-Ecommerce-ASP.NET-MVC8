using TShop.Helpers;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Services
{
    public class ProductService
    {
        private readonly TshopContext _context;
        private readonly ProductHelper _productHelper;

        public ProductService(TshopContext context, ProductHelper productHelper)
        {
            _context = context;
            _productHelper = productHelper;
        }

        public List<ProductCategoryVM> GetProductsByCategorys(string[] categoryAlias, int count)
        {

            var catIdParents = _context.Categories
                                     .Where(c => categoryAlias.Contains(c.Alias))   
                                     .ToList();

            var categoryVMs = new List<ProductCategoryVM>();

            foreach (var item in catIdParents)
            {
                var catIdLv2 = _context.Categories
                           .Where(c => c.ParentId == item.CategoryId)
                           .Select(c => c.CategoryId)
                           .ToList();

                var query = _productHelper.GetProductQuery();

                query = query.Where(p => catIdLv2.Contains(p.Category.ParentId ?? 0))
                    .Take(count);

                var products = _productHelper.GetProducts(query);


                categoryVMs.Add(new ProductCategoryVM
                {
                    Category = item,
                    ProductCategorys = products
                });
            }

            return categoryVMs;
        }
        public List<ProductTypeCategoryVM> GetProductsByTypeCategorys(string? categoryAlias, string[] typeCategoryAliases, int count)
        {
            var typeCategoryVMs = new List<ProductTypeCategoryVM>();

            List<int> catIdLv2 = null;

            if (categoryAlias != null)
            {
                var catIdParent = _context.Categories
                                          .Where(c => c.Alias == categoryAlias)
                                          .Select(c => c.CategoryId)
                                          .FirstOrDefault();
                if(catIdParent == 0)
                {
                    return null;
                }

                catIdLv2 = _context.Categories
                                   .Where(c => c.ParentId == catIdParent)
                                   .Select(c => c.CategoryId)
                                   .ToList();
                foreach (var alias in typeCategoryAliases)
                {
                    var typeCat = _context.Categories.Where(t => t.Alias == alias && catIdLv2.Contains(t.ParentId.Value)).FirstOrDefault();

                    if (typeCat == null) continue;

                    var query = _productHelper.GetProductQuery()
                                          .Where(p => p.Category.Alias == alias);

                    if (catIdLv2 != null)
                    {
                        query = query.Where(p => catIdLv2.Contains(p.Category.ParentId ?? 0));
                    }

                    if (count > 0)
                    {
                        query = query.Take(count);
                    }

                    var products = _productHelper.GetProducts(query).ToList();

                    typeCategoryVMs.Add(new ProductTypeCategoryVM
                    {
                        TypeCategory = typeCat,
                        ProductTypeCats = products
                    });


                }
            }
            else
            {
                foreach (var alias in typeCategoryAliases)
                {
                    var typeCat = _context.Categories.Where(t => t.Alias == alias ).FirstOrDefault();

                    if (typeCat == null) continue;

                    var query = _productHelper.GetProductQuery()
                                          .Where(p => p.Category.Alias == alias);

                    if (count > 0)
                    {
                        query = query.Take(count);
                    }

                    var products = _productHelper.GetProducts(query).ToList();

                    typeCategoryVMs.Add(new ProductTypeCategoryVM
                    {
                        TypeCategory = typeCat,
                        ProductTypeCats = products
                    });


                }
            }

            return typeCategoryVMs;
        }


        public IQueryable<Product> CategoryFilterAlias(IQueryable<Product> query, string alias)
        {
            return query = query.Where(p => p.Category.Alias == alias);
        }
        public IQueryable<Product> CategoryFilter(IQueryable<Product> query, int categoryId)
        {
            return query = query.Where(p => p.Category.CategoryId == categoryId);
        }
        public IQueryable<Product> SizesFilter(IQueryable<Product> query, List<int> sizeIds)
        {
            return query = query.Where(p => p.Variants.Any(v => v.VariantDetails.Any(vd => sizeIds.Contains(vd.SizeId ?? 0) && vd.Quantity > 0 )));
        }
        public IQueryable<Product> ColorsFilter(IQueryable<Product> query, List<int> colorIds)
        {
            return query = query.Where(p => p.Variants.Any(v => colorIds.Contains(v.Color.ParentId??0)));
        }
        public IQueryable<Product> PriceFilter(IQueryable<Product> query, int priceMin, int priceMax)
        {
            return query = query.Where(p => (p.Discount.HasValue && p.Discount.Value >= priceMin && p.Discount.Value <= priceMax) ||
        (!p.Discount.HasValue && p.Price >= priceMin && p.Price <= priceMax)
    );
        }

        public List<ProductVM> GetHotDeal(int count)
        {

                var query = _productHelper.GetProductQuery();

                query = query.Where(p => p.HotDeal == true)
                    .Take(count);
                var products = _productHelper.GetProducts(query);


            return products;
        }

        public List<ProductVM> GetbestSeller(int count)
        {

            var query = _productHelper.GetProductQuery();

            query = query.Where(p => p.BestSellers == true)
                .Take(count);
            var products = _productHelper.GetProducts(query);


            return products;
        }
        public IQueryable<Product> SortFilter(IQueryable<Product> query, string sortOption)
        {
             switch (sortOption)
            {
                case "new":
                    query = query.OrderByDescending(p => p.DateCreated);
                    break;
                case "asc":
                    query = query.OrderBy(p => p.Discount.HasValue ? p.Discount.Value : p.Price);
                    break;
                case "desc":
                    query = query.OrderByDescending(p => p.Discount.HasValue ? p.Discount.Value : p.Price);
                    break;
            }
            return query;
   
        }

        public Category GetCategoryFirstParent(Category cat)
        {
            var category = _context.Categories
                                   .Where(c => c.CategoryId == cat.ParentId)       
                                   .FirstOrDefault();

            if (category == null || category.ParentId == null)
            {
                return category;
            }

            return GetCategoryFirstParent(category);
        }

        public Category GetCategoryLastChild(Category cat, string AliasChild)
        {
            var category2 = _context.Categories
                                   .Where(c => c.ParentId == cat.CategoryId)
                                   .Select(c => c.CategoryId)
                                   .ToList();

            var category3 = _context.Categories.Where(c => category2.Contains(c.ParentId.Value) && c.Alias == AliasChild).FirstOrDefault();
            
            return category3 ;
        }

    }
}

