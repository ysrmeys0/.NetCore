using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Web.Data;
using Movies.Web.ViewModels;
using Microsoft.Extensions.Logging;

namespace Movies.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ReportController> _logger;

        public ReportController(ApplicationDbContext db, ILogger<ReportController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                // Filmlere göre oyuncular
                var moviesGrouped = _db.Movies
                    .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                    .Select(m => new MovieGroup
                    {
                        FilmAdi = m.Ad,
                        Tur = m.Tur,
                        Yil = m.Yil,
                        Oyuncular = m.MovieActors
                            .Select(ma => new ActorDto 
                            { 
                                AdSoyad = ma.Actor.Ad + " " + ma.Actor.Soyad, 
                                Rol = ma.Rol 
                            }).ToList()
                    }).ToList();

                _logger.LogInformation("ReportController: MoviesGrouped verisi yüklendi. Toplam film sayısı={Count}", moviesGrouped.Count);

                // Oyunculara göre filmler
                var actorsGrouped = _db.Actors
                    .Include(a => a.MovieActors)
                    .ThenInclude(ma => ma.Movie)
                    .Select(a => new ActorGroup
                    {
                        ActorAdiSoyadi = a.Ad + " " + a.Soyad,
                        Filmler = a.MovieActors
                            .Select(ma => new MovieDto
                            {
                                Ad = ma.Movie.Ad,
                                Tur = ma.Movie.Tur,
                                Yil = ma.Movie.Yil,
                                Rol = ma.Rol
                            }).ToList()
                    }).ToList();

                _logger.LogInformation("ReportController: ActorsGrouped verisi yüklendi. Toplam oyuncu sayısı={Count}", actorsGrouped.Count);

                var model = new ReportViewModel
                {
                    MoviesGrouped = moviesGrouped,
                    ActorsGrouped = actorsGrouped
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportController Index aksiyonu çalışırken hata oluştu.");
                TempData["Error"] = "Raporlar yüklenirken bir hata oluştu.";
                return View(new ReportViewModel
                {
                    MoviesGrouped = new List<MovieGroup>(),
                    ActorsGrouped = new List<ActorGroup>()
                });
            }
        }
    }
}
