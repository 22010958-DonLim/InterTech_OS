using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Telegram.Bot.Types;

[Route("OSHub/[controller]")]
[ApiController]
public class TasksAPIController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksAPIController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/TasksAPI
    [HttpGet]
    public async Task<ActionResult<string>> GetTasks(int Userid)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.Name);

        var currentDate = DateTime.Now;
        var startOfDay = currentDate.Date; // This ensures we're checking from the start of the day

        // Get the list of tasks that the user has not completed today
        var completedTasksToday = await _context.StrawberryUserTask
            .Where(s => s.UserId == Userid &&
                        s.CompletedDate.HasValue &&
                        s.CompletedDate.Value.Date == startOfDay)
            .Select(s => s.TaskId)
            .ToListAsync();

        // Get all tasks and filter out the ones that have been completed today
        var tasksToShow = await _context.StrawberryTask
            .Include(t => t.StrawberryUserTask)
            .Where(t => !completedTasksToday.Contains(t.TaskId))
            .Select(t => new
            {
                TaskId = t.TaskId,
                TaskDescription = t.TaskDescription,
                PointsReward = t.PointsReward
            })
            .ToListAsync();

        // Configure JsonSerializerOptions to handle object cycles
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            // Add other options as needed
        };

        // Serialize the filtered tasks to JSON
        var jsonTasks = JsonSerializer.Serialize(tasksToShow, jsonOptions);

        // Return the JSON result
        return Content(jsonTasks, "application/json");
    }

    // GET: api/TasksAPI/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StrawberryHub.Models.StrawberryTask>> GetTask(int id)
    {
        var task = await _context.StrawberryTask.FirstOrDefaultAsync(m => m.TaskId == id);

        if (task == null)
        {
            return NotFound();
        }

        return task;
    }

    // PUT: api/TasksAPI/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTask(int id, StrawberryHub.Models.StrawberryTask task)
    {
        if (id != task.TaskId)
        {
            return BadRequest();
        }

        _context.Entry(task).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TaskExists(id))
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

    // POST: api/TasksAPI
    [HttpPost]
    public async Task<ActionResult<StrawberryHub.Models.StrawberryTask>> PostTask(StrawberryHub.Models.StrawberryTask task)
    {
        _context.StrawberryTask.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTask", new { id = task.TaskId }, task);
    }

    // DELETE: api/TasksAPI/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<StrawberryHub.Models.StrawberryTask>> DeleteTask(int id)
    {
        var task = await _context.StrawberryTask.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.StrawberryTask.Remove(task);
        await _context.SaveChangesAsync();

        return task;
    }

    private bool TaskExists(int id)
    {
        return _context.StrawberryTask.Any(e => e.TaskId == id);
    }
}
