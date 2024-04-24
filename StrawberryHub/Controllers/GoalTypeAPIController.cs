using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrawberryHub.Controllers.API
{
    [Route("api/goaltype")]
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
        public async Task<ActionResult<IEnumerable<StrawberryGoalType>>> GetGoalTypes()
        {
            return await _context.StrawberryGoalType.ToListAsync();
        }

        // GET: api/GoalTypesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryGoalType>> GetGoalType(int id)
        {
            var goalType = await _context.StrawberryGoalType.FindAsync(id);

            if (goalType == null)
            {
                return NotFound("GoalType not found");
            }

            return goalType;
        }

        // PUT: api/GoalTypesAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoalType(int id, StrawberryGoalType goalType)
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
        public async Task<ActionResult<StrawberryGoalType>> PostGoalType(StrawberryGoalType goalType)
        {
            _context.StrawberryGoalType.Add(goalType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoalType", new { id = goalType.GoalTypeId }, goalType);
        }

        // DELETE: api/GoalTypesAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StrawberryGoalType>> DeleteGoalType(int id)
        {
            var goalType = await _context.StrawberryGoalType.FindAsync(id);
            if (goalType == null)
            {
                return NotFound("GoalType not found");
            }

            _context.StrawberryGoalType.Remove(goalType);
            await _context.SaveChangesAsync();

            return goalType;
        }

        private bool GoalTypeExists(int id)
        {
            return _context.StrawberryGoalType.Any(e => e.GoalTypeId == id);
        }

        // GET: api/GoalTypesAPI/Type
        [HttpGet("Type")]
        public async Task<ActionResult<IEnumerable<object>>> GetGoalTypeAndId()
        {
            return await _context.StrawberryGoalType
                .Select(g => new { 
                    g.GoalTypeId, 
                    g.Type })
                .ToListAsync();
        }

    }
}
