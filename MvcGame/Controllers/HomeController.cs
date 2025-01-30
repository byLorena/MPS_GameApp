using Microsoft.AspNetCore.Mvc;
using MvcGame.Models;
using System.Diagnostics;
using MvcGame.Data;
using System.Linq;

namespace MvcGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MvcGameContext _context;

        public HomeController(ILogger<HomeController> logger, MvcGameContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            var recentGames = _context.Game
               .OrderByDescending(g => g.ReleaseDate) // Sortam după data lansării
               .Take(4) // Limitam la 4 jocuri
               .ToList();

            return View(recentGames);
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