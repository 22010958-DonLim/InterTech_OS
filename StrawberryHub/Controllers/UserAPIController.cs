using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace StrawberryHub.Controllers.API;

[Route("api/users")]
[ApiController]
public class UsersAPIController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDbService _dbSvc;

    public UsersAPIController(AppDbContext context, IDbService dbsvc)
    {
        _context = context;
        _dbSvc = dbsvc;
    }


    [Route("Login/{username}/{pw}")]
    public IActionResult Login(string username, string pw)
    {
        List<StrawberryUser> users = _dbSvc.GetList<StrawberryUser>(
            @"SELECT  UserId, Username, UserRole FROM StrawberryUser
               WHERE Username = '{0}' AND Password = HASHBYTES('SHA1', '{1}')", username, pw);
        if (users.Count == 1)
        {
            return Ok(users[0]);
        }
        else
        {
            return BadRequest();
        }
    }

    // GET: api/users/search/{username}
    [HttpGet("search/{username}")]
    public async Task<ActionResult<IEnumerable<StrawberryUser>>> SearchByUsername(string username)
    {
        var users = await _context.StrawberryUser
            .Where(u => EF.Functions.Like(u.Username, $"%{username}%"))
            .Include(u => u.StrawberryRank)
            .Include(u => u.Goal)
            .ThenInclude(u => u.GoalType)
            .ToListAsync();

        if (!users.Any())
        {
            return NotFound("No users found with the given username.");
        }

        var result = users.Select(u => new
        {
            UserId = u.UserId,
            Username = u.Username,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Points = u.Points,
            RankId = u.RankId,
            Rankname = u.StrawberryRank.RankName,
            Goals = u.Goal.Select(g => new
            {
                GoalId = g.GoalId,
                GoalTypeId = g.GoalType.GoalTypeId
            })
        });

        return Ok(result);
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
    [HttpPost("Register/{username}/{password}/{firstname}/{lastname}/{email}/{goalTypeId1}/{goalTypeId2}/{goalTypeId3}")]
    public IActionResult Register(string username, string password, string firstname, string lastname, string email, int goalTypeId1, int goalTypeId2, int goalTypeId3)
    {
        try
        {

            byte[] passwordHash;
            using (var sha1 = SHA1.Create())
            {
                passwordHash =
                    sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            // Your registration logic here
            // Example: Save the user to the database
            var newUser = new StrawberryUser
            {
                Username = username,
                Password = passwordHash, // Remember to hash passwords securely
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                //GoalTypeId = goalTypeId,
                Points = 0,
                RankId = 4,
                UserRole = "User",
                TelegramId = "1297116053",
                Otp = 0
                // Add other properties as needed
            };

            _context.StrawberryUser.Add(newUser);
            _context.SaveChanges();

            if (goalTypeId1 == 1)
            {
                StrawberryGoal newGoal = new StrawberryGoal();
                newGoal.UserId = newUser.UserId;  // Assuming newUser.UserId is set after adding newUser to context
                newGoal.GoalTypeId = 1;

                _context.Add(newGoal);  // Add new goal to context
            }

            if (goalTypeId2 == 1)
            {
                StrawberryGoal newGoal2 = new StrawberryGoal();
                newGoal2.UserId = newUser.UserId;  // Assuming newUser.UserId is set after adding newUser to context
                newGoal2.GoalTypeId = 2;

                _context.Add(newGoal2);  // Add new goal to context
            }

            if (goalTypeId3 == 1)
            {
                StrawberryGoal newGoal3 = new StrawberryGoal();
                newGoal3.UserId = newUser.UserId;  // Assuming newUser.UserId is set after adding newUser to context
                newGoal3.GoalTypeId = 3;

                _context.Add(newGoal3);  // Add new goal to context
            }

            _context.SaveChanges();
            return Ok("User registered successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    private bool UserExists(int id)
    {
        return _context.StrawberryUser.Any(e => e.UserId == id);
    }

    [HttpGet("ArticlesByUserPreference")]
    public IActionResult GetArticlesByUserPreference()
    {
        try
        {
            // Get the user's claims from the current HttpContext
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

            // Get articles based on user preferences
            var preferredArticles = _context.StrawberryArticle
                .Where(article => user.Goal.Any(preference => preference.GoalTypeId == article.GoalTypeId))
                .Select(article => new
                {
                    ArticleId = article.ArticleId,
                    GoalTypeId = article.GoalTypeId,
                    ArticleContent = article.ArticleContent,
                    PublishedDate = article.PublishedDate
                })
                .ToList();

            // Get the remaining articles not covered by user preferences
            var remainingArticles = _context.StrawberryArticle
                .Where(article => !user.Goal.Any(preference => preference.GoalTypeId == article.GoalTypeId))
                .Select(article => new
                {
                    ArticleId = article.ArticleId,
                    GoalTypeId = article.GoalTypeId,
                    ArticleContent = article.ArticleContent,
                    PublishedDate = article.PublishedDate
                })
                .ToList();

            // Combine the preferred articles and remaining articles
            var allArticles = preferredArticles.Concat(remainingArticles).ToList();

            return Ok(allArticles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("ArticlesByUserPreference/{Username}")]
    public IActionResult GetArticlesByUserPreferences(string Username)
    {
        try
        {
            var user = _context.StrawberryUser
                .Include(u => u.Goal)
                .ThenInclude(u => u.GoalType)
                .FirstOrDefault(u => u.Username.ToLower() == Username.ToLower());

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Fetch all articles from the database
            var allArticles = _context.StrawberryArticle
                .Include(u => u.GoalType)
                .Include(u => u.StrawberryUser)
                .Include(u => u.StrawberryLikeComments)
                .ToList();

            // Project articles into custom object
            var articleDtos = allArticles
                .Select(article => new
                {
                    ArticleId = article.ArticleId,
                    GoalTypeId = article.GoalTypeId,
                    Type = article.GoalType.Type,
                    Title = article.Title,
                    ArticleContent = article.ArticleContent,
                    PublishedDate = article.PublishedDate,
                    UserId = article.UserId,
                    Username = article.StrawberryUser.Username,
                    Picture = article.Picture,
                    TotalLikes = article.StrawberryLikeComments.Count(l => l.Likes == 1), // Count the likes
                    TotalComments = article.StrawberryLikeComments.Count(c => c.CommentText != null) // Count the comments
                })
                .ToList();

            // Separate preferred and remaining articles
            var preferredArticles = articleDtos
                .Where(article => user.Goal.Any(goal => goal.GoalTypeId == article.GoalTypeId))
                .ToList();

            var remainingArticles = articleDtos
                .Where(article => !user.Goal.Any(goal => goal.GoalTypeId == article.GoalTypeId))
                .ToList();

            // Combine the preferred articles and remaining articles
            var finalArticles = preferredArticles.Concat(remainingArticles).ToList();

            return Ok(finalArticles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


}
