using Microsoft.AspNetCore.Mvc;
using TShop.Models;

namespace TShop.Controllers
{
    public class LocationsController : Controller
    {
        private TshopContext _context;
        public LocationsController(TshopContext context) {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetCity()
        {
            var citys = _context.Locations.Where(l => l.Levels == null).ToList();
            return Json(citys);
        }
        [HttpGet("api/location/district/{cityId}")]
        public async Task<IActionResult> GetDistrict(int cityId)
        {
            var districts = _context.Locations.Where(l => l.ParentCode == cityId && l.Levels == 1).ToList();
            return Json(districts);
        }
        [HttpGet("api/location/ward/{districtId}")]
        public async Task<IActionResult> GetWard(int districtId)
        {
            var wards = _context.Locations.Where(l => l.ParentCode == districtId && l.Levels == 2).ToList();
            return Json(wards);
        }


    }
}
