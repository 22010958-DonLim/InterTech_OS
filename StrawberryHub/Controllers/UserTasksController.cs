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
    public class UserTasksController : Controller
    {
        private readonly AppDbContext _context;

        public UserTasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UserTasks
        public async Task<IActionResult> Index()
        {
			var strawberryUserTasks = await _context.StrawberryUserTask
                .Include(i => i.StrawberryTask)
                .Include(i => i.StrawberryUser)
                .ToListAsync();
			return View(strawberryUserTasks);
		}

        // GET: UserTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryUserTask == null)
            {
                return NotFound();
            }

            var strawberryUserTask = await _context.StrawberryUserTask
                .Include(s => s.StrawberryTask)
                .Include(s => s.StrawberryUser)
                .FirstOrDefaultAsync(m => m.CompletedId == id);
            if (strawberryUserTask == null)
            {
                return NotFound();
            }

            return View(strawberryUserTask);
        }

        // GET: UserTasks/Create
        public IActionResult Create()
        {
            ViewData["TaskId"] = new SelectList(_context.StrawberryTask, "TaskId", "TaskId");
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username");
            return View();
        }

        // POST: UserTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompletedId,TaskId,UserId,Points,CompletedDate")] StrawberryUserTask strawberryUserTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(strawberryUserTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskId"] = new SelectList(_context.StrawberryTask, "TaskId", "TaskId", strawberryUserTask.TaskId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryUserTask.UserId);
            return View(strawberryUserTask);
        }

        // GET: UserTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryUserTask == null)
            {
                return NotFound();
            }

            var strawberryUserTask = await _context.StrawberryUserTask.FindAsync(id);
            if (strawberryUserTask == null)
            {
                return NotFound();
            }
            ViewData["TaskId"] = new SelectList(_context.StrawberryTask, "TaskId", "TaskId", strawberryUserTask.TaskId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryUserTask.UserId);
            return View(strawberryUserTask);
        }

        // POST: UserTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompletedId,TaskId,UserId,Points,CompletedDate")] StrawberryUserTask strawberryUserTask)
        {
            if (id != strawberryUserTask.CompletedId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(strawberryUserTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StrawberryUserTaskExists(strawberryUserTask.CompletedId))
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
            ViewData["TaskId"] = new SelectList(_context.StrawberryTask, "TaskId", "TaskId", strawberryUserTask.TaskId);
            ViewData["UserId"] = new SelectList(_context.StrawberryUser, "UserId", "Username", strawberryUserTask.UserId);
            return View(strawberryUserTask);
        }

        // GET: UserTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryUserTask == null)
            {
                return NotFound();
            }

            var strawberryUserTask = await _context.StrawberryUserTask
                .Include(s => s.StrawberryTask)
                .Include(s => s.StrawberryUser)
                .FirstOrDefaultAsync(m => m.CompletedId == id);
            if (strawberryUserTask == null)
            {
                return NotFound();
            }

            return View(strawberryUserTask);
        }

        // POST: UserTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryUserTask == null)
            {
                return Problem("Entity set 'AppDbContext.StrawberryUserTask'  is null.");
            }
            var strawberryUserTask = await _context.StrawberryUserTask.FindAsync(id);
            if (strawberryUserTask != null)
            {
                _context.StrawberryUserTask.Remove(strawberryUserTask);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StrawberryUserTaskExists(int id)
        {
          return (_context.StrawberryUserTask?.Any(e => e.CompletedId == id)).GetValueOrDefault();
        }
    }
}
