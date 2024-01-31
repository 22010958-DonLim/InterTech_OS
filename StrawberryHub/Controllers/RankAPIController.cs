using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrawberryHub.Controllers.API
{
    [Route("api/rank")]
    [ApiController]
    public class RanksAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RanksAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RanksAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryRank>>> GetRanks()
        {
            return await _context.StrawberryRank.ToListAsync();
        }

        // GET: api/RanksAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryRank>> GetRank(int id)
        {
            var rank = await _context.StrawberryRank.FindAsync(id);

            if (rank == null)
            {
                return NotFound();
            }

            return rank;
        }

        // PUT: api/RanksAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRank(int id, StrawberryRank rank)
        {
            if (id != rank.RankId)
            {
                return BadRequest("Invalid ID");
            }

            _context.Entry(rank).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RankExists(id))
                {
                    return NotFound("Rank not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RanksAPI
        [HttpPost]
        public async Task<ActionResult<StrawberryRank>> PostRank(StrawberryRank rank)
        {
            _context.StrawberryRank.Add(rank);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRank", new { id = rank.RankId }, rank);
        }

        // DELETE: api/RanksAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StrawberryRank>> DeleteRank(int id)
        {
            var rank = await _context.StrawberryRank.FindAsync(id);
            if (rank == null)
            {
                return NotFound("Rank not found");
            }

            _context.StrawberryRank.Remove(rank);
            await _context.SaveChangesAsync();

            return rank;
        }

        private bool RankExists(int id)
        {
            return _context.StrawberryRank.Any(e => e.RankId == id);
        }
    }
}
