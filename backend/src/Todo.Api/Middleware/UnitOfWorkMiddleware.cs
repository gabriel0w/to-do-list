using System.Data;
using Todo.Application.Common.Persistence;
using Todo.Infrastructure.Data;

namespace Todo.Api.Middleware;

public class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;

    public UnitOfWorkMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork<TodoDbContext> uow)
    {
        await uow.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            await _next(context);

            if (context.Response.StatusCode < 400)
            {
                await uow.CommitAsync();
            }
            else
            {
                await uow.RollbackAsync();
            }
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }
}

