using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly IWebHostEnvironment _env;

        public ArticlesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var articles = _context.StrawberryArticle
                .Include(a => a.GoalType)
                .Include(a => a.StrawberryUser) // Assuming there's a property named User in StrawberryArticle representing the user who created the article
                .ToListAsync();
            return View(await articles);
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
                .Include(a => a.StrawberryUser)
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
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "Type");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,GoalTypeId,Title,ArticleContent,PublishedDate,Photo,Picture,UserId")] StrawberryArticle article, IFormFile photo)
        {
            ModelState.Remove("Picture");     // No Need to Validate "Picture" - derived from "Photo".
            ModelState.Remove("UserId");
            ModelState.Remove("PublishDate");
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

                    string picfilename = DoPhotoUpload(article.Photo);
                    article.Picture = picfilename.EscQuote();
                    article.UserId = userId; // Assign the retrieved user id to the article
                    article.PublishedDate = DateTime.Now;
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
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "Type", article.GoalTypeId);

            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,GoalTypeId,Title,ArticleContent,PublishedDate,Photo,Picture,UserId")] StrawberryArticle article, IFormFile photo)
        {
            ModelState.Remove("Photo");       // No Need to Validate "Photo"
            ModelState.Remove("UserId");
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null)
                    {
                        string picfilename = DoPhotoUpload(photo);
                        article.Picture = picfilename.EscQuote();
                    }
                    article.PublishedDate = DateTime.Now;
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
                string photoFile = article.Picture;
                string fullpath = Path.Combine(_env.WebRootPath, "photos/" + photoFile);
                System.IO.File.Delete(fullpath);

                _context.StrawberryArticle.Remove(article);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
          return (_context.StrawberryArticle?.Any(e => e.ArticleId == id)).GetValueOrDefault();
        }

        private string DoPhotoUpload(IFormFile photo)
        {
            string fext = Path.GetExtension(photo.FileName);
            string uname = Guid.NewGuid().ToString();
            string fname = uname + fext;
            string fullpath = Path.Combine(_env.WebRootPath, "photos/" + fname);
            using (FileStream fs = new(fullpath, FileMode.Create))
            {
                photo.CopyTo(fs);
            }
            return fname;
        }

        // GET: Articles/Recent
        public async Task<IActionResult> ShowArticle()
        {
            var articles = await _context.StrawberryArticle
                .OrderByDescending(a => a.PublishedDate)
                .Take(3)
                .ToListAsync();

            return View("ShowArticle", articles);
        }


        public async Task<IActionResult> ArticlePage()
        {

			var articles = _context.StrawberryArticle
				 .Include(a => a.GoalType)
				 .Include(a => a.StrawberryUser) // Assuming there's a property named User in StrawberryArticle representing the user who created the article
				 .ToListAsync();

			return View("ArticlePage", await articles);
		}
    }
}
