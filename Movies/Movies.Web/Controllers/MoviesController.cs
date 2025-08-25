using Microsoft.AspNetCore.Mvc;
using Movies.Web.Data;
using Movies.Web.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Movies.Web.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(ApplicationDbContext context, ILogger<MoviesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Movies
        public IActionResult Index()
        {
            _logger.LogInformation("Movies Index sayfası görüntülendi.");
            var movies = _context.Movies.ToList();
            return View(movies);
        }

        // GET: Movies/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details sayfası çağrıldı ama ID null.");
                return NotFound();
            }

            var movie = _context.Movies
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .FirstOrDefault(m => m.MovieId == id);

            if (movie == null)
            {
                _logger.LogWarning("ID {MovieId} ile detay bulunamadı.", id);
                return NotFound();
            }

            ViewBag.Actors = _context.Actors
                .Select(a => new SelectListItem
                {
                    Value = a.ActorId.ToString(),
                    Text = a.Ad + " " + a.Soyad
                })
                .ToList();

            _logger.LogInformation("Details sayfası görüntülendi. MovieId={MovieId}", id);
            return View(movie);
        }

        // POST: Movies/AddActor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddActor(int movieId, int actorId, string rol)
        {
            if (movieId == 0 || actorId == 0 || string.IsNullOrEmpty(rol))
            {
                _logger.LogWarning("Eksik veri ile AddActor çağrıldı: MovieId={MovieId}, ActorId={ActorId}, Rol={Rol}", movieId, actorId, rol);
                TempData["Error"] = "Lütfen tüm alanları doldurun.";
                return RedirectToAction("Details", new { id = movieId });
            }

            var exists = _context.MovieActors.Any(ma => ma.MovieId == movieId && ma.ActorId == actorId);
            if (exists)
            {
                _logger.LogWarning("Oyuncu zaten filme ekli: MovieId={MovieId}, ActorId={ActorId}", movieId, actorId);
                TempData["Error"] = "Bu oyuncu zaten bu filme ekli.";
                return RedirectToAction("Details", new { id = movieId });
            }

            var movieActor = new MovieActor
            {
                MovieId = movieId,
                ActorId = actorId,
                Rol = rol
            };

            _context.MovieActors.Add(movieActor);
            _context.SaveChanges();
            _logger.LogInformation("Film-oyuncu ilişkisi eklendi: MovieId={MovieId}, ActorId={ActorId}, Rol={Rol}", movieId, actorId, rol);

            TempData["Success"] = "Oyuncu filme başarıyla eklendi.";
            return RedirectToAction("Details", new { id = movieId });
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Create sayfası açıldı.");
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Movies.Add(movie);
                _context.SaveChanges();
                _logger.LogInformation("Yeni film eklendi: {Movie} ({Year})", movie.Ad, movie.Yil);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Geçersiz model ile film ekleme denemesi: {Movie}", movie.Ad);
            return View(movie);
        }

        // GET: Movies/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit sayfası çağrıldı ama ID null.");
                return NotFound();
            }

            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                _logger.LogWarning("Edit için film bulunamadı. MovieId={MovieId}", id);
                return NotFound();
            }

            _logger.LogInformation("Edit sayfası açıldı. MovieId={MovieId}", id);
            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Movie movie)
        {
            if (id != movie.MovieId)
            {
                _logger.LogWarning("Edit POST çağrıldı ama ID uyuşmuyor. RouteId={RouteId}, MovieId={MovieId}", id, movie.MovieId);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(movie);
                _context.SaveChanges();
                _logger.LogInformation("Film güncellendi: {Movie} ({Year})", movie.Ad, movie.Yil);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Edit POST başarısız: {Movie}", movie.Ad);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete sayfası çağrıldı ama ID null.");
                return NotFound();
            }

            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                _logger.LogWarning("Silinecek film bulunamadı. MovieId={MovieId}", id);
                return NotFound();
            }

            _logger.LogInformation("Delete sayfası açıldı. MovieId={MovieId}", id);
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                _logger.LogInformation("Film silindi: {Movie}", movie.Ad);
            }
            else
            {
                _logger.LogWarning("DeleteConfirmed çağrıldı ama film bulunamadı. MovieId={MovieId}", id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
