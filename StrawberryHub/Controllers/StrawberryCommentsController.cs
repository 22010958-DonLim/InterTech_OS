using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using StrawberryHub.Services;

namespace StrawberryHub.Controllers
{
    public class StrawberryCommentsController : Controller
    {
        private readonly AppDbContext _context;

        public StrawberryCommentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: StrawberryComments
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.StrawberryComment.Include(s => s.StrawberryArticle).Include(s => s.StrawberryUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: StrawberryComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryComment == null)
            {
                return NotFound();
            }

            var strawberryComment = await _context.StrawberryComment
                .Include(s => s.StrawberryArticle)
                .Include(s => s.StrawberryUser)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (strawberryComment == null)
            {
                return NotFound();
            }

            return View(strawberryComment);
        }

        // GET: StrawberryComments/Create
        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId");
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username");
            return View();
        }

        // POST: StrawberryComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("CommentId,UserId,ArticleId,CommentText,CommentDateTime")] StrawberryComment strawberryComment)
        {
            if (ModelState.IsValid)
            {
                var currentDate = DateTime.Now;
                var startOfDay = currentDate.Date;
                var CommentTaskId = 4; // Assuming 4 is the ID for "Commenting Article" task
                var hasCommentToday = await _context.StrawberryUserTask
                    .AnyAsync(s => s.UserId == strawberryComment.UserId &&
                                   s.TaskId == CommentTaskId &&
                                   s.CompletedDate.HasValue &&
                                   s.CompletedDate.Value.Date == startOfDay);

                if (!hasCommentToday)
                {
                    // Record the task completion in StrawberryUserTask
                    var task = await _context.StrawberryTask.FindAsync(CommentTaskId);
                    if (task != null)
                    {
                        var newUserTask = new StrawberryUserTask
                        {
                            TaskId = CommentTaskId,
                            UserId = strawberryComment.UserId,
                            Points = task.PointsReward,
                            CompletedDate = currentDate
                        };
                        _context.StrawberryUserTask.Add(newUserTask);

                        // Update the user's points in StrawberryUser table
                        var user = await _context.StrawberryUser.FindAsync(strawberryComment.UserId);
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
                    _context.Add(strawberryComment);
                await _context.SaveChangesAsync();
                return View(strawberryComment);
            }
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryComment.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryComment.UserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> NewCommentCreate([FromBody] StrawberryComment strawberryComment)
        {
            if (ModelState.IsValid)
            {
                var currentDate = DateTime.Now;
                strawberryComment.CommentDateTime = currentDate;
                var startOfDay = currentDate.Date;
                var CommentTaskId = 4; // Assuming 4 is the ID for "Commenting Article" task

                var hasCommentToday = await _context.StrawberryUserTask
                    .AnyAsync(s => s.UserId == strawberryComment.UserId &&
                                   s.TaskId == CommentTaskId &&
                                   s.CompletedDate.HasValue &&
                                   s.CompletedDate.Value.Date == startOfDay);

                if (!hasCommentToday)
                {
                    // Record the task completion in StrawberryUserTask
                    var task = await _context.StrawberryTask.FindAsync(CommentTaskId);
                    if (task != null)
                    {
                        var newUserTask = new StrawberryUserTask
                        {
                            TaskId = CommentTaskId,
                            UserId = strawberryComment.UserId,
                            Points = task.PointsReward,
                            CompletedDate = currentDate
                        };
                        _context.StrawberryUserTask.Add(newUserTask);

                        // Update the user's points in StrawberryUser table
                        var user = await _context.StrawberryUser.FindAsync(strawberryComment.UserId);
                        if (user != null)
                        {
                            user.Points += task.PointsReward;
                            _context.Entry(user).State = EntityState.Modified;
                        }
                    }
                }

                _context.Add(strawberryComment);
                await _context.SaveChangesAsync();

                // Fetch the username for the response
                var username = await _context.StrawberryUser
                    .Where(u => u.UserId == strawberryComment.UserId)
                    .Select(u => u.Username)
                    .FirstOrDefaultAsync();

                return Json(new
                {
                    success = true,
                    username = username,
                    commentText = strawberryComment.CommentText,
                    commentDateTime = strawberryComment.CommentDateTime
                });
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        // GET: StrawberryComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryComment == null)
            {
                return NotFound();
            }

            var strawberryComment = await _context.StrawberryComment.FindAsync(id);
            if (strawberryComment == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryComment.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryComment.UserId);
            return View(strawberryComment);
        }

        // POST: StrawberryComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,UserId,ArticleId,CommentText,CommentDateTime")] StrawberryComment strawberryComment)
        {
            if (id != strawberryComment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(strawberryComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StrawberryCommentExists(strawberryComment.CommentId))
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
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryComment.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryComment.UserId);
            return View(strawberryComment);
        }

        // GET: StrawberryComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryComment == null)
            {
                return NotFound();
            }

            var strawberryComment = await _context.StrawberryComment
                .Include(s => s.StrawberryArticle)
                .Include(s => s.StrawberryUser)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (strawberryComment == null)
            {
                return NotFound();
            }

            return View(strawberryComment);
        }

        // POST: StrawberryComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryComment == null)
            {
                return Problem("Entity set 'AppDbContext.StrawberryComment'  is null.");
            }
            var strawberryComment = await _context.StrawberryComment.FindAsync(id);
            if (strawberryComment != null)
            {
                _context.StrawberryComment.Remove(strawberryComment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StrawberryCommentExists(int id)
        {
          return (_context.StrawberryComment?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
