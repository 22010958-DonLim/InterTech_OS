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
    public class UsersAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UsersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryUser>>> GetUsers()
        {
            return await _context.StrawberryUser.ToListAsync();
        }

        // GET: api/UsersAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryUser>> GetUser(int id)
        {
            var user = await _context.StrawberryUser.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/UsersAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, StrawberryUser user)
        {
            if (id != user.UserId)
            {
                return BadRequest("Invalid ID");
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound("User not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UsersAPI
        [HttpPost]
        public async Task<ActionResult<StrawberryUser>> PostUser(StrawberryUser user)
        {
            _context.StrawberryUser.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/UsersAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StrawberryUser>> DeleteUser(int id)
        {
            var user = await _context.StrawberryUser.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            _context.StrawberryUser.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.StrawberryUser.Any(e => e.UserId == id);
        }
    }
}
