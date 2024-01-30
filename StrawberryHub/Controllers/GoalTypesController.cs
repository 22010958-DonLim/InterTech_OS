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
              return _context.StrawberryGoalType != null ? 
                          View(await _context.StrawberryGoalType.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.GoalType'  is null.");
        }

        // GET: GoalTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryGoalType == null)
            {
                return NotFound();
            }

            var goalType = await _context.StrawberryGoalType
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
        public async Task<IActionResult> Create([Bind("GoalTypeId,Type")] StrawberryGoalType goalType)
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
            if (id == null || _context.StrawberryGoalType == null)
            {
                return NotFound();
            }

            var goalType = await _context.StrawberryGoalType.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("GoalTypeId,Type")] StrawberryGoalType goalType)
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
            if (id == null || _context.StrawberryGoalType == null)
            {
                return NotFound();
            }

            var goalType = await _context.StrawberryGoalType
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
            if (_context.StrawberryGoalType == null)
            {
                return Problem("Entity set 'AppDbContext.GoalType'  is null.");
            }
            var goalType = await _context.StrawberryGoalType.FindAsync(id);
            if (goalType != null)
            {
                _context.StrawberryGoalType.Remove(goalType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoalTypeExists(int id)
        {
          return (_context.StrawberryGoalType?.Any(e => e.GoalTypeId == id)).GetValueOrDefault();
        }
    }
}
