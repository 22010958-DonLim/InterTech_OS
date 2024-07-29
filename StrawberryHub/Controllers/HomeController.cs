using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Diagnostics;

namespace StrawberryHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
			_context = context;
		}

        public async Task<IActionResult> Index()
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.Name);
            var userId = 0;
            if (userIdClaim != null)
            {
                string username = userIdClaim.Value;
                userId = await _context.StrawberryUser
                            .Where(u => u.Username == username)
                            .Select(u => u.UserId)
                            .FirstOrDefaultAsync();

                var currentDate = DateTime.Now;
                var startOfDay = currentDate.Date; // This ensures we're checking from the start of the day

                // Get the list of tasks that the user has not completed today
                var completedTasksToday = await _context.StrawberryUserTask
                    .Where(s => s.UserId == userId &&
                                s.CompletedDate.HasValue &&
                                s.CompletedDate.Value.Date == startOfDay)
                    .Select(s => s.TaskId)
                    .ToListAsync();

                // Get all tasks and filter out the ones that have been completed today
                var tasksToShow = await _context.StrawberryTask
                    .Where(t => !completedTasksToday.Contains(t.TaskId))
                    .ToListAsync();

                // Fetch the top 5 earliest articles created by Admin users
                var topAdminArticles = await _context.StrawberryArticle
                    .Include(a => a.StrawberryUser)
                    .Where(a => a.StrawberryUser.UserRole == "Admin")
					.OrderByDescending(a => a.PublishedDate) // Order by creation date descending
					.Take(8) // Take the top 5 earliest articles
                    .ToListAsync();

                // Pass tasks and articles to the view
                ViewBag.TasksToShow = tasksToShow;
                ViewBag.ShowDailyTaskButton = true;
                ViewBag.TopAdminArticles = topAdminArticles ?? new List<StrawberryArticle>();

                return View();
            }

            return View();
        }

        public IActionResult About() // Add this method
		{
			return View();
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}