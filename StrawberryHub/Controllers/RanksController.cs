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
    public class RanksController : Controller
    {
        private readonly AppDbContext _context;

        public RanksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ranks
        public async Task<IActionResult> Index()
        {
              return _context.StrawberryRank != null ? 
                          View(await _context.StrawberryRank.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Rank'  is null.");
        }

        // GET: Ranks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryRank == null)
            {
                return NotFound();
            }

            var rank = await _context.StrawberryRank
                .FirstOrDefaultAsync(m => m.RankId == id);
            if (rank == null)
            {
                return NotFound();
            }

            return View(rank);
        }

        // GET: Ranks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ranks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RankId,RankName,MinPoints,MaxPoints")] StrawberryRank rank)
        {
            if (ModelState.IsValid)
            {
                // Check if a Rank with the same RankName already exists
                if (_context.StrawberryRank.Any(r => r.RankName == rank.RankName))
                {
                    ModelState.AddModelError("RankName", "Rank with this name already exists.");
                    return View(rank);
                }

                _context.Add(rank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rank);
        }

        // GET: Ranks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryRank == null)
            {
                return NotFound();
            }

            var rank = await _context.StrawberryRank.FindAsync(id);
            if (rank == null)
            {
                return NotFound();
            }
            return View(rank);
        }

        // POST: Ranks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RankId,RankName,MinPoints,MaxPoints")] StrawberryRank rank)
        {
            if (id != rank.RankId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RankExists(rank.RankId))
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
            return View(rank);
        }

        // GET: Ranks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryRank == null)
            {
                return NotFound();
            }

            var rank = await _context.StrawberryRank
                .FirstOrDefaultAsync(m => m.RankId == id);
            if (rank == null)
            {
                return NotFound();
            }

            return View(rank);
        }

        // POST: Ranks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryRank == null)
            {
                return Problem("Entity set 'AppDbContext.Rank'  is null.");
            }
            var rank = await _context.StrawberryRank.FindAsync(id);
            if (rank != null)
            {
                _context.StrawberryRank.Remove(rank);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RankExists(int id)
        {
          return (_context.StrawberryRank?.Any(e => e.RankId == id)).GetValueOrDefault();
        }
    }
}
