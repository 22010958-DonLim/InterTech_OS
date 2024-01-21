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
    public class ReflectionsController : Controller
    {
        private readonly AppDbContext _context;

        public ReflectionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reflections
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Reflection.Include(r => r.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Reflections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reflection == null)
            {
                return NotFound();
            }

            var reflection = await _context.Reflection
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReflectionId == id);
            if (reflection == null)
            {
                return NotFound();
            }

            return View(reflection);
        }

        // GET: Reflections/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "UserId");
            return View();
        }

        // POST: Reflections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReflectionId,UserId,Date,Content")] Reflection reflection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reflection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "UserId", reflection.UserId);
            return View(reflection);
        }

        // GET: Reflections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reflection == null)
            {
                return NotFound();
            }

            var reflection = await _context.Reflection.FindAsync(id);
            if (reflection == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "UserId", reflection.UserId);
            return View(reflection);
        }

        // POST: Reflections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReflectionId,UserId,Date,Content")] Reflection reflection)
        {
            if (id != reflection.ReflectionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reflection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReflectionExists(reflection.ReflectionId))
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
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "UserId", reflection.UserId);
            return View(reflection);
        }

        // GET: Reflections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reflection == null)
            {
                return NotFound();
            }

            var reflection = await _context.Reflection
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReflectionId == id);
            if (reflection == null)
            {
                return NotFound();
            }

            return View(reflection);
        }

        // POST: Reflections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reflection == null)
            {
                return Problem("Entity set 'AppDbContext.Reflection'  is null.");
            }
            var reflection = await _context.Reflection.FindAsync(id);
            if (reflection != null)
            {
                _context.Reflection.Remove(reflection);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReflectionExists(int id)
        {
          return (_context.Reflection?.Any(e => e.ReflectionId == id)).GetValueOrDefault();
        }
    }
}
