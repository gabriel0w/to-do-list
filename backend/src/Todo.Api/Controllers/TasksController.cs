using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Todo.Application.Tasks;
using Todo.Domain.Entities;
using Todo.Infrastructure.Data;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TodoDbContext _db;
    private readonly IValidator<CreateTaskRequest> _createValidator;

    public TasksController(TodoDbContext db, IValidator<CreateTaskRequest> createValidator)
    {
        _db = db;
        _createValidator = createValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
    {
        var items = await _db.Tasks
            .OrderBy(t => t.IsDone)
            .ThenBy(t => t.OrderIndex)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create([FromBody] CreateTaskRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return ValidationProblem(errors);
        }

        // Pick next order index (append to end)
        var maxOrder = await _db.Tasks.MaxAsync(t => (int?)t.OrderIndex) ?? 0;

        var item = new TaskItem
        {
            Title = request.Title.Trim(),
            IsDone = false,
            OrderIndex = maxOrder + 1,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(item);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetById([FromRoute] int id)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<ActionResult<TaskItem>> ToggleComplete([FromRoute] int id)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();

        item.IsDone = !item.IsDone;
        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();

        _db.Tasks.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
