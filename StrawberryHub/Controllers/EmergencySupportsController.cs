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
    public class EmergencySupportsController : Controller
    {
        private readonly AppDbContext _context;

        public EmergencySupportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: EmergencySupports
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.StrawberryEmergencySupport.Include(e => e.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: EmergencySupports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryEmergencySupport == null)
            {
                return NotFound();
            }

            var emergencySupport = await _context.StrawberryEmergencySupport
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmergencySupportId == id);
            if (emergencySupport == null)
            {
                return NotFound();
            }

            return View(emergencySupport);
        }

        // GET: EmergencySupports/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "UserId");
            return View();
        }

        // POST: EmergencySupports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmergencySupportId,UserId,Timestamp,Message")] StrawberryEmergencySupport emergencySupport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emergencySupport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "UserId", emergencySupport.UserId);
            return View(emergencySupport);
        }

        // GET: EmergencySupports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryEmergencySupport == null)
            {
                return NotFound();
            }

            var emergencySupport = await _context.StrawberryEmergencySupport.FindAsync(id);
            if (emergencySupport == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "UserId", emergencySupport.UserId);
            return View(emergencySupport);
        }

        // POST: EmergencySupports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmergencySupportId,UserId,Timestamp,Message")] StrawberryEmergencySupport emergencySupport)
        {
            if (id != emergencySupport.EmergencySupportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emergencySupport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmergencySupportExists(emergencySupport.EmergencySupportId))
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
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "UserId", emergencySupport.UserId);
            return View(emergencySupport);
        }

        // GET: EmergencySupports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryEmergencySupport == null)
            {
                return NotFound();
            }

            var emergencySupport = await _context.StrawberryEmergencySupport
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmergencySupportId == id);
            if (emergencySupport == null)
            {
                return NotFound();
            }

            return View(emergencySupport);
        }

        // POST: EmergencySupports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryEmergencySupport == null)
            {
                return Problem("Entity set 'AppDbContext.EmergencySupport'  is null.");
            }
            var emergencySupport = await _context.StrawberryEmergencySupport.FindAsync(id);
            if (emergencySupport != null)
            {
                _context.StrawberryEmergencySupport.Remove(emergencySupport);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmergencySupportExists(int id)
        {
          return (_context.StrawberryEmergencySupport?.Any(e => e.EmergencySupportId == id)).GetValueOrDefault();
        }
    }
}
