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
    public class ArticlesController : Controller
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.StrawberryArticle.Include(a => a.GoalType);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryArticle == null)
            {
                return NotFound();
            }

            var article = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "GoalTypeId");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,GoalTypeId,ArticleContent,PublishedDate")] StrawberryArticle article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "GoalTypeId", article.GoalTypeId);
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryArticle == null)
            {
                return NotFound();
            }

            var article = await _context.StrawberryArticle.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "GoalTypeId", article.GoalTypeId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,GoalTypeId,ArticleContent,PublishedDate")] StrawberryArticle article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
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
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "GoalTypeId", article.GoalTypeId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryArticle == null)
            {
                return NotFound();
            }

            var article = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryArticle == null)
            {
                return Problem("Entity set 'AppDbContext.Article'  is null.");
            }
            var article = await _context.StrawberryArticle.FindAsync(id);
            if (article != null)
            {
                _context.StrawberryArticle.Remove(article);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
          return (_context.StrawberryArticle?.Any(e => e.ArticleId == id)).GetValueOrDefault();
        }
    }
}
