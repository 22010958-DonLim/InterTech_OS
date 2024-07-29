using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using StrawberryHub.Services;
using Telegram.Bot.Types;

namespace StrawberryHub.Controllers
{
    public class StrawberryLikesController : Controller
    {
        private readonly AppDbContext _context;

        public StrawberryLikesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: StrawberryLikes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.StrawberryLike.Include(s => s.StrawberryArticle).Include(s => s.StrawberryUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: StrawberryLikes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryLike == null)
            {
                return NotFound();
            }

            var strawberryLike = await _context.StrawberryLike
                .Include(s => s.StrawberryArticle)
                .Include(s => s.StrawberryUser)
                .FirstOrDefaultAsync(m => m.LikeId == id);
            if (strawberryLike == null)
            {
                return NotFound();
            }

            return View(strawberryLike);
        }

        // GET: StrawberryLikes/Create
        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId");
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username");
            return View();
        }

        // POST: StrawberryLikes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LikeId,UserId,ArticleId,Likes,LikeDateTime")] StrawberryLike strawberryLike)
        {
            if (ModelState.IsValid)
            {
                var existingLike = await _context.StrawberryLike
        .FirstOrDefaultAsync(l => l.ArticleId == strawberryLike.ArticleId && l.UserId == strawberryLike.UserId);

                if (existingLike == null)
                {
                    var currentDate = DateTime.Now;
                    var startOfDay = currentDate.Date;
                    var likeTaskId = 3; // Assuming 3 is the ID for "Liking Article" task
                    var hasLikedToday = await _context.StrawberryUserTask
                        .AnyAsync(s => s.UserId == strawberryLike.UserId &&
                                       s.TaskId == likeTaskId &&
                                       s.CompletedDate.HasValue &&
                                       s.CompletedDate.Value.Date == startOfDay);

                    if (!hasLikedToday)
                    {
                        // Record the task completion in StrawberryUserTask
                        var task = await _context.StrawberryTask.FindAsync(likeTaskId);
                        if (task != null)
                        {
                            var newUserTask = new StrawberryUserTask
                            {
                                TaskId = likeTaskId,
                                UserId = strawberryLike.UserId,
                                Points = task.PointsReward,
                                CompletedDate = currentDate
                            };
                            _context.StrawberryUserTask.Add(newUserTask);

                            // Update the user's points in StrawberryUser table
                            var user = await _context.StrawberryUser.FindAsync(strawberryLike.UserId);
                            if (user != null)
                            {
                                user.Points += task.PointsReward;
                                // Check if the user has enough points to rank up
                                var nextRank = _context.StrawberryRank
                                    .Where(r => r.MinPoints <= user.Points && r.MaxPoints >= user.Points)
                                    .OrderBy(r => r.RankId)
                                    .FirstOrDefault();

                                if (nextRank != null && nextRank.RankId > user.RankId)
                                {
                                    // Update the user's rank
                                    user.RankId = nextRank.RankId;
                                }

                                _context.Entry(user).State = EntityState.Modified;
                            }
                        }
                    }

                    // User hasn't liked any article today, so create a new like record
                    _context.Add(strawberryLike);
                    await _context.SaveChangesAsync();
                } else
                {
                    // User has already liked the article, so we'll remove the like (dislike)
                    _context.StrawberryLike.Remove(existingLike);
                    await _context.SaveChangesAsync();
                }

                // Save changes for both StrawberryLike and StrawberryUserTask
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } 
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryLike.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryLike.UserId);
            return View(strawberryLike);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLike([FromBody] StrawberryLike strawberryLike)
        {
            if (ModelState.IsValid)
            {
                var existingLike = await _context.StrawberryLike
                    .FirstOrDefaultAsync(l => l.ArticleId == strawberryLike.ArticleId && l.UserId == strawberryLike.UserId);

                if (existingLike == null)
                {
                    var currentDate = DateTime.Now;
                    var startOfDay = currentDate.Date;
                    var likeTaskId = 3; // Assuming 3 is the ID for "Liking Article" task

                    var hasLikedToday = await _context.StrawberryUserTask
                        .AnyAsync(s => s.UserId == strawberryLike.UserId &&
                                       s.TaskId == likeTaskId &&
                                       s.CompletedDate.HasValue &&
                                       s.CompletedDate.Value.Date == startOfDay);

                    if (!hasLikedToday)
                    {
                        // Record the task completion in StrawberryUserTask
                        var task = await _context.StrawberryTask.FindAsync(likeTaskId);
                        if (task != null)
                        {
                            var newUserTask = new StrawberryUserTask
                            {
                                TaskId = likeTaskId,
                                UserId = strawberryLike.UserId,
                                Points = task.PointsReward,
                                CompletedDate = currentDate
                            };
                            _context.StrawberryUserTask.Add(newUserTask);

                            // Update the user's points in StrawberryUser table
                            var user = await _context.StrawberryUser.FindAsync(strawberryLike.UserId);
                            if (user != null)
                            {
                                user.Points += task.PointsReward;
                                _context.Entry(user).State = EntityState.Modified;
                            }
                        }
                    }

                    // User hasn't liked this article, so create a new like record
                    strawberryLike.Likes = 1;
                    strawberryLike.LikeDateTime = DateTime.Now;
                    _context.Add(strawberryLike);
                    await _context.SaveChangesAsync();

                    return Ok(new { success = true, message = "Like added successfully", action = "added" });
                }
                else
                {
                    // User has already liked the article, so we'll remove the like (dislike)
                    _context.StrawberryLike.Remove(existingLike);
                    await _context.SaveChangesAsync();

                    return Ok(new { success = true, message = "Like removed successfully", action = "removed" });
                }
            }

            return BadRequest(ModelState);
        }

        // GET: StrawberryLikes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryLike == null)
            {
                return NotFound();
            }

            var strawberryLike = await _context.StrawberryLike.FindAsync(id);
            if (strawberryLike == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryLike.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryLike.UserId);
            return View(strawberryLike);
        }

        // POST: StrawberryLikes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LikeId,UserId,ArticleId,Likes")] StrawberryLike strawberryLike)
        {
            if (id != strawberryLike.LikeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(strawberryLike);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StrawberryLikeExists(strawberryLike.LikeId))
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
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryLike.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryLike.UserId);
            return View(strawberryLike);
        }

        // GET: StrawberryLikes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryLike == null)
            {
                return NotFound();
            }

            var strawberryLike = await _context.StrawberryLike
                .Include(s => s.StrawberryArticle)
                .Include(s => s.StrawberryUser)
                .FirstOrDefaultAsync(m => m.LikeId == id);
            if (strawberryLike == null)
            {
                return NotFound();
            }

            return View(strawberryLike);
        }

        // POST: StrawberryLikes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryLike == null)
            {
                return Problem("Entity set 'AppDbContext.StrawberryLike'  is null.");
            }
            var strawberryLike = await _context.StrawberryLike.FindAsync(id);
            if (strawberryLike != null)
            {
                _context.StrawberryLike.Remove(strawberryLike);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StrawberryLikeExists(int id)
        {
          return (_context.StrawberryLike?.Any(e => e.LikeId == id)).GetValueOrDefault();
        }
    }
}
