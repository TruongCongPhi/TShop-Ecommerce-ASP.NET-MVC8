using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TShop.Models;
using TShop.ViewModels;
using System.Linq;
using TShop.Helpers;
using TShop.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Drawing.Printing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly TshopContext _context;
        private readonly ILogger<ProductController> _logger;
        private readonly ProductHelper _productHelper;
        private readonly ProductService _productService;

        public ProductController(TshopContext context, ILogger<ProductController> logger, ProductHelper productHelper, ProductService productService)
        {
            _context = context;
            _logger = logger;
            _productHelper = productHelper;
            _productService = productService;
        }
        public IActionResult Index(int? typeCategory)
        {

            return View();
        }
        [Route("chi-tiet/{alias}")]
        public IActionResult Detail(string? alias)
        {
            var product1 = _context.Products.Include(p => p.Category).Where(p => p.Alias == alias).FirstOrDefault();
            var query = _productHelper.GetProductQuery();

            query = query.Take(8).Where(q => q.Alias != alias && q.Category.Alias == product1.Category.Alias).OrderByDescending(q => q.DateCreated);
            var recommend = _productHelper.GetProducts(query);

            var product = _context.Products
                    .Where(p => p.Alias == alias)
                    .Select(p => new ProductDetailVM
                    {
                        Product = p,
                        Variants = p.Variants.Select(v => new VariantVM
                        {
                            VariantId = v.VariantId,
                            ColorId = v.ColorId ?? 0,
                            ColorName = v.Color.ColorName,
                            ColorHex = v.Color.ColorHex,
                            Thumb = v.Thumb,
                            productImages = v.ProductImages.Select(i => new ProductImage
                            {
                                ImageId = i.ImageId,
                                ImageUrl = i.ImageUrl,
                                Caption = i.Caption,
                            }).ToList(),
                            VariantDetails = v.VariantDetails.Select(vd => new VariantDetailVM
                            {
                                VariantDetailId = vd.VariantDetailId,
                                SizeId = vd.SizeId ?? 0,
                                SizeName = vd.Size.SizeName,
                                Quantity = vd.Quantity??0
                            }).ToList()
                        }).ToList(),
                        ProductCommentVM = p.ProductComments.Select(c => new ProductCommentVM                    
                        {
                            CommentId = c.CommentId,
                            Content = c.Content,
                            UserName = c.Account.FullName,
                            CreateAt = c.CreateAt
                        }).ToList(),
                        RecommendedProducts = recommend,
                    })                                                     
                    .FirstOrDefault();
            if (product == null)
            {

                TempData["Message"] = $"Không tìm thấy sản phẩm {alias}";
                return Redirect("/404");
            }

            var typeCate = _context.Products.Where(p => p.Alias == alias).Select(p => p.Category).FirstOrDefault();
            ViewBag.TypeCategory = typeCate;
            ViewBag.Category = _productService.GetCategoryFirstParent(typeCate);
            

            return View(product);
        }
        [Route("danh-muc/{category}")]
        public IActionResult ProductCategory(string? category)
        {
            var products = _productService.GetProductsByTypeCategorys(category, new string[] { "ao-phong","ao-polo", "quan-sooc","vay", }, 0);

            if (products == null || products.Count == 0)
            {
                TempData["Message"] = $"Không tìm thấy danh mục {category}";
                return Redirect("/404");
            }
            ViewBag.CategoryName = _context.Categories.Where(c => c.Alias == category).FirstOrDefault().CategoryName;
            ViewBag.CategoryAlias = category;

            return View(products);
        }

        [Route("kieu-danh-muc/{category}/{typecategory}")]
        public IActionResult ProductTypeCategoryDetail(string category, string typecategory, int page = 1)
        {
            int pageSize = Utilities.PAGE_SIZE;
            var catParent = _context.Categories
                                          .Where(c => c.Alias == category)
                                          .FirstOrDefault();

            var typeCat = _productService.GetCategoryLastChild(catParent, typecategory);

            if (typeCat == null)
            {
                TempData["Message"] = $"Không tìm thấy kiểu danh mục {typecategory}";
                return Redirect("/404");
            }

            var productTypeCatVM = _productService.GetProductsByTypeCategorys(category, new string[] { typecategory }, 0);

            if (productTypeCatVM == null)
            {
                TempData["Message"] = $"Không tìm thấy danh mục {category}";
                return Redirect("/404");
            }

            var products = productTypeCatVM.Select(c => c.ProductTypeCats).First();

            ViewBag.TotalCount = products.Count;

            products = products.Take(pageSize).Skip((page - 1) * pageSize).ToList();

            ViewBag.TypeCat = typeCat;
            ViewBag.Category = catParent;
            ViewBag.PageCurrent = page;
            ViewBag.PageSize = pageSize;

            return View(products);
        }

        [HttpPost]
        public IActionResult LoadMoreProducts(int page, string? alias, int categoryId, string sortOption, List<int> sizeIds, List<int> colorIds, int priceMin, int priceMax)
        {
            int pageSize = Utilities.PAGE_SIZE;
            _logger.LogInformation("page: " + page);
            try
            {
                var query = _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Variants)
                                    .ThenInclude(v => v.Color)
                                .Include(p => p.Variants)
                                    .ThenInclude(v => v.VariantDetails)
                                        .ThenInclude(vd => vd.Size)
                                .AsQueryable();
                if (!string.IsNullOrEmpty(alias))
                {
                    _logger.LogInformation(alias);
                    query = _productService.CategoryFilterAlias(query, alias);
                }
                else
                {
                    query = _productService.CategoryFilter(query, categoryId);
                }

                if (sizeIds.Any()) { query = _productService.SizesFilter(query, sizeIds); }

                if (colorIds.Any()) { query = _productService.ColorsFilter(query, colorIds); }

                if (sortOption != "") { query = _productService.SortFilter(query, sortOption); }
          
                query = _productService.PriceFilter(query, priceMin, priceMax);

                query = query.Skip((page - 1) * pageSize).Take(pageSize).AsQueryable();

                var products = _productHelper.GetProducts(query).ToList();


                return PartialView("ProductItem", products);
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        public IActionResult LoadMoreProductsCount(int page,string? alias, int categoryId, string sortOption, List<int> sizeIds, List<int> colorIds, int priceMin, int priceMax)
        {
            int pageSize = Utilities.PAGE_SIZE;
            try
            {
                var query = _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Variants)
                                    .ThenInclude(v => v.Color)
                                .Include(p => p.Variants)
                                    .ThenInclude(v => v.VariantDetails)
                                        .ThenInclude(vd => vd.Size)
                                .AsQueryable();
                if (!string.IsNullOrEmpty(alias))
                {
                    query = _productService.CategoryFilterAlias(query, alias);
                }
                else
                {
                    query = _productService.CategoryFilter(query, categoryId);
                }

                if (sizeIds.Any()) query = _productService.SizesFilter(query, sizeIds);

                if (colorIds.Any()) query = _productService.ColorsFilter(query, colorIds);

                if (sortOption != "") query = _productService.SortFilter(query, sortOption);

                query = _productService.PriceFilter(query, priceMin, priceMax);

                var countAll = _productHelper.GetProducts(query).Count();

                _logger.LogInformation("tổng sp" + countAll);

                query = query.Skip((page - 1) * pageSize).Take(pageSize).AsQueryable();

                var countFiler = _productHelper.GetProducts(query).ToList().Count();

                var countNow = (page - 1) * pageSize + countFiler;


                return Json(new { countAll = countAll, countSelect = countFiler, countNow = countNow });
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("kieu-danh-mục/{typecategory}")]
        public IActionResult ProductTypeCategory(string typecategory, string? cat)
        {
            try
            {
                var typeCats = new List<ProductTypeCategoryVM>();
                if (cat == "")
                {
                    typeCats = _productService.GetProductsByTypeCategorys(null, new string[] { typecategory }, Utilities.PAGE_SIZE);
                }
                else typeCats = _productService.GetProductsByTypeCategorys(cat, new string[] { typecategory }, Utilities.PAGE_SIZE);

                ViewBag.TotalCount = _productService.GetProductsByTypeCategorys(cat, new string[] { typecategory },0).FirstOrDefault().ProductTypeCats.Count;


                if (typeCats.Count == 0)
                {
                return Redirect("/404");

                }

                return View(typeCats);
            }
            catch(Exception e)
            {
                _logger.LogInformation("lỗi:" + e.Message);
                return Redirect("/404");
            }
        }

        [HttpPost("product/detail/comment")]
        public IActionResult Comment(string content, int productId, string alias)
        {
            if (User.Identity.IsAuthenticated)
            {
                var AccountIdClaim = User.FindFirst("AccountId")?.Value;
                if (!string.IsNullOrEmpty(AccountIdClaim) && int.TryParse(AccountIdClaim, out int AccountId))
                {
                    var Account = _context.Accounts.FirstOrDefault(c => c.AccountId == AccountId);

                    if (Account == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    var comment = new ProductComment
                    {
                        AccountId = Account.AccountId,
                        ProductId = productId,
                        Content = content
                    };
                    _context.ProductComments.Add(comment);
                    _context.SaveChanges();
                    return RedirectToAction("Detail", "Product", new { alias = alias });

                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }

            }
            return RedirectToAction("Index", "Home");

        }

        [HttpGet("Product/SearchProducts")]
        public IActionResult SearchProducts(string query)
        {
            _logger.LogInformation("Tìm kiếm: " + query);
            if (string.IsNullOrEmpty(query))
            {
                return Json(new List<ProductVM>());
            }

            var products = _context.Products
                .Where(p => p.ProductName.Contains(query) || p.ProductCode.Contains(query))
                .Include(p => p.Variants)
                .Select(p => new ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductCode = p.ProductCode,
                    Alias = p.Alias,
                    Variants = p.Variants.Select(v => new VariantVM
                    {
                        Thumb = v.Thumb
                    }).ToList()
                }).ToList();

            return PartialView("_ProductItemSearchPartial", products);
        }

    }
}
