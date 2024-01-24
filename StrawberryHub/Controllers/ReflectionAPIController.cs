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
    public class ReflectionsAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReflectionsAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ReflectionsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reflection>>> GetReflections()
        {
            var reflections = await _context.Reflection.ToListAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            var jsonString = JsonSerializer.Serialize(reflections, jsonOptions);

            return Ok(jsonString);
        }

        // GET: api/ReflectionsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reflection>> GetReflection(int id)
        {
            var reflection = await _context.Reflection
                .Include(r => r.User) // Include related User data if needed
                .FirstOrDefaultAsync(m => m.ReflectionId == id);

            if (reflection == null)
            {
                return NotFound();
            }

            return Ok(reflection);
        }

        // PUT: api/ReflectionsAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReflection(int id, Reflection reflection)
        {
            if (id != reflection.ReflectionId)
            {
                return BadRequest("Invalid ID");
            }

            _context.Entry(reflection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReflectionExists(id))
                {
                    return NotFound("Reflection not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ReflectionsAPI
        [HttpPost]
        public async Task<ActionResult<Reflection>> PostReflection(Reflection reflection)
        {
            _context.Reflection.Add(reflection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReflection", new { id = reflection.ReflectionId }, reflection);
        }

        // DELETE: api/ReflectionsAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Reflection>> DeleteReflection(int id)
        {
            var reflection = await _context.Reflection.FindAsync(id);
            if (reflection == null)
            {
                return NotFound("Reflection not found");
            }

            _context.Reflection.Remove(reflection);
            await _context.SaveChangesAsync();

            return reflection;
        }

        private bool ReflectionExists(int id)
        {
            return _context.Reflection.Any(e => e.ReflectionId == id);
        }
    }
}
