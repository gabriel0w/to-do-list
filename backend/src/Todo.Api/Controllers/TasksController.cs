using Microsoft.AspNetCore.Mvc;
using Todo.Application.Tasks;
using Todo.Domain.Entities;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
        => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create([FromBody] CreateTaskRequest request)
    {
        try
        {
            var item = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return Problem(title: "Validation failed", detail: ex.Message, statusCode: 400);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetById([FromRoute] int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<ActionResult<TaskItem>> ToggleComplete([FromRoute] int id)
    {
        var item = await _service.ToggleCompleteAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var removed = await _service.DeleteAsync(id);
        if (!removed) return NotFound();
        return NoContent();
    }
}
