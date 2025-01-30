using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcGame.Data;
using MvcGame.Models;

namespace MvcGame.Controllers
{
    public class GamesController : Controller
    {
        private readonly MvcGameContext _context;

        public GamesController(MvcGameContext context)
        {
            _context = context;
        }

        public IActionResult Collection()
        {
            ViewData["Title"] = "Games";
            return View();
        }

        // GET: Games
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "ReleaseDate" ? "date_desc" : "ReleaseDate";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var games = from s in _context.Game
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                games = games.Where(s => s.Title.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "title_desc":
                    games = games.OrderByDescending(s => s.Title);
                    break;
                case "ReleaseDate":
                    games = games.OrderBy(s => s.ReleaseDate);
                    break;
                case "date_desc":
                    games = games.OrderByDescending(s => s.ReleaseDate);
                    break;
                case "Genre":
                    games = games.OrderBy(s => s.Genre);
                    break;
                case "genre_desc":
                    games = games.OrderByDescending(s => s.Genre);
                    break;
                case "Price":
                    games = games.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    games = games.OrderByDescending(s => s.Price);
                    break;
                default:
                    games = games.OrderBy(s => s.Title);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Game>.CreateAsync(games.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            var reviews = await _context.Review.Where(review => review.GameId == game.Id).ToListAsync();
            game.Reviews = reviews;

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Games/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
     [Bind("Id,Title,ReleaseDate,Genre,Price, AddImage")] Game game,
     IFormFile imageFile) // Adaugam IFormFile ca parametru
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generam un nume unic pentru imagine
                var fileName = $"game_{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/games", fileName);

                // Salvam fișierul pe server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Salvam calea relativă în baza de date
                game.AddImage = $"/images/games/{fileName}";
            }

            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Games/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
    [Bind("Id,Title,ReleaseDate,Genre,Price,AddImage")] Game game,
    IFormFile imageFile) // Adaugam IFormFile
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                // Ștergem vechea imagine
                if (!string.IsNullOrEmpty(game.AddImage))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", game.AddImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Salvam noua imagine
                var fileName = $"game_{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/games", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                game.AddImage = $"/images/games/{fileName}";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // POST: AddReview
        [HttpPost]
        public IActionResult AddReview(int gameId, string username, string message)
        {
            var game = _context.Game.Include(g => g.Reviews).FirstOrDefault(g => g.Id == gameId);
            if (game == null)
            {
                return NotFound();
            }

            var review = new Review
            {
                User = username,
                ReviewMessage = message,
                PublishDate = DateTime.Now,
                GameId = gameId
            };

            game.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = gameId });
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Game == null)
            {
                return Problem("Entity set 'MvcGameContext.Game' is null.");
            }
            var game = await _context.Game.FindAsync(id);
            if (game != null)
            {
                _context.Game.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
          return (_context.Game?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // POST: Games/DeleteReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _context.Review.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            // Salvăm gameId pentru redirecționare
            var gameId = review.GameId;

            _context.Review.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = gameId });
        }
    }
}
