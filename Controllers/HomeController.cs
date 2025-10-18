using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NgocHiepCeilingFans.Models;

namespace NgocHiepCeilingFans.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopQuatTranDbContext _context;

        public HomeController(ILogger<HomeController> logger, ShopQuatTranDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        { 
            var featuredProducts = _context.SanPhams
                .OrderByDescending(p => p.NgayTao)
                .Take(8)
                .ToList();

            return View(featuredProducts);
        }
        public IActionResult LienHe()
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
}