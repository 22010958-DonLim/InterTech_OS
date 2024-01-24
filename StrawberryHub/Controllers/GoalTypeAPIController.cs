using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrawberryHub.Controllers.API
{
    [Route("OSHub/[controller]")]
    [ApiController]
    public class GoalTypesAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GoalTypesAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GoalTypesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalType>>> GetGoalTypes()
        {
            return await _context.GoalType.ToListAsync();
        }

        // GET: api/GoalTypesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoalType>> GetGoalType(int id)
        {
            var goalType = await _context.GoalType.FindAsync(id);

            if (goalType == null)
            {
                return NotFound("GoalType not found");
            }

            return goalType;
        }

        // PUT: api/GoalTypesAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoalType(int id, GoalType goalType)
        {
            if (id != goalType.GoalTypeId)
            {
                return BadRequest("Invalid ID");
            }

            _context.Entry(goalType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoalTypeExists(id))
                {
                    return NotFound("GoalType not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GoalTypesAPI
        [HttpPost]
        public async Task<ActionResult<GoalType>> PostGoalType(GoalType goalType)
        {
            _context.GoalType.Add(goalType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoalType", new { id = goalType.GoalTypeId }, goalType);
        }

        // DELETE: api/GoalTypesAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GoalType>> DeleteGoalType(int id)
        {
            var goalType = await _context.GoalType.FindAsync(id);
            if (goalType == null)
            {
                return NotFound("GoalType not found");
            }

            _context.GoalType.Remove(goalType);
            await _context.SaveChangesAsync();

            return goalType;
        }

        private bool GoalTypeExists(int id)
        {
            return _context.GoalType.Any(e => e.GoalTypeId == id);
        }
    }
}
