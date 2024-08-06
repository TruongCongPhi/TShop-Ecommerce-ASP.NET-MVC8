using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TShop.Areas.Admin.Controllers;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly TshopContext _context;
        private readonly ILogger<BlogController> _logger;
        public INotyfService _notifyService { get; }
        public BlogController(ILogger<BlogController> logger, TshopContext context, INotyfService notifyService)
        {
            _logger = logger;
            _context = context;
            _notifyService = notifyService;
        }
        [Route("blog")]
        public IActionResult Index()
        {
            var blogCategories = _context.BlogCategories
                .Include(bc => bc.Blogs)
                .ToList();

            var blogVMs = blogCategories.Select(bc => new BlogVM
            {
                CategoryName = bc.CategoryName,
                Blogs = bc.Blogs.OrderByDescending(b => b.DateCreated).ToList()
            }).ToList();

            return View(blogVMs);
        }
        [HttpGet("/blog-detail")]
        public IActionResult Detail(int id)
        {
            var blogs = _context.Blogs.Include(b => b.BlogCategory).Where(b => b.BlogId == id).FirstOrDefault();
            return View(blogs);
        }
    }
}
