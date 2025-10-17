using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Todo.Api.Filters;

public class ValidationExceptionFilter : IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ValidationExceptionFilter(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException vex)
        {
            var modelState = new ModelStateDictionary();
            foreach (var failure in vex.Errors)
            {
                modelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }

            var details = _problemDetailsFactory.CreateValidationProblemDetails(
                context.HttpContext,
                modelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation failed");

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
