using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TShop.Models;
using TShop.Services;

namespace TShop.Areas.Admin.Controllers
{
    [Area("admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TshopContext _context;
        private readonly ProductService _productService;

        public HomeController(ILogger<HomeController> logger, TshopContext context, ProductService productService)
        {
            _logger = logger;
            _context = context;
            _productService = productService;
        }
        public async Task< IActionResult> Index()
        {
            var orders = _context.Orders.ToList().Count();
            var products = _context.Products.ToList().Count();
            var blogs = _context.Blogs.ToList().Count();
            var accounts = _context.Accounts.ToList().Count();

            ViewBag.Orders = orders;
            ViewBag.Products = products;
            ViewBag.Blogs = blogs;
            ViewBag.Accounts = accounts;

            var blogList = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.Account).OrderByDescending(b => b.DateCreated);

            return View(await blogList.ToListAsync());
        }
    }
}
