using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
    public async Task<ActionResult<string>> GetTasks()
    {
        var tasks = await _context.StrawberryTask.Include(t => t.User).ToListAsync();

        // Configure JsonSerializerOptions to handle object cycles
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            // Add other options as needed
        };

        // Serialize the tasks to JSON
        var jsonTasks = JsonSerializer.Serialize(tasks, jsonOptions);

        // Return the JSON result
        return Content(jsonTasks, "application/json");
    }

    // GET: api/TasksAPI/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StrawberryHub.Models.StrawberryTask>> GetTask(int id)
    {
        var task = await _context.StrawberryTask.Include(t => t.User).FirstOrDefaultAsync(m => m.TaskId == id);

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
