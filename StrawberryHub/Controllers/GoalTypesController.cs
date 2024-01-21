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
    public class GoalTypesController : Controller
    {
        private readonly AppDbContext _context;

        public GoalTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: GoalTypes
        public async Task<IActionResult> Index()
        {
              return _context.GoalType != null ? 
                          View(await _context.GoalType.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.GoalType'  is null.");
        }

        // GET: GoalTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GoalType == null)
            {
                return NotFound();
            }

            var goalType = await _context.GoalType
                .FirstOrDefaultAsync(m => m.GoalTypeId == id);
            if (goalType == null)
            {
                return NotFound();
            }

            return View(goalType);
        }

        // GET: GoalTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GoalTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GoalTypeId,Type")] GoalType goalType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(goalType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(goalType);
        }

        // GET: GoalTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GoalType == null)
            {
                return NotFound();
            }

            var goalType = await _context.GoalType.FindAsync(id);
            if (goalType == null)
            {
                return NotFound();
            }
            return View(goalType);
        }

        // POST: GoalTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GoalTypeId,Type")] GoalType goalType)
        {
            if (id != goalType.GoalTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goalType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoalTypeExists(goalType.GoalTypeId))
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
            return View(goalType);
        }

        // GET: GoalTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GoalType == null)
            {
                return NotFound();
            }

            var goalType = await _context.GoalType
                .FirstOrDefaultAsync(m => m.GoalTypeId == id);
            if (goalType == null)
            {
                return NotFound();
            }

            return View(goalType);
        }

        // POST: GoalTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GoalType == null)
            {
                return Problem("Entity set 'AppDbContext.GoalType'  is null.");
            }
            var goalType = await _context.GoalType.FindAsync(id);
            if (goalType != null)
            {
                _context.GoalType.Remove(goalType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoalTypeExists(int id)
        {
          return (_context.GoalType?.Any(e => e.GoalTypeId == id)).GetValueOrDefault();
        }
    }
}
