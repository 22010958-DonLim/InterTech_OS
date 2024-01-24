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
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            var articles = await _context.Article
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
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Article
                .Include(a => a.GoalType)
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        // POST: api/ArticlesAPI
        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle(Article article)
        {
            _context.Article.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.ArticleId }, article);
        }

        // PUT: api/ArticlesAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
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
            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.ArticleId == id);
        }
    }
}
