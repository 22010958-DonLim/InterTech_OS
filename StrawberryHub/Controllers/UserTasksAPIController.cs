using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using StrawberryHub.Services;

namespace StrawberryHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTasksAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserTasksAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserTasksAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryUserTask>>> GetStrawberryUserTask()
        {
          if (_context.StrawberryUserTask == null)
          {
              return NotFound();
          }
            return await _context.StrawberryUserTask.ToListAsync();
        }

        // GET: api/UserTasksAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryUserTask>> GetStrawberryUserTask(int id)
        {
          if (_context.StrawberryUserTask == null)
          {
              return NotFound();
          }
            var strawberryUserTask = await _context.StrawberryUserTask.FindAsync(id);

            if (strawberryUserTask == null)
            {
                return NotFound();
            }

            return strawberryUserTask;
        }

        // PUT: api/UserTasksAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStrawberryUserTask(int id, StrawberryUserTask strawberryUserTask)
        {
            if (id != strawberryUserTask.CompletedId)
            {
                return BadRequest();
            }

            _context.Entry(strawberryUserTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StrawberryUserTaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserTasksAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StrawberryUserTask>> PostStrawberryUserTask(StrawberryUserTask strawberryUserTask)
        {
          if (_context.StrawberryUserTask == null)
          {
              return Problem("Entity set 'AppDbContext.StrawberryUserTask'  is null.");
          }
            _context.StrawberryUserTask.Add(strawberryUserTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStrawberryUserTask", new { id = strawberryUserTask.CompletedId }, strawberryUserTask);
        }

        // DELETE: api/UserTasksAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStrawberryUserTask(int id)
        {
            if (_context.StrawberryUserTask == null)
            {
                return NotFound();
            }
            var strawberryUserTask = await _context.StrawberryUserTask.FindAsync(id);
            if (strawberryUserTask == null)
            {
                return NotFound();
            }

            _context.StrawberryUserTask.Remove(strawberryUserTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StrawberryUserTaskExists(int id)
        {
            return (_context.StrawberryUserTask?.Any(e => e.CompletedId == id)).GetValueOrDefault();
        }
    }
}
