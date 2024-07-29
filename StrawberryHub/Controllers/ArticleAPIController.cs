using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using static StrawberryHub.Controllers.ArticlesController;
using System.Text;
using System.IO;

namespace StrawberryHub.Controllers.API
{
    [Route("api/article")]
    [ApiController]
    public class ArticlesAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string chatGptApiKey = "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";

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

        private string PhotoUpload(IFormFile photo)
        {
            string fname = photo.FileName; // Use the file name we provided
            string fullpath = Path.Combine(_env.WebRootPath, "photos/" + fname);
            using (FileStream fs = new(fullpath, FileMode.Create))
            {
                photo.CopyTo(fs);
            }
            return fname;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<StrawberryArticle>> PostArticle(
     [FromForm] int GoalTypeId,
     [FromForm] string Title,
     [FromForm] string ArticleContent,
     [FromForm] IFormFile Photo,
     [FromForm] string Username)
        {
            // Find the UserId based on Username
            int userId = await _context.StrawberryUser
                .Where(u => u.Username == Username)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();

            if (userId <= 0)
            {
                return BadRequest("Invalid Username");
            }

            StrawberryArticle article = new StrawberryArticle();

            // Handle file upload
            if (Photo != null && Photo.Length > 0)
            {
                string picfilename = DoPhotoUpload(Photo);
                article.Picture = picfilename;
            }
            else
            {
                return BadRequest("Photo is required");
            }

            article.UserId = userId;
            article.PublishedDate = DateTime.Now;
            article.Title = Title;
            article.ArticleContent = ArticleContent;
            article.GoalTypeId = GoalTypeId;
            _context.StrawberryArticle.Add(article);
            await _context.SaveChangesAsync();

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

        [HttpGet("Generate/{generatedTitle}")]
        public async Task<IActionResult> GenerateImage(string generatedTitle)
        {
            var model = "dall-e-3";
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            var endpoint = "https://api.openai.com/v1/images/generations";
            string imagePrompt = $"Generate an image representing the title: '{generatedTitle}'. The image should showcase young people in the Singapore CBD engaging in activities related to the topic. Some places could be like marina bay sands, jewel airport, singapore university, singapore polytechnic or some historical sites around singapore";
            var requestBody = new
            {
                model = model,
                prompt = imagePrompt,
                n = 1,
                size = "1024x1024"
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {chatGptApiKey}");
                var jsonRequestBody = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var contents = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(endpoint, contents);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, new { error = $"API request failed: {response.StatusCode}", details = responseContent });
                    }

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    using (JsonDocument document = JsonDocument.Parse(responseContent))
                    {
                        JsonElement root = document.RootElement;
                        if (root.TryGetProperty("data", out JsonElement dataArray) &&
                            dataArray.GetArrayLength() > 0 &&
                            dataArray[0].TryGetProperty("url", out JsonElement urlElement))
                        {
                            string imageUrl = urlElement.GetString()!;
                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                using (var imageClient = new HttpClient())
                                {
                                    try
                                    {
                                        byte[] generated = await imageClient.GetByteArrayAsync(imageUrl);
                                        string uniqueFileName = $"{Guid.NewGuid()}_{generatedTitle}.png";
                                        using (var memoryStream = new MemoryStream(generated))
                                        {
                                            

                                            var formFile = new FormFile(memoryStream, 0, generated.Length, "generatedImage", uniqueFileName)
                                            {
                                                Headers = new HeaderDictionary(),
                                                ContentType = "image/png"
                                            };

                                            string savedFileName = PhotoUpload(formFile);
                                            string base64Image = Convert.ToBase64String(generated);
                                            string imageDataUrl = $"data:image/png;base64,{base64Image}";
                                            return Ok(new { imageUrl = imageUrl, imageFile = formFile });
                                        }
                                    }
                                    catch (HttpRequestException ex)
                                    {
                                        return StatusCode(500, new { error = "Failed to download the generated image", details = ex.Message });
                                    }
                                }
                            }
                        }
                    }

                    return StatusCode(500, new { error = "Failed to extract image URL from API response", details = responseContent });
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, new { error = "Failed to connect to the image generation service", details = ex.Message });
                }
                catch (JsonException ex)
                {
                    return StatusCode(500, new { error = "Failed to process the response from the image generation service", details = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "An unexpected error occurred", details = ex.Message });
                }
            }
        }


    }
}
