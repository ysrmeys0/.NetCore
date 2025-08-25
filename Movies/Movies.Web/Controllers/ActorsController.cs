using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movies.Web.Data;
using Movies.Web.Models;
using Microsoft.Extensions.Logging;

namespace Movies.Web.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ActorsController> _logger;

        public ActorsController(ApplicationDbContext context, ILogger<ActorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Actors
        public IActionResult Index()
        {
            _logger.LogInformation("Actors Index sayfası görüntülendi.");
            var actors = _context.Actors.ToList();

            // Film seçimi için ViewBag
            ViewBag.Movies = new SelectList(_context.Movies.ToList(), "MovieId", "Ad");

            return View(actors);
        }

        // POST: Actors/AddToMovie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToMovie(int actorId, int movieId, string rol)
        {
            if (actorId == 0 || movieId == 0 || string.IsNullOrEmpty(rol))
            {
                _logger.LogWarning("Eksik veri ile AddToMovie çağrıldı: ActorId={ActorId}, MovieId={MovieId}, Rol={Rol}", actorId, movieId, rol);
                TempData["Error"] = "Lütfen tüm alanları doldurun.";
                return RedirectToAction(nameof(Index));
            }

            var exists = _context.MovieActors.Any(ma => ma.ActorId == actorId && ma.MovieId == movieId);
            if (exists)
            {
                _logger.LogWarning("Oyuncu zaten filme ekli: ActorId={ActorId}, MovieId={MovieId}", actorId, movieId);
                TempData["Error"] = "Bu oyuncu zaten bu filme ekli.";
                return RedirectToAction(nameof(Index));
            }

            var movieActor = new MovieActor
            {
                ActorId = actorId,
                MovieId = movieId,
                Rol = rol
            };

            _context.MovieActors.Add(movieActor);
            _context.SaveChanges();
            _logger.LogInformation("Oyuncu filme eklendi: ActorId={ActorId}, MovieId={MovieId}, Rol={Rol}", actorId, movieId, rol);

            TempData["Success"] = "Oyuncu filme başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Actor Create sayfası açıldı.");
            return View();
        }

        // POST: Actors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Actors.Add(actor);
                _context.SaveChanges();
                _logger.LogInformation("Yeni oyuncu eklendi: {Actor}", actor.Ad + " " + actor.Soyad);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Geçersiz model ile oyuncu ekleme denemesi: {Actor}", actor.Ad + " " + actor.Soyad);
            return View(actor);
        }

        // GET: Actors/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit sayfası çağrıldı ama ID null.");
                return NotFound();
            }

            var actor = _context.Actors.Find(id);
            if (actor == null)
            {
                _logger.LogWarning("Edit için oyuncu bulunamadı. ActorId={ActorId}", id);
                return NotFound();
            }

            _logger.LogInformation("Edit sayfası açıldı. ActorId={ActorId}", id);
            return View(actor);
        }

        // POST: Actors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Actor actor)
        {
            if (id != actor.ActorId)
            {
                _logger.LogWarning("Edit POST çağrıldı ama ID uyuşmuyor. RouteId={RouteId}, ActorId={ActorId}", id, actor.ActorId);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(actor);
                _context.SaveChanges();
                _logger.LogInformation("Oyuncu güncellendi: {Actor}", actor.Ad + " " + actor.Soyad);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Edit POST başarısız: {Actor}", actor.Ad + " " + actor.Soyad);
            return View(actor);
        }

        // GET: Actors/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete sayfası çağrıldı ama ID null.");
                return NotFound();
            }

            var actor = _context.Actors.Find(id);
            if (actor == null)
            {
                _logger.LogWarning("Silinecek oyuncu bulunamadı. ActorId={ActorId}", id);
                return NotFound();
            }

            _logger.LogInformation("Delete sayfası açıldı. ActorId={ActorId}", id);
            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
                _context.SaveChanges();
                _logger.LogInformation("Oyuncu silindi: {Actor}", actor.Ad + " " + actor.Soyad);
            }
            else
            {
                _logger.LogWarning("DeleteConfirmed çağrıldı ama oyuncu bulunamadı. ActorId={ActorId}", id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
