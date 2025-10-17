using FluentValidation;
using Todo.Application.Features.Tasks.Contracts;

namespace Todo.Application.Features.Tasks.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Title is required")
            .Must(title => !string.IsNullOrWhiteSpace(title))
                .WithMessage("Title cannot be whitespace only")
            .MaximumLength(200).WithMessage("Title must be at most 200 characters")
            .Matches(@"^(?=.*\S).{1,200}$").WithMessage("Title must contain non-whitespace characters")
            .Must(t => t.Trim().Length > 0).WithMessage("Title cannot be empty after trimming");
    }
}
