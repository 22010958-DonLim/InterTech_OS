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
    public class LikeCommentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDbService _dbService;

        public LikeCommentsController(AppDbContext context, IDbService dbService)
        {
            _context = context;
            _dbService = dbService; 
        }

        // GET: StrawberryLikeComments
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.StrawberryLikeComment.Include(s => s.StrawberryArticle).Include(s => s.StrawberryUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: StrawberryLikeComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryLikeComment == null)
            {
                return NotFound();
            }

            var strawberryLikeComment = await _context.StrawberryLikeComment
                .Include(s => s.StrawberryArticle)
                .Include(s => s.StrawberryUser)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (strawberryLikeComment == null)
            {
                return NotFound();
            }

            return View(strawberryLikeComment);
        }

        // GET: StrawberryLikeComments/Create
        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId");
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "UserId");
            return View();
        }

        // POST: StrawberryLikeComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,UserId,ArticleId,CommentText,Likes,CommentTimestamp,LikeTimestamp")] StrawberryLikeComment strawberryLikeComment)
        {
            // Ensure CommentId is hidden
            ModelState.Remove("CommentTimeStamp");
            ModelState.Remove("LikeTimeStamp");
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.Name);
                int userId = 0;
                if (userIdClaim != null)
                {
                    string username = userIdClaim.Value; // Extract the value of the claim
                    userId = await _context.StrawberryUser
                        .Where(u => u.Username == username)
                        .Select(u => u.UserId)
                        .FirstOrDefaultAsync();

                    // Now userId contains the UserId of the user with the specified username
                }

                strawberryLikeComment.UserId = userId;

                // Set LikeTimestamp if Likes is 1
                if (strawberryLikeComment.Likes == 1)
                {
                    strawberryLikeComment.LikeTimestamp = DateTime.Now;
                }
                else
                {
                    strawberryLikeComment.LikeTimestamp = null;
                }

                // Set CommentTimestamp if CommentText is not null or empty
                if (!string.IsNullOrEmpty(strawberryLikeComment.CommentText))
                {
                    strawberryLikeComment.CommentTimestamp = DateTime.Now;
                }
                else
                {
                    strawberryLikeComment.CommentTimestamp = null;
                }


                _context.Add(strawberryLikeComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryLikeComment.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "UserId", strawberryLikeComment.UserId);
            return View(strawberryLikeComment);
        }

        // GET: StrawberryLikeComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryLikeComment == null)
            {
                return NotFound();
            }

            var strawberryLikeComment = await _context.StrawberryLikeComment.FindAsync(id);
            if (strawberryLikeComment == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryLikeComment.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "UserId", strawberryLikeComment.UserId);
            return View(strawberryLikeComment);
        }

        // POST: StrawberryLikeComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,UserId,ArticleId,CommentText,Likes,CommentTimestamp,LikeTimestamp")] StrawberryLikeComment strawberryLikeComment)
        {
            if (id != strawberryLikeComment.CommentId)
            {
                return NotFound();
            }

            ModelState.Remove("CommentId");
            ModelState.Remove("CommentTimeStamp");
            ModelState.Remove("LikeTimeStamp");
            if (ModelState.IsValid)
            {
                // Set LikeTimestamp if Likes is 1
                if (strawberryLikeComment.Likes == 1)
                {
                    strawberryLikeComment.LikeTimestamp = DateTime.Now;
                }
                else
                {
                    strawberryLikeComment.LikeTimestamp = null;
                }

                // Set CommentTimestamp if CommentText is not null or empty
                if (!string.IsNullOrEmpty(strawberryLikeComment.CommentText))
                {
                    strawberryLikeComment.CommentTimestamp = DateTime.Now;
                }
                else
                {
                    strawberryLikeComment.CommentTimestamp = null;
                }

                try
                {
                    _context.Update(strawberryLikeComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StrawberryLikeCommentExists(strawberryLikeComment.CommentId))
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

            ViewData["ArticleId"] = new SelectList(_context.StrawberryArticle, "ArticleId", "ArticleId", strawberryLikeComment.ArticleId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "UserId", strawberryLikeComment.UserId);
            return View(strawberryLikeComment);
        }


        // GET: StrawberryLikeComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryLikeComment == null)
            {
                return NotFound();
            }

            var strawberryLikeComment = await _context.StrawberryLikeComment.Include(s => s.StrawberryArticle).Include(s => s.StrawberryUser)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (strawberryLikeComment == null)
            {
                return NotFound();
            }

            return View(strawberryLikeComment);
        }

        // POST: StrawberryLikeComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryLikeComment == null)
            {
                return Problem("Entity set 'AppDbContext.StrawberryLikeComment'  is null.");
            }
            var strawberryLikeComment = await _context.StrawberryLikeComment.FindAsync(id);
            if (strawberryLikeComment != null)
            {
                _context.StrawberryLikeComment.Remove(strawberryLikeComment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StrawberryLikeCommentExists(int id)
        {
          return (_context.StrawberryLikeComment?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
