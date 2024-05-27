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
    [Route("api/article")]
    [ApiController]
    public class ArticlesAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ArticlesAPIController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/ArticlesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryArticle>>> GetArticles()
        {
            var articles = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .Include(a => a.StrawberryUser)
                .ToListAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            var jsonString = JsonSerializer.Serialize(articles, jsonOptions);

            return Ok(jsonString);
        }


        // GET: api/ArticlesAPI/5
        [HttpGet("Retrieve/{id}")]
        public async Task<ActionResult<StrawberryArticle>> GetArticle(int id)
        {
            var article = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .Include(a => a.StrawberryUser)
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

        private string DoPhotoUpload(IFormFile photo)
        {
            string fext = Path.GetExtension(photo.FileName);
            string uname = Guid.NewGuid().ToString();
            string fname = uname + fext;
            string fullpath = Path.Combine(_env.WebRootPath, "photos/" + fname);
            using (FileStream fs = new(fullpath, FileMode.Create))
            {
                photo.CopyTo(fs);
            }
            return fname;
        }

        // POST: api/ArticlesAPI
        [HttpPost("Create/{GoalTypeId}/{Title}/{ArticleContent}/{UserId}")]
        public async Task<ActionResult<StrawberryArticle>> PostArticle(int GoalTypeId, string Title, string ArticleContent, IFormFile photo, int UserId)
        {

                StrawberryArticle article = new StrawberryArticle();
                ModelState.Remove("Picture");     // No Need to Validate "Picture" - derived from "Photo".
                ModelState.Remove("UserId");
                ModelState.Remove("PublishDate");

                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.Name);
                int userId = 0;
                if (userIdClaim != null)
                {
                    string username = userIdClaim.Value; // Extract the value of the claim
                    userId = await _context.StrawberryUser
                        .Where(u => u.Username == username)
                        .Select(u => u.UserId)
                        .FirstOrDefaultAsync();

                    // Now userId contains the UserId of the user with the specified username
                }

                string picfilename = DoPhotoUpload(photo);
                article.Picture = picfilename.EscQuote();
                article.UserId = UserId; // Assign the retrieved user id to the article
                article.PublishedDate = DateTime.Now;
                article.Title = Title;
                article.ArticleContent = ArticleContent;
                article.GoalTypeId = GoalTypeId;
                _context.StrawberryArticle.Add(article);
                _context.SaveChanges();

                return Ok("Article Created Successfully");


        }

        // PUT: api/ArticlesAPI/5
        [HttpPut("Update/{id}")]
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
        // Add this method to your ArticlesAPIController
// GET: api/ArticlesAPI/Search?query=Stress
[HttpGet("Search/{query}")]
public async Task<ActionResult<IEnumerable<StrawberryArticle>>> SearchArticles(string query)
{
    if (string.IsNullOrWhiteSpace(query))
    {
        // Handle invalid or empty search queries
        return BadRequest("Invalid search query");
    }

    var jsonOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.Preserve,

    };

    var matchingArticles = await _context.StrawberryArticle
        .Include(a => a.GoalType)
        .Where(a => a.ArticleContent.Contains(query.ToLower()) ||
                    a.GoalType.Type.Contains(query.ToLower()))
        .Select(a => new 
        {
            ArticleId = a.ArticleId,
            GoalTypeId = a.GoalTypeId,
            ArticleContent = a.ArticleContent,
            PublishedDate = a.PublishedDate,
            Picture = a.Picture,
            Title = a.Title,
            UserId = a.UserId
        })
        .ToListAsync();

    if (matchingArticles == null || matchingArticles.Count == 0)
    {
        return NotFound("No articles found for the given query");
    }

    var jsonString = JsonSerializer.Serialize(matchingArticles, jsonOptions);

            var jsonObject = JsonDocument.Parse(jsonString).RootElement;

    return Ok(matchingArticles);
}


    }
}
