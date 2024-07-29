using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using StrawberryHub.Models;
using StrawberryHub.Services;

namespace StrawberryHub.Controllers
{
    public class StrawberryFeedbacksController : Controller
    {
        private readonly AppDbContext _context;

        public StrawberryFeedbacksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: StrawberryFeedbacks
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.StrawberryFeedback.Include(s => s.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: StrawberryFeedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryFeedback == null)
            {
                return NotFound();
            }

            var strawberryFeedback = await _context.StrawberryFeedback
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (strawberryFeedback == null)
            {
                return NotFound();
            }

            return View(strawberryFeedback);
        }

        // GET: StrawberryFeedbacks/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username");
            return View();
        }

        // POST: StrawberryFeedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeedbackId,UserId,Stars,Message")] StrawberryFeedback strawberryFeedback)
        {
            if (ModelState.IsValid)
            {
                _context.Add(strawberryFeedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryFeedback.UserId);
            return View(strawberryFeedback);
        }

        // POST: StrawberryFeedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> CreateFeedback(int Stars, string Message)
        {
            var usernameClaim = HttpContext.User.FindFirst(ClaimTypes.Name);

            if (usernameClaim == null)
            {
                return BadRequest("Username claim not found");
            }

            var username = usernameClaim.Value;

            // Fetch the user from the database, including their preferences
            var user = _context.StrawberryUser.Include(u => u.Goal).FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var feedback = new StrawberryFeedback();
            feedback.UserId = user.UserId;
            feedback.Message = Message;
            feedback.Stars = Stars;
            
            if (ModelState.IsValid)
            {
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", feedback.UserId);
            return Json(new { success = false });
        }

        // GET: StrawberryFeedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryFeedback == null)
            {
                return NotFound();
            }

            var strawberryFeedback = await _context.StrawberryFeedback.FindAsync(id);
            if (strawberryFeedback == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryFeedback.UserId);
            return View(strawberryFeedback);
        }

        // POST: StrawberryFeedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackId,UserId,Stars,Message")] StrawberryFeedback strawberryFeedback)
        {
            if (id != strawberryFeedback.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(strawberryFeedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StrawberryFeedbackExists(strawberryFeedback.FeedbackId))
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
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryFeedback.UserId);
            return View(strawberryFeedback);
        }

        // GET: StrawberryFeedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryFeedback == null)
            {
                return NotFound();
            }

            var strawberryFeedback = await _context.StrawberryFeedback
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (strawberryFeedback == null)
            {
                return NotFound();
            }

            return View(strawberryFeedback);
        }

        // POST: StrawberryFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryFeedback == null)
            {
                return Problem("Entity set 'AppDbContext.StrawberryFeedback'  is null.");
            }
            var strawberryFeedback = await _context.StrawberryFeedback.FindAsync(id);
            if (strawberryFeedback != null)
            {
                _context.StrawberryFeedback.Remove(strawberryFeedback);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StrawberryFeedbackExists(int id)
        {
          return (_context.StrawberryFeedback?.Any(e => e.FeedbackId == id)).GetValueOrDefault();
        }
    }
}
