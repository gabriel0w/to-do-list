using Microsoft.AspNetCore.Mvc;
using System.Data;
using Todo.Api.Filters;
using Todo.Application.Features.Tasks.Contracts;
using Todo.Application.Features.Tasks.Interfaces;
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
    [SessionManagement(typeof(Todo.Infrastructure.Persistence.Db.TodoDbContext), SessionStrategy.ReadOnly, IsolationLevel.ReadCommitted)]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll([FromQuery] TaskListFilter? filter)
    {
        var items = filter is null ?
            await _service.GetAllAsync() :
            await _service.GetAllAsync(filter);
        return Ok(items);
    }

    [HttpPost]
    [SessionManagement(typeof(Todo.Infrastructure.Persistence.Db.TodoDbContext), SessionStrategy.Transaction, IsolationLevel.ReadCommitted)]
    public async Task<ActionResult<TaskItem>> Create([FromBody] CreateTaskRequest request)
    {
        var item = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet("{id:int}")]
    [SessionManagement(typeof(Todo.Infrastructure.Persistence.Db.TodoDbContext), SessionStrategy.ReadOnly, IsolationLevel.ReadCommitted)]
    public async Task<ActionResult<TaskItem>> GetById([FromRoute] int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPatch("{id:int}/complete")]
    [SessionManagement(typeof(Todo.Infrastructure.Persistence.Db.TodoDbContext), SessionStrategy.Transaction, IsolationLevel.ReadCommitted)]
    public async Task<ActionResult<TaskItem>> ToggleComplete([FromRoute] int id)
    {
        var item = await _service.ToggleCompleteAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    [SessionManagement(typeof(Todo.Infrastructure.Persistence.Db.TodoDbContext), SessionStrategy.Transaction, IsolationLevel.ReadCommitted)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var removed = await _service.DeleteAsync(id);
        if (!removed) return NotFound();
        return NoContent();
    }

    [HttpPut("reorder")]
    [SessionManagement(typeof(Todo.Infrastructure.Persistence.Db.TodoDbContext), SessionStrategy.Transaction, IsolationLevel.ReadCommitted)]
    public async Task<IActionResult> Reorder([FromBody] ReorderTasksRequest request)
    {
        await _service.ReorderAsync(request);
        return NoContent();
    }
}
