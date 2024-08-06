using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using TShop.Helpers;
using TShop.Models;

namespace TShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogsController : Controller
    {

        private readonly TshopContext _context;
        private readonly ILogger<BlogsController> _logger;
        public INotyfService _notifyService { get; }
        public BlogsController(ILogger<BlogsController> logger, TshopContext context, INotyfService notifyService)
        {
            _logger = logger;
            _context = context;
            _notifyService = notifyService;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.Account);
            return View(await blogs.ToListAsync());
        }

        [HttpGet("admin/blogs/create")]
        public IActionResult Create()
        {
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategories, "BlogCategoryId", "CategoryName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (ModelState.IsValid)
            {
                var AccountIdClaim = Convert.ToInt32( User.FindFirst("AccountId")?.Value);
                var thumb = "";
                if (Request.Form.Files != null)
                {
                    var image = Request.Form.Files["thumb"];
                    thumb = await Utilities.SaveImageAsync(image, "blog");
                }
                blog.Thumb = thumb;
                blog.AccountId = AccountIdClaim;
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        [HttpPost("/admin/blogs/deleteblog")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã xóa" });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategories, "BlogCategoryId", "CategoryName");

            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Title,Content,Author,Published,IsHot,IsNewFeed,BlogCategoryId,DateCreated,Thumb")] Blog blog)
        {
            if (id != blog.BlogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var AccountIdClaim = Convert.ToInt32(User.FindFirst("AccountId")?.Value);

                    if (Request.Form.Files.Any())
                    {
                        var image = Request.Form.Files["image"];
                        if (image != null && image.Length > 0)
                        {
                            var thumb = await Utilities.SaveImageAsync(image, "blog");
                            blog.Thumb = thumb;
                        }
                    }
                    blog.AccountId = AccountIdClaim;
                   
                    blog.DateModified = DateTime.Now;
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.BlogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }
        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }

        [HttpPost("admin/blogs/search")]
        public IActionResult SearchAccounts(string query)
        {
            IQueryable<Blog> blogQuery = _context.Blogs
                .Include(p => p.BlogCategory)
                .Include(p => p.Account);

            if (!string.IsNullOrEmpty(query))
            {
                var searchQuery = query.ToLower();
                blogQuery = blogQuery.Where(p => p.Title.ToLower().Contains(searchQuery) ||
                                                         p.BlogCategory.CategoryName.ToLower().Contains(searchQuery) ||
                                                           p.Account.FullName.ToLower().Contains(searchQuery));

            }

            var blogs = blogQuery.ToList();
            return PartialView("_BlogListPartial", blogs); // Create a partial view for product list rendering
        }
    }
}
