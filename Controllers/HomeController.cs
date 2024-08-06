using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TShop.Models;
using TShop.Services;
using TShop.ViewModels;

namespace TShop.Controllers;

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

    public IActionResult Index()
    {
        var categorys = _productService.GetProductsByCategorys(new string[] { "nam", "nu" }, 10);
        var ao_phong = _productService.GetProductsByTypeCategorys(null, new string[] { "ao-phong" }, 10);
        var ao_polo = _productService.GetProductsByTypeCategorys(null, new string[] { "ao-polo" }, 10);
        var quan_shorts = _productService.GetProductsByTypeCategorys(null, new string[] { "quan-sooc" }, 10);
        var vay = _productService.GetProductsByTypeCategorys(null, new string[] { "vay" }, 10);

        var blogs = _context.Blogs.OrderByDescending(b => b.DateCreated).Take(3).ToList();


        var homeVMs = new HomeVM
        {
            HotDeal = _productService.GetHotDeal(10),
            BestSeller = _productService.GetbestSeller(10),
            Categories = categorys,
            AoPhong = ao_phong,
            AoPolo = ao_polo,
            QuanShorts = quan_shorts,
            Vay = vay,
            Blogs = blogs,
        };
        return View(homeVMs);
    }
    [Route("about")]

    public IActionResult About()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }
    [Route("404")]
    public IActionResult Page404()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
