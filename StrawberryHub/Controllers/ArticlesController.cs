using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StrawberryHub.Models;
using StrawberryHub.Services;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Text.Json.Serialization;

namespace StrawberryHub.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string chatGptApiKey = "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";
        private const string ImageBufferKey = "ImageBuffer";

        public ArticlesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private byte[] GetImageBuffer()
        {
            return HttpContext.Session.Get(ImageBufferKey)!;
        }

        private void SetImageBuffer(byte[] buffer)
        {
            HttpContext.Session.Set(ImageBufferKey, buffer);
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var articles = _context.StrawberryArticle
                .Include(a => a.GoalType)
                .Include(a => a.StrawberryUser) // Assuming there's a property named User in StrawberryArticle representing the user who created the article
                .ToListAsync();
            return View(await articles);
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StrawberryArticle == null)
            {
                return NotFound();
            }

            var article = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .Include(a => a.StrawberryUser)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/ArticleDetail/5
        public async Task<IActionResult> ArticleDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .Include(a => a.StrawberryUser)
                .Include(a => a.StrawberryLike)
                .Include(a => a.StrawberryComment)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            var usernameClaim = HttpContext.User.FindFirst(ClaimTypes.Name);

            if (usernameClaim == null)
            {
                return BadRequest("Username claim not found");
            }

            var username = usernameClaim.Value;

            // Fetch the user from the database, including their preferences
            var user = _context.StrawberryUser.Include(u => u.Goal).FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Check if the current user has liked this article
            var isLiked = article.StrawberryLike.Any(l => l.UserId == user.UserId);

            // Pass the like status to the view using ViewBag
            ViewBag.user = user.UserId;

            return View(article); // Assuming you have an "ArticleDetail.cshtml" view
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "Type");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,GoalTypeId,Title,ArticleContent,PublishedDate,Photo,Picture,UserId")] StrawberryArticle article, IFormFile photo)
        {
            ModelState.Remove("Picture");     // No Need to Validate "Picture" - derived from "Photo".
            ModelState.Remove("UserId");
            ModelState.Remove("PublishDate");
            if (ModelState.IsValid)
            {
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

                    string picfilename = DoPhotoUpload(article.Photo);
                    article.Picture = picfilename.EscQuote();
                    article.UserId = userId; // Assign the retrieved user id to the article
                    article.PublishedDate = DateTime.Now;
                    _context.Add(article);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                
            }
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "GoalTypeId", article.GoalTypeId);
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StrawberryArticle == null)
            {
                return NotFound();
            }

            var article = await _context.StrawberryArticle.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "Type", article.GoalTypeId);

            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,GoalTypeId,Title,ArticleContent,PublishedDate,Photo,Picture,UserId")] StrawberryArticle article, IFormFile photo)
        {
            ModelState.Remove("Photo");       // No Need to Validate "Photo"
            ModelState.Remove("UserId");
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null)
                    {
                        string picfilename = DoPhotoUpload(photo);
                        article.Picture = picfilename.EscQuote();
                    }
                    article.PublishedDate = DateTime.Now;
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "GoalTypeId", article.GoalTypeId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StrawberryArticle == null)
            {
                return NotFound();
            }

            var article = await _context.StrawberryArticle
                .Include(a => a.GoalType)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StrawberryArticle == null)
            {
                return Problem("Entity set 'AppDbContext.Article'  is null.");
            }
            var article = await _context.StrawberryArticle.FindAsync(id);
            if (article != null)
            {
                string photoFile = article.Picture;
                string fullpath = Path.Combine(_env.WebRootPath, "photos/" + photoFile);
                System.IO.File.Delete(fullpath);

                _context.StrawberryArticle.Remove(article);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
          return (_context.StrawberryArticle?.Any(e => e.ArticleId == id)).GetValueOrDefault();
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

        // GET: Articles/Recent
        public async Task<IActionResult> ShowArticle()
        {
            var articles = await _context.StrawberryArticle
                .OrderByDescending(a => a.PublishedDate)
                .Take(3)
                .ToListAsync();

            return View("ShowArticle", articles);
        }


        public async Task<IActionResult> ArticlePage()
        {

			var articles = _context.StrawberryArticle
				 .Include(a => a.GoalType)
				 .Include(a => a.StrawberryUser) // Assuming there's a property named User in StrawberryArticle representing the user who created the article
				 .ToListAsync();

			return View("ArticlePage", await articles);
		}

		public IActionResult CreateArticlePage()
		{
			ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "Type");
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticlePage([Bind("ArticleId,GoalTypeId,Title,ArticleContent,PublishedDate,Photo,Picture,UserId")] StrawberryArticle article, IFormFile photo)
        {
            ModelState.Remove("Picture");     // No Need to Validate "Picture" - derived from "Photo".
            ModelState.Remove("UserId");
            ModelState.Remove("PublishedDate");

            article.PublishedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
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

				// Check if the user has already completed the "Posting Article" task today
				var currentDate = DateTime.Now;
				var startOfDay = currentDate.Date;
				var postingTaskId = 2; // Assuming 1 is the ID for "Posting Article" task
				var hasCompletedPostingToday = await _context.StrawberryUserTask
					.AnyAsync(s => s.UserId == userId &&
								   s.TaskId == postingTaskId &&
								   s.CompletedDate.HasValue &&
								   s.CompletedDate.Value.Date == startOfDay);

				if (!hasCompletedPostingToday)
				{
					// User hasn't completed the task today, so create a new record
					var task = await _context.StrawberryTask.FindAsync(postingTaskId);
					if (task != null)
					{
						var newUserTask = new StrawberryUserTask
						{
							TaskId = postingTaskId,
							UserId = userId,
							Points = task.PointsReward,
							CompletedDate = currentDate
						};
						_context.StrawberryUserTask.Add(newUserTask);

						// Update the user's points in StrawberryUser table
						var user = await _context.StrawberryUser.FindAsync(userId);
						if (user != null)
						{
							user.Points += task.PointsReward; // Assuming 'Points' is a field in StrawberryUser
                                                              // Check if the user has enough points to rank up
                            var nextRank = _context.StrawberryRank
                                .Where(r => r.MinPoints <= user.Points && r.MaxPoints >= user.Points)
                                .OrderBy(r => r.RankId)
                                .FirstOrDefault();

                            if (nextRank != null && nextRank.RankId > user.RankId)
                            {
                                // Update the user's rank
                                user.RankId = nextRank.RankId;
                            }

                            _context.Entry(user).State = EntityState.Modified;
                        }
					}
				}


				string picfilename = DoPhotoUpload(article.Photo);
                article.Picture = picfilename.EscQuote();
                article.UserId = userId; // Assign the retrieved user id to the article
                article.PublishedDate = DateTime.Now;
                _context.Add(article);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Article uploaded successfully!"; // Add this line
				TempData["MsgType"] = "Success";

				return RedirectToAction("Index", "Home");

            }
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "GoalTypeId", article.GoalTypeId);
            return View(article);
        }

		[Authorize]
		public async Task<IActionResult> Stress()
		{

			var articles = _context.StrawberryArticle
				 .Include(a => a.GoalType)
				 .Include(a => a.StrawberryUser)
                 .Where(a => a.GoalTypeId == 1)
				 .ToListAsync();

			return View("Stress", await articles);
		}

		[Authorize]
		public async Task<IActionResult> Mental()
		{

			var articles = _context.StrawberryArticle
				 .Include(a => a.GoalType)
				 .Include(a => a.StrawberryUser)
				 .Where(a => a.GoalTypeId == 2)
				 .ToListAsync();

			return View("Mental", await articles);
		}

		[Authorize]
		public async Task<IActionResult> NewHobbies()
		{

			var articles = _context.StrawberryArticle
				 .Include(a => a.GoalType)
				 .Include(a => a.StrawberryUser)
				 .Where(a => a.GoalTypeId == 3)
				 .ToListAsync();

			return View("NewHobbies", await articles);
		}

		[Authorize]
		public async Task<IActionResult> AdminArticle()
		{

			var articles = await _context.StrawberryArticle
					.Include(a => a.StrawberryUser)
					.Where(a => a.StrawberryUser.UserRole == "Admin")
					.ToListAsync();

			return View("AdminArticle", articles);
		}

		// GET: Articles/Edit/5
		public async Task<IActionResult> EditArticle(int? id)
		{
			if (id == null || _context.StrawberryArticle == null)
			{
				return NotFound();
			}

			var article = await _context.StrawberryArticle.FindAsync(id);
			if (article == null)
			{
				return NotFound();
			}

			ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "Type", article.GoalTypeId);

			return View(article);
		}

		// POST: Articles/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditArticle(int id, [Bind("ArticleId,GoalTypeId,Title,ArticleContent,PublishedDate,Photo,Picture,UserId")] StrawberryArticle article, IFormFile photo)
		{
			ModelState.Remove("Photo");       // No Need to Validate "Photo"
			ModelState.Remove("UserId");
			if (id != article.ArticleId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					if (photo != null)
					{
						string picfilename = DoPhotoUpload(photo);
						article.Picture = picfilename.EscQuote();
					}
					article.PublishedDate = DateTime.Now;
					_context.Update(article);
					await _context.SaveChangesAsync();

                    // Add success message to TempData
                    TempData["SuccessMessage"] = "Article edited successfully.";

                    // Redirect to the ArticleDetail view
                    return RedirectToAction("ArticleDetail", new { id = article.ArticleId });
                }
				catch (DbUpdateConcurrencyException)
				{
					if (!ArticleExists(article.ArticleId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
			}
			ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "GoalTypeId", article.GoalTypeId);
			return View(article);
		}

        public ActionResult GenerateContent()
        {
            ViewData["GoalTypeId"] = new SelectList(_context.StrawberryGoalType, "GoalTypeId", "Type");
            return View();
        }
        
        public async Task<IActionResult> GeneratedContent()
        {
            try
            {
                var model = "gpt-3.5-turbo";
                var maxTokens = 1000; // Adjust as needed

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {chatGptApiKey}");

                    var chatRequest = new
                    {
                        model = model,
                        messages = new[]
                        {
                    new { role = "system", content = "You are a helpful assistant that generates article title and content from reliable sources." },
                    new { role = "user", content = "\"Create a captivating title and engaging content (250-300 words) focused on one of the following topics: stress management, mental wellness, or exciting new hobbies for youth. The content should be based on recent articles or upcoming events in Singapore that can benefit young Singaporeans from 2023 onwards. Your goal is to create a piece that's both attention-grabbing and genuinely helpful.\r\nStructure your content as follows:\r\n\r\nAn eye-catching title that sparks curiosity\r\nA brief, compelling introduction\r\n3-4 main points that deliver valuable information or actionable tips\r\nA concise conclusion that reinforces the main message\r\n\r\nRemember to keep the tone upbeat and relatable to young readers. Use vivid language, interesting facts, or surprising statistics to maintain engagement throughout the piece.\r\nAt the end of the content, include:\r\n\r\nThe source of the information (ensure it's from the last 6 months)\r\nGoal Type: [Choose one: Mental Wellness / Stress Management / New Hobbies]\r\n\r\nExample title formats to inspire you:\r\n\r\n'5 Mind-Blowing Ways Singaporean Teens Are Beating Stress in 2024'\r\n'The Secret Hobby That's Transforming Youth Mental Health in Singapore'\r\n'You Won't Believe What's Helping Singapore's Gen Z Stay Calm and Focused'\r\n\r\nRemember, the content should be informative and beneficial while also being irresistibly clickable!\"" }
                },
                        max_tokens = maxTokens
                    };

                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(chatRequest), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>();
                        if (chatResponse?.Choices != null && chatResponse.Choices.Length > 0)
                        {
                            var message = chatResponse.Choices[0].Message;
                            if (message != null)
                            {
                                var lines = message.Content?.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                                if (lines != null && lines.Length > 0)
                                {
                                    string generatedTitle = lines[0].Trim().Replace("Article Title: ", "").Trim('"');
                                    string generatedContent = string.Join("\n", lines.Skip(1)).Trim().Replace("Article Content:\n", "");
                                    var relatedLinks = await FetchRelatedLinksAsync(generatedContent);
                                    // Generate image based on the title and content
                                    byte[] generatedImage = await GenerateImageAsync(generatedTitle);
                                    // Convert the byte array to a base64 string for displaying in the view
                                    string base64Image = Convert.ToBase64String(generatedImage);
                                    string imageDataUrl = $"data:image/png;base64,{base64Image}";
                                    SetImageBuffer(generatedImage);
                                    // Fetch related links
                                    return Json(new { success = true, generatedTitle, generatedContent, generatedImage = imageDataUrl, relatedLinks });
                                }
                            }
                        }
                        return Json(new { errorMessage = "No valid content found in the response." });

                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new { errorMessage = $"API Error: {response.StatusCode} - {errorContent}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { errorMessage = $"Exception: {ex.Message}" });
            }
        }

        public async Task<byte[]> GenerateImageAsync(string title)
        {
            var model = "dall-e-3";
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"); // Use environment variable
            var endpoint = "https://api.openai.com/v1/images/generations";
            string imagePrompt = $"Generate an image representing the title: '{title}'. The image should showcase young people in Singapore engaging in activities related to the topic.";
            var requestBody = new
            {
                model = model,
                prompt = imagePrompt,
                n = 1,
                size = "1024x1024"
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var jsonRequestBody = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var contents = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(endpoint, contents);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        // Directly parse the JSON to extract the URL
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
                                        return await imageClient.GetByteArrayAsync(imageUrl);
                                    }
                                }
                            }
                        }

                        // If we couldn't extract the URL, throw an exception
                        throw new Exception($"Failed to extract image URL from response. Full response: {responseContent}");
                    }
                    else
                    {
                        throw new Exception($"Failed to generate image: {response.StatusCode} - {responseContent}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception($"HTTP request failed: {ex.Message}", ex);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new Exception($"Failed to parse JSON response: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unexpected error occurred: {ex.Message}", ex);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ModifyPhoto(string userPrompt, string title)
        {
            var model = "dall-e-3";
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"); // Use environment variable
            var endpoint = "https://api.openai.com/v1/images/generations";
            string imagePrompt = $"Generate an image representing the title: '{title}'. As well as include additional information: '{userPrompt}'. The image should showcase young people in Singapore engaging in activities related to the topic.";
            var requestBody = new
            {
                model = model,
                prompt = imagePrompt,
                n = 1,
                size = "1024x1024"
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var jsonRequestBody = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var contents = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(endpoint, contents);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        // Directly parse the JSON to extract the URL
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
                                        byte[] generated =  await imageClient.GetByteArrayAsync(imageUrl);
                                        string base64Image = Convert.ToBase64String(generated);
                                        string imageDataUrl = $"data:image/png;base64,{base64Image}";
                                        return Json(new { success = true,  generatedImage = imageDataUrl });
                                    }
                                }
                            }
                        }

                        // If we couldn't extract the URL, throw an exception
                        throw new Exception($"Failed to extract image URL from response. Full response: {responseContent}");
                    }
                    else
                    {
                        throw new Exception($"Failed to generate image: {response.StatusCode} - {responseContent}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception($"HTTP request failed: {ex.Message}", ex);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new Exception($"Failed to parse JSON response: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unexpected error occurred: {ex.Message}", ex);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateImg(string title)
        {
            var model = "dall-e-3";
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"); // Use environment variable
            var endpoint = "https://api.openai.com/v1/images/generations";
            string imagePrompt = $"Generate an image representing the title: '{title}'. The image should showcase young people in Singapore engaging in activities related to the topic.";
            var requestBody = new
            {
                model = model,
                prompt = imagePrompt,
                n = 1,
                size = "1024x1024"
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var jsonRequestBody = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var contents = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(endpoint, contents);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        // Directly parse the JSON to extract the URL
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
                                        byte[] generated = await imageClient.GetByteArrayAsync(imageUrl);
                                        string base64Image = Convert.ToBase64String(generated);
                                        string imageDataUrl = $"data:image/png;base64,{base64Image}";
                                        return Json(new { success = true, generatedImage = imageDataUrl });
                                    }
                                }
                            }
                        }

                        // If we couldn't extract the URL, throw an exception
                        throw new Exception($"Failed to extract image URL from response. Full response: {responseContent}");
                    }
                    else
                    {
                        throw new Exception($"Failed to generate image: {response.StatusCode} - {responseContent}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception($"HTTP request failed: {ex.Message}", ex);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new Exception($"Failed to parse JSON response: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unexpected error occurred: {ex.Message}", ex);
                }
            }
        }

        public async Task<IActionResult> ModifyContent(string userPrompt, string generatedTitle, string generatedBody)
        {
            try
            {
                var model = "gpt-3.5-turbo";
                var maxTokens = 1000; // Adjust as needed
                var prompt = $"This is my Title and Article Content\nTitle: {generatedTitle}\nArticle Content: {generatedBody}\nHelp me modify the Article title and content based on the user prompt: {userPrompt}";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {chatGptApiKey}");

                    var chatRequest = new
                    {
                        model = model,
                        messages = new[]
                        {
                    new { role = "system", content = "You are a helpful assistant that is going to help modify the Article Title and Content based on the User Prompt." },
                    new { role = "user", content = prompt}
                },
                        max_tokens = maxTokens
                    };

                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(chatRequest), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>();
                        if (chatResponse?.Choices != null && chatResponse.Choices.Length > 0)
                        {
                            var message = chatResponse.Choices[0].Message;
                            if (message != null)
                            {
                                var lines = message.Content?.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                                if (lines != null && lines.Length > 0)
                                {
                                    string generatedTitles = lines[0].Trim().Replace("Article Title: ", "").Trim('"');
                                    string generatedContent = string.Join("\n", lines.Skip(1)).Trim().Replace("Article Content:\n", "");
                                    return Json(new { success = true, generatedTitles, generatedContent });
                                }
                            }
                        }
                        return Json(new { errorMessage = "No valid content found in the response." });

                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new { errorMessage = $"API Error: {response.StatusCode} - {errorContent}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { errorMessage = $"Exception: {ex.Message}" });
            }
        }

        private async Task<List<RelatedLink>> FetchRelatedLinksAsync(string query)
        {
            string serpApiKey = "b997de0d9b939528a93f7a1b657983f147f4bda7e98795e49193140efdf15c11"; // Replace with your SerpAPI key
            using (var httpClient = new HttpClient())
            {
                var searchUrl = $"https://serpapi.com/search.json?q={Uri.EscapeDataString(query)}&api_key={serpApiKey}";
                try
                {
                    var response = await httpClient.GetAsync(searchUrl);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("API Response: " + json);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var searchResults = System.Text.Json.JsonSerializer.Deserialize<SerpApiSearchResults>(json, options);

                    if (searchResults?.OrganicResults != null)
                    {
                        var relatedLinks = searchResults.OrganicResults
                            .Select(result => new RelatedLink
                            {
                                Title = string.IsNullOrEmpty(result.Title) ? "No Title" : result.Title,
                                Url = string.IsNullOrEmpty(result.Link) ? "No URL" : result.Link
                            })
                            .ToList();

                        foreach (var link in relatedLinks)
                        {
                            Console.WriteLine($"Title: {link.Title}, Link: {link.Url}");
                        }

                        return relatedLinks;
                    }
                    else
                    {
                        Console.WriteLine("No organic results found or organic_results is missing.");
                        return new List<RelatedLink>();
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.Error.WriteLine($"Request error: {e.Message}");
                    return new List<RelatedLink>();
                }
                catch (System.Text.Json.JsonException e)
                {
                    Console.Error.WriteLine($"JSON deserialization error: {e.Message}");
                    return new List<RelatedLink>();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Unexpected error: {e.Message}");
                    return new List<RelatedLink>();
                }
            }
        }

        // Classes for deserializing the SerpAPI response
        public class SerpApiSearchResults
        {
            [JsonPropertyName("organic_results")]
            public List<OrganicResult> OrganicResults { get; set; } = null!;
        }

        public class OrganicResult
        {
            [JsonPropertyName("position")]
            public int Position { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;

            [JsonPropertyName("link")]
            public string Link { get; set; } = string.Empty;

            [JsonPropertyName("redirect_link")]
            public string RedirectLink { get; set; } = string.Empty;

            [JsonPropertyName("displayed_link")]
            public string DisplayedLink { get; set; } = string.Empty;

            [JsonPropertyName("favicon")]
            public string Favicon { get; set; } = string.Empty;

            [JsonPropertyName("date")]
            public string Date { get; set; } = string.Empty;

            [JsonPropertyName("snippet")]
            public string Snippet { get; set; } = string.Empty;

            [JsonPropertyName("snippet_highlighted_words")]
            public List<string> SnippetHighlightedWords { get; set; } = new List<string>();

            [JsonPropertyName("missing")]
            public List<string> Missing { get; set; } = new List<string>();

            [JsonPropertyName("source")]
            public string Source { get; set; } = string.Empty;
        }

        public class RelatedLink
        {
            public string Url { get; set; } = null!;
            public string Title { get; set; } = null!;
        }

        public class AttemptInfo
        {
            public int AttemptsLeft { get; set; }
            public DateTime? ResetTime { get; set; }
        }

        public class ImageCache
        {
            private static byte[]? _lastGeneratedImage;

            public static void StoreImage(byte[] image)
            {
                _lastGeneratedImage = image;
            }

            public static byte[]? GetLastImage()
            {
                return _lastGeneratedImage;
            }
        }



        public class ImageResponse
        {
            public long Created { get; set; }
            public List<ImageData> Data { get; set; } = null!;

        }

        public class ImageData
        {
            public string Revised_prompt { get; set; } = null!;
            public string Url { get; set; } = null!;
        }


        public class ChatResponse
        {
            public string? Id { get; set; }
            public string? Object { get; set; }
            public int Created { get; set; }
            public string? Model { get; set; }
            public Choice[]? Choices { get; set; }
            public string? SystemFingerprint { get; set; }

            public class Choice
            {
                public int Index { get; set; }
                public Message? Message { get; set; }
                public string? FinishReason { get; set; }
            }

            public class Message
            {
                public string? Role { get; set; }
                public string? Content { get; set; }
            }
        }


    }
}
