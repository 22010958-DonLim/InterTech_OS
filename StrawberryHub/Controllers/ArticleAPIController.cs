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
    public class ArticlesAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ArticlesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryArticle>>> GetArticles()
        {
            var articles = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .ToListAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            var jsonString = JsonSerializer.Serialize(articles, jsonOptions);

            return Ok(jsonString);
        }


        // GET: api/ArticlesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryArticle>> GetArticle(int id)
        {
            var article = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            if (article == null)
            {
                return NotFound();
            }

            // Retrieve the user's ID from the authentication context
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            // Retrieve the user's chosen goal type ID from the user table
            //int? userGoalTypeId = await _context.User
                //.Where(u => u.UserId == userId)
                //.Select(u => u.GoalTypeId)
                //.FirstOrDefaultAsync();

            // Check if the article's goal type matches the user's chosen goal type
           // if (userGoalTypeId.HasValue && article.GoalTypeId != userGoalTypeId)
            //{
                // If not, the user doesn't have access to this article based on their chosen goal
                //return Forbid(); // You can return a different status code based on your requirements
           // }

            return article;
        }

        // POST: api/ArticlesAPI
        [HttpPost]
        public async Task<ActionResult<StrawberryArticle>> PostArticle(StrawberryArticle article)
        {
            _context.StrawberryArticle.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.ArticleId }, article);
        }

        // PUT: api/ArticlesAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, StrawberryArticle article)
        {
            if (id != article.ArticleId)
            {
                return BadRequest();
            }

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
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

        // DELETE: api/ArticlesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.StrawberryArticle.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.StrawberryArticle.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticleExists(int id)
        {
            return _context.StrawberryArticle.Any(e => e.ArticleId == id);
        }

        // Add this method to your ArticlesAPIController
        // GET: api/ArticlesAPI/Search?query=Stress
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<StrawberryArticle>>> SearchArticles(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // Handle invalid or empty search queries
                return BadRequest("Invalid search query");
            }

            var matchingArticles = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .Where(a => a.ArticleContent.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            a.GoalType.Type.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            if (matchingArticles == null || matchingArticles.Count == 0)
            {
                return NotFound("No articles found for the given query");
            }

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            var jsonString = JsonSerializer.Serialize(matchingArticles, jsonOptions);

            return Ok(jsonString);
        }

    }
}
