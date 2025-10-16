using FluentValidation;

namespace Todo.Application.Tasks;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Title is required")
            .Must(title => !string.IsNullOrWhiteSpace(title))
                .WithMessage("Title cannot be whitespace only")
            .MaximumLength(200).WithMessage("Title must be at most 200 characters");
    }
}

