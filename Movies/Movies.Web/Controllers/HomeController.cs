using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Movies.Web.Models;
using Movies.Web.Data; // DbContext namespace'i
using Microsoft.Extensions.Logging;

namespace Movies.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        // DbContext ve logger'ı constructor'a ekle
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

    public IActionResult Index()
    {
        try
        {
            // Veritabanından istatistikleri al
            var totalMovies = _db.Movies.Count();
            var totalActors = _db.Actors.Count();
            var totalMovieActors = _db.MovieActors.Count();

            // ViewBag'e atıyoruz
            ViewBag.TotalMovies = totalMovies;
            ViewBag.TotalActors = totalActors;
            ViewBag.TotalMovieActors = totalMovieActors;

            // Logger ile loglama (dynamic yerine int kullandık)
            _logger.LogInformation(
                "Home Index sayfası görüntülendi. ToplamFilmler={TotalMovies}, ToplamOyuncular={TotalActors}, Toplamİlişkilendirmeler={TotalMovieActors}",
                totalMovies, totalActors, totalMovieActors
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Home Index sayfası yüklenirken hata oluştu.");
            TempData["Error"] = "İstatistikler yüklenirken bir hata oluştu.";
        }

        return View();
    }


        public IActionResult Privacy()
        {
            _logger.LogInformation("Home Privacy sayfası görüntülendi.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogWarning("Home Error sayfası görüntülendi. RequestId={RequestId}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
