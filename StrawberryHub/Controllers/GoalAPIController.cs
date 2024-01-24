using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace StrawberryHub.Controllers.API
{
    [Route("OSHub/[controller]")]
    [ApiController]
    public class GoalsAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GoalsAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GoalsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Goal>>> GetGoals()
        {
            var goals = await _context.Goal
                .Include(g => g.GoalType)
                .Include(g => g.User)
                .ToListAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            var jsonString = JsonSerializer.Serialize(goals, jsonOptions);

            return Ok(jsonString);
        }

        // GET: api/GoalsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Goal>> GetGoal(int id)
        {
            var goal = await _context.Goal
                .Include(g => g.GoalType)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.GoalId == id);

            if (goal == null)
            {
                return NotFound("Goal not found");
            }

            return goal;
        }

        // PUT: api/GoalsAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoal(int id, Goal goal)
        {
            if (id != goal.GoalId)
            {
                return BadRequest("Invalid ID");
            }

            _context.Entry(goal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoalExists(id))
                {
                    return NotFound("Goal not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GoalsAPI
        [HttpPost]
        public async Task<ActionResult<Goal>> PostGoal(Goal goal)
        {
            _context.Goal.Add(goal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoal", new { id = goal.GoalId }, goal);
        }

        // DELETE: api/GoalsAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Goal>> DeleteGoal(int id)
        {
            var goal = await _context.Goal.FindAsync(id);
            if (goal == null)
            {
                return NotFound("Goal not found");
            }

            _context.Goal.Remove(goal);
            await _context.SaveChangesAsync();

            return goal;
        }

        private bool GoalExists(int id)
        {
            return _context.Goal.Any(e => e.GoalId == id);
        }
    }
}
