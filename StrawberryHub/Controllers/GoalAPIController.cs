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
    [Route("api/goal")]
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
        public async Task<ActionResult<IEnumerable<StrawberryGoal>>> GetGoals()
        {
            var goals = await _context.StrawberryGoal
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

        // GET: api/goal/5        // GET: api/goal/User/{UserId}
        [HttpGet("User/{UserId}")]
        public async Task<ActionResult<IEnumerable<StrawberryGoal>>> GetGoalsByUser(int userId)
        {
            var goals = await _context.StrawberryGoal
                .Where(m => m.UserId == userId)
                .ToListAsync();

            if (goals == null || !goals.Any())
            {
                return NotFound("No goals found for this user");
            }
            var result = goals.Select(g => new
            {
                GoalTypeId = g.GoalTypeId,
                GoalId = g.GoalId
            });

            var jsonString = JsonSerializer.Serialize(result);

            return Ok(jsonString);
        }


        // PUT: api/GoalsAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoal(int id, StrawberryGoal goal)
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
        public async Task<ActionResult<StrawberryGoal>> PostGoal(StrawberryGoal goal)
        {
            _context.StrawberryGoal.Add(goal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoal", new { id = goal.GoalId }, goal);
        }

        // DELETE: api/GoalsAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StrawberryGoal>> DeleteGoal(int id)
        {
            var goal = await _context.StrawberryGoal.FindAsync(id);
            if (goal == null)
            {
                return NotFound("Goal not found");
            }

            _context.StrawberryGoal.Remove(goal);
            await _context.SaveChangesAsync();

            return goal;
        }

        private bool GoalExists(int id)
        {
            return _context.StrawberryGoal.Any(e => e.GoalId == id);
        }

    }
}
