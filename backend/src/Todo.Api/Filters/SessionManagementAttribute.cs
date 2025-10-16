using System.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Todo.Infrastructure.Persistence;

namespace Todo.Api.Filters;

public enum SessionStrategy
{
    ReadOnly,
    Transaction
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class SessionManagementAttribute : ActionFilterAttribute
{
    private readonly Type _contextType;
    private readonly bool _doAsync;
    private readonly IsolationLevel _isolationLevel;
    private readonly SessionStrategy _sessionStrategy;

    public SessionManagementAttribute(Type contextType, SessionStrategy sessionStrategy, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool doAsync = false)
    {
        _contextType = contextType;
        _sessionStrategy = sessionStrategy;
        _isolationLevel = isolationLevel;
        _doAsync = doAsync;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var uowType = typeof(IUnitOfWork<>).MakeGenericType(_contextType);
        var uow = (context.HttpContext.RequestServices.GetService(uowType) as Application.Common.Persistence.IUnitOfWork)!;

        if (_sessionStrategy == SessionStrategy.ReadOnly)
            uow.SetReadOnly();

        if (_doAsync)
            uow.BeginTransactionAsync(_isolationLevel).Wait();
        else
            uow.BeginTransaction(_isolationLevel);

        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var uowType = typeof(IUnitOfWork<>).MakeGenericType(_contextType);
        var uow = (Application.Common.Persistence.IUnitOfWork)context.HttpContext.RequestServices.GetService(uowType)!;
        if (!uow.IsTransactionOpen)
            return;

        try
        {
            if (_sessionStrategy == SessionStrategy.ReadOnly && uow.HasChanges())
            {
                throw new ReadOnlyException("SessionStrategy is ReadOnly but changes were detected");
            }

            if (context.Exception == null)
            {
                if (_doAsync) uow.CommitAsync().Wait(); else uow.Commit();
            }
            else
            {
                if (_doAsync) uow.RollbackAsync().Wait(); else uow.Rollback();
            }
        }
        finally
        {
            base.OnActionExecuted(context);
        }
    }
}

