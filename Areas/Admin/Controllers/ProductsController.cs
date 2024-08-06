using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using TShop.Helpers;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly TshopContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly ProductHelper _productHelper;


        public INotyfService _notifyService { get; }
        public ProductsController(ILogger<ProductsController> logger, TshopContext context, INotyfService notifyService, ProductHelper productHelper)
        {
            _logger = logger;
            _context = context;
            _notifyService = notifyService;
            _productHelper = productHelper;
        }

        // GET: Admin/Products
        [HttpGet("admin/products")]
        public async Task<IActionResult> Index()
        {
            var tshopContext = _context.Products.Include(p => p.Category).Include(p => p.Variants);
            return View(await tshopContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        [HttpGet("admin/products/create")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.ParentId == null && c.Levels == null), "CategoryId", "CategoryName");
            ViewData["ColorId"] = new SelectList(_context.Colors.Where(cl => cl.ParentId == null && cl.Levels == null), "ColorId", "ColorName");
            ViewData["SizeId"] = new SelectList(_context.Sizes.Where(cl => cl.ParentId == null && cl.Levels == null), "SizeId", "SizeName");

            return View();
        }


        [HttpPost("/admin/products/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
               Product product,
               List<int> Colors,
               Dictionary<int, List<int>> Sizes,
               Dictionary<int, List<int>> Stocks)
        {
            _logger.LogInformation($"ProductName: {product.ProductName}");
            _logger.LogInformation($"Alias: {product.Alias}");

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.ParentId == null && c.Levels == null), "CategoryId", "CategoryName");
                ViewData["ColorId"] = new SelectList(_context.Colors.Where(cl => cl.ParentId == null && cl.Levels == null), "ColorId", "ColorName");
                ViewData["SizeId"] = new SelectList(_context.Sizes.Where(cl => cl.ParentId == null && cl.Levels == null), "SizeId", "SizeName");
                return View(product);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var productCode = await ProductCodeExists();
                product.ProductCode = productCode;
                product.Alias = Utilities.Slugify(product.ProductName) + "-" + productCode;
                _context.Add(product);
                await _context.SaveChangesAsync();

                int totalStock = 0;
                foreach (var color in Colors)
                {
                    _logger.LogInformation($"mau: {color}");
                    var mainImageKey = $"MainImages-{color}";
                    var mainImage = Request.Form.Files[mainImageKey];

                    var mainImagePath = await Utilities.SaveImageAsync(mainImage, "product");

                    var productVariant = new Variant
                    {
                        ColorId = color,
                        Thumb = mainImagePath,
                        ProductId = product.ProductId
                    };
                    _context.Variants.Add(productVariant);
                    await _context.SaveChangesAsync();

                    for (int i = 0; i < Sizes[color].Count; i++)
                    {
                        var VariantDetail = new VariantDetail
                        {
                            VariantId = productVariant.VariantId,
                            SizeId = Sizes[color][i],
                            Quantity = Stocks[color][i]
                        };
                        totalStock += Stocks[color][i];
                        _context.VariantDetails.Add(VariantDetail);
                        await _context.SaveChangesAsync();
                    }

                    if (Request.Form.Files != null)
                    {

                        var variantImageKey = $"VariantImages-{color}";
                        var variantImages = Request.Form.Files.Where(f => f.Name == variantImageKey).ToList();
                        foreach (var image in variantImages)
                        {
                            if (image != null && image.Length > 0)
                            {
                                var imagePath = await Utilities.SaveImageAsync(image, "product");
                                var productImage = new Models.ProductImage
                                {
                                    ImageUrl = imagePath,
                                    VariantId = productVariant.VariantId,
                                };
                                _context.ProductImages.Add(productImage);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                product.Stock = totalStock;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _notifyService.Success("Thêm sản phẩm thành công");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                await transaction.RollbackAsync();
                _notifyService.Error("Có lỗi xảy ra khi thêm sản phẩm. Vui lòng thử lại.");
                ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.ParentId == null && c.Levels == null), "CategoryId", "CategoryName");
                ViewData["ColorId"] = new SelectList(_context.Colors.Where(cl => cl.ParentId == null && cl.Levels == null), "ColorId", "ColorName");
                ViewData["SizeId"] = new SelectList(_context.Sizes.Where(cl => cl.ParentId == null && cl.Levels == null), "SizeId", "SizeName");
                return View(product);
            }
        }
        // GET: Product/GetSubCategories/5
        [HttpGet("/admin/product/GetCategories/{categoryId}")]
        public async Task<IActionResult> GetCategories(int categoryId, int level)
        {
            _logger.LogInformation("danh muc id" + categoryId + "Lv" + level);
            var categories = await _context.Categories
                                              .Where(c => c.Levels == level && c.ParentId == categoryId)
                                              .Select(c => new { c.CategoryId, c.CategoryName })
                                              .ToListAsync();
            return Json(categories);
        }
        [HttpPost("admin/product/addvariant")]
        public IActionResult AddVariant([FromBody] VariantData data)
        {
            try
            {
                var selectedColors = data.SelectedColors.Select(int.Parse).ToList();
                var selectedSizes = data.SelectedSizes.Select(int.Parse).ToList();

                var colors = _context.Colors
                .Where(c => selectedColors.Contains(c.ColorId))
                .ToList();

                var sizes = _context.Sizes
                    .Where(s => selectedSizes.Contains(s.SizeId))
                    .ToList();

                var model = new VariantViewModel
                {
                    Colors = colors,
                    Sizes = sizes,
                };

                _notifyService.Success("Tạo biến thể thành công");
                return PartialView("_AddVariant", model);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        public class VariantViewModel
        {
            public List<Models.Color> Colors { get; set; }
            public List<Models.Size> Sizes { get; set; }

            public List<Models.Variant>? Variants { get; set; }

        }
        public class VariantData
        {
            public List<string> SelectedColors { get; set; }
            public List<string> SelectedSizes { get; set; }
        }
        [HttpGet("/admin/product/GetColors")]
        public async Task<IActionResult> GetColors([FromQuery] List<int?> colorIds)
        {

            _logger.LogInformation("màu" + colorIds.Count);

            var colors = await _context.Colors
                .Where(c => colorIds.Contains(c.ParentId))
                .Select(c => new { c.ColorId, c.ColorName, c.ColorHex })
                .ToListAsync();
            return Json(colors);
        }
        [HttpPost("admin/product/addcolor")]
        public async Task<IActionResult> AddColor([FromBody] Models.Color color)
        {
            var nameCheck = _context.Colors.Where(c => c.ColorName == color.ColorName).FirstOrDefault();
            if (nameCheck != null)
            {
                return Json(new
                {
                    success = false,
                    message = "Tên màu đã tồn tại",
                });
            }

            _context.Add(color);
            await _context.SaveChangesAsync();
            _notifyService.Success("Thêm màu mới thành công");

            return Json(new
            {
                success = true,
                message = "Thêm màu: " + color.ColorName + "(" + color.ColorHex + ") thành công"
            });
        }

        [HttpGet("/admin/product/GetSizes/{sizeGroupId}")]
        public async Task<IActionResult> GetSizes(int? sizeGroupId)
        {
            var sizes = await _context.Sizes.Where(c => c.ParentId == sizeGroupId).Select(c => new { c.SizeId, c.SizeName }).ToListAsync();
            return Json(sizes);
        }

        [HttpPost("admin/product/addsize")]
        public async Task<IActionResult> AddSize([FromBody] Models.Size size)
        {
            var nameCheck = _context.Sizes.Where(c => c.SizeName == size.SizeName).FirstOrDefault();
            if (nameCheck != null)
            {
                return Json(new
                {
                    success = false,
                    message = "Tên size đã tồn tại",
                });
            }

            _context.Add(size);
            await _context.SaveChangesAsync();
            return Json(new
            {
                success = true,
                message = "Thêm size: " + size.SizeName + " thành công"
            });
        }
        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var query = _productHelper.GetProductQuery().Where(p => p.ProductId == id);
            var variantQuery = query.SelectMany(p => p.Variants).Distinct().ToList();
            var variants = new VariantViewModel
            {
                Variants = variantQuery
            };

            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.CategoryId == product.CategoryId), "CategoryId", "CategoryName");
            ViewData["CategoryParentId"] = new SelectList(_context.Categories.Where(c => c.ParentId == null && c.Levels == null), "CategoryId", "CategoryName");
            ViewData["ColorId"] = new SelectList(_context.Colors.Where(cl => cl.ParentId == null && cl.Levels == null), "ColorId", "ColorName");
            ViewData["SizeId"] = new SelectList(_context.Sizes.Where(cl => cl.ParentId == null && cl.Levels == null), "SizeId", "SizeName");
            ViewData["Variants"] = variants;


            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product,
    List<int> Colors,
    Dictionary<int, List<int>> Sizes,
    Dictionary<int, List<int>> Stocks)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Fetch the existing product from the database
                    var existingProduct = await _context.Products
                        .Include(p => p.Variants)
                        .ThenInclude(v => v.VariantDetails)
                        .Include(p => p.Variants)
                        .ThenInclude(v => v.ProductImages)
                        .FirstOrDefaultAsync(p => p.ProductId == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Update product properties
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.Description = product.Description;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.Price = product.Price;
                    existingProduct.Discount = product.Discount;
                    existingProduct.Tags = product.Tags;
                    existingProduct.Title = product.Title;
                    existingProduct.Active = product.Active;
                    existingProduct.HotDeal = product.HotDeal;
                    existingProduct.BestSellers = product.BestSellers;
                    existingProduct.HomeFlag = product.HomeFlag;
                    existingProduct.DateModified = DateTime.Now;

                    // Handle the variants
                    int totalStock = 0;
                    bool hasErrors = false;
                    // Remove existing variants
                    _context.Variants.RemoveRange(existingProduct.Variants);

                    foreach (var color in Colors)
                    {
                        var mainImageKey = $"MainImages-{color}";
                        var mainImage = Request.Form.Files[mainImageKey];
                        var variantImageKey = $"VariantImages-{color}";
                        var variantImages = Request.Form.Files.Where(f => f.Name == variantImageKey).ToList();

                        // Check if there's an existing variant for this color
                        var existingVariant = existingProduct.Variants.FirstOrDefault(v => v.ColorId == color);

                        if (existingVariant == null)
                        {
                            if (mainImage == null || mainImage.Length == 0)
                            {
                                ModelState.AddModelError(mainImageKey, "Vui lòng chọn ảnh đại diện cho sản phẩm.");
                                hasErrors = true;
                                continue; // Bỏ qua việc xử lý biến thể này và tiếp tục với biến thể tiếp theo
                            }
                            
                            var mainImagePath = await Utilities.SaveImageAsync(mainImage, "product");

                            var productVariant = new Variant
                            {
                                ColorId = color,
                                Thumb = mainImagePath,
                                ProductId = product.ProductId
                            };
                            _context.Variants.Add(productVariant);
                            await _context.SaveChangesAsync();

                            for (int i = 0; i < Sizes[color].Count; i++)
                            {
                                var VariantDetail = new VariantDetail
                                {
                                    VariantId = productVariant.VariantId,
                                    SizeId = Sizes[color][i],
                                    Quantity = Stocks[color][i]
                                };
                                totalStock += Stocks[color][i];
                                _context.VariantDetails.Add(VariantDetail);
                                await _context.SaveChangesAsync();
                            }

                        }
                        else 
                        {
                            var mainImagePath = existingVariant.Thumb;
                            if (mainImage != null)
                            {
                                mainImagePath = await Utilities.SaveImageAsync(mainImage, "product");
                            }

                            existingVariant.ColorId = color;
                            existingVariant.Thumb = mainImagePath;
                            _context.Variants.Update(existingVariant);

                            var variantDetailCount = existingVariant.VariantDetails.Count();
                            var index = 0;
                            foreach(var variantDetail in existingVariant.VariantDetails)
                            {
                                variantDetail.SizeId = Sizes[color][index];
                                variantDetail.Quantity = Stocks[color][index];
                                _context.VariantDetails.Update(variantDetail);
                                totalStock += Stocks[color][index];

                                index++;
                            }

                            foreach (var image in existingVariant.ProductImages)
                            {
                                _context.ProductImages.Update(image);
                            }
                        }

                        if (variantImages.Count > 0)
                        {

                            foreach (var image in variantImages)
                            {
                                if (image != null && image.Length > 0)
                                {
                                    var imagePath = await Utilities.SaveImageAsync(image, "product");
                                    var productImage = new Models.ProductImage
                                    {
                                        ImageUrl = imagePath,
                                        VariantId = existingVariant?.VariantId ?? 0,
                                    };
                                    _context.ProductImages.Add(productImage);
                                }
                            }
                        }
                    }
                  
                    if (hasErrors)
                    {
                        var query = _productHelper.GetProductQuery().Where(p => p.ProductId == id);
                        var variantQuery = query.SelectMany(p => p.Variants).Distinct().ToList();
                        var variants = new VariantViewModel
                        {
                            Variants = variantQuery
                        };
                        ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.CategoryId == product.CategoryId), "CategoryId", "CategoryName");
                        ViewData["CategoryParentId"] = new SelectList(_context.Categories.Where(c => c.ParentId == null && c.Levels == null), "CategoryId", "CategoryName");
                        ViewData["ColorId"] = new SelectList(_context.Colors.Where(cl => cl.ParentId == null && cl.Levels == null), "ColorId", "ColorName");
                        ViewData["SizeId"] = new SelectList(_context.Sizes.Where(cl => cl.ParentId == null && cl.Levels == null), "SizeId", "SizeName");
                        ViewData["Variants"] = variants;
                    }

                    existingProduct.Stock = totalStock;
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    _notifyService.Success("Cập nhật sản phẩm thành công");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product");
                    await transaction.RollbackAsync();
                    _notifyService.Error("Có lỗi xảy ra khi cập nhật sản phẩm. Vui lòng thử lại.");
                }
            }
            
            return View(product);
        }



        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost("admin/products/search")]
        public IActionResult SearchProducts(string query)
        {
            IQueryable<Product> productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants);

            if (!string.IsNullOrEmpty(query))
            {
                var searchQuery = query.ToLower();
                productsQuery = productsQuery.Where(p => p.ProductName.ToLower().Contains(searchQuery) ||
                                                         p.ProductCode.ToLower().Contains(searchQuery) ||
                                                         p.Category.CategoryName.ToLower().Contains(searchQuery));
            }

            var products = productsQuery.ToList();
            return PartialView("_ProductListPartial", products); // Create a partial view for product list rendering
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        private async Task<string> ProductCodeExists()
        {
            string productCode;
            bool exists;

            do
            {
                productCode = Utilities.GenerateProductCode();
                exists = await _context.Products.AnyAsync(p => p.ProductCode == productCode);
            } while (exists);

            return productCode;
        }
    }
}
