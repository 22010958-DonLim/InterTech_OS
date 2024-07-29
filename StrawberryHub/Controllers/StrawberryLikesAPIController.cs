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
    [Route("api/Likes")]
    [ApiController]
    public class StrawberryLikesAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StrawberryLikesAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StrawberryLikesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryLike>>> GetStrawberryLike()
        {
          if (_context.StrawberryLike == null)
          {
              return NotFound();
          }
            return await _context.StrawberryLike.ToListAsync();
        }

        // GET: api/StrawberryLikesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryLike>> GetStrawberryLike(int id)
        {
          if (_context.StrawberryLike == null)
          {
              return NotFound();
          }
            var strawberryLike = await _context.StrawberryLike.FindAsync(id);

            if (strawberryLike == null)
            {
                return NotFound();
            }

            return strawberryLike;
        }

        [HttpGet("TotalLikes/{ArticleId}")]
        public async Task<ActionResult<int>> GetTotalStrawberryLikes(int articleId)
        {
            if (_context.StrawberryLike == null)
            {
                return NotFound();
            }

            var articleLikes = await _context.StrawberryLike
                .Include(c => c.StrawberryUser)
                .Where(c => c.ArticleId == articleId)
                .Select(c => new
                {
                    UserId = c.UserId,
                    Username = c.StrawberryUser.Username,
                    Likes = c.Likes,
                    LikeDateTime = c.LikeDateTime
                })
                .ToListAsync();


            return Ok(articleLikes);
        }

        [HttpGet("TotalLikesCount/{ArticleId}")]
        public async Task<ActionResult<int>> TotalStrawberryLikesCount(int articleId)
        {
            if (_context.StrawberryLike == null)
            {
                return NotFound();
            }

            var articleLikes = await _context.StrawberryLike
                .Include(c => c.StrawberryUser)
                .Where(c => c.ArticleId == articleId)
                .Select(c => new
                {
                    UserId = c.UserId,
                    Username = c.StrawberryUser.Username,
                    Likes = c.Likes,
                    LikeDateTime = c.LikeDateTime
                })
                .ToListAsync();

            return Ok(articleLikes.Count());
        }

        // PUT: api/StrawberryLikesAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStrawberryLike(int id, StrawberryLike strawberryLike)
        {
            if (id != strawberryLike.LikeId)
            {
                return BadRequest();
            }

            _context.Entry(strawberryLike).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StrawberryLikeExists(id))
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

        // POST: api/StrawberryLikesAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/Likes
        [HttpPost("Create/{UserId}/{ArticleId}/{Likes}")]
        public async Task<ActionResult<StrawberryLike>> PostStrawberryLike(int userId, int Likes, int articleId)
        {
            if (_context.StrawberryLike == null)
            {
                return Problem("Entity set 'AppDbContext.StrawberryLike' is null.");
            }

            // Remove all likes for this article and user where Likes != 1
            var invalidLikes = await _context.StrawberryLike
                .Where(l => l.ArticleId == articleId && l.UserId == userId && l.Likes != 1)
                .ToListAsync();

            if (invalidLikes.Any())
            {
                _context.StrawberryLike.RemoveRange(invalidLikes);
                await _context.SaveChangesAsync();
            }

            // Check if the user has already liked the article
            var existingLike = await _context.StrawberryLike
                .FirstOrDefaultAsync(l => l.ArticleId == articleId && l.UserId == userId);

            var strawberryLike = new StrawberryLike();
            strawberryLike.UserId = userId;
            strawberryLike.ArticleId = articleId;
            strawberryLike.Likes = 1;
            strawberryLike.LikeDateTime = DateTime.Now;

            if (existingLike != null)
            {
                // User has already liked the article, so we'll remove the like (unlike)
                _context.StrawberryLike.Remove(existingLike);
                await _context.SaveChangesAsync();
                return Ok("You Have Disliked the article");
            }
            else
            {
                // User hasn't liked any article today, so create a new like record
                _context.StrawberryLike.Add(strawberryLike);
                await _context.SaveChangesAsync();
                return Ok("You Have Liked the article");
            }
        }

        // DELETE: api/Likes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStrawberryLike(int id)
        {
            if (_context.StrawberryLike == null)
            {
                return NotFound();
            }

            var strawberryLike = await _context.StrawberryLike.FindAsync(id);
            if (strawberryLike == null)
            {
                return NotFound();
            }

            _context.StrawberryLike.Remove(strawberryLike);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StrawberryLikeExists(int id)
        {
            return (_context.StrawberryLike?.Any(e => e.LikeId == id)).GetValueOrDefault();
        }
    }
}
