using FluentValidation;
using Todo.Application.Features.Tasks.Contracts;

namespace Todo.Application.Features.Tasks.Validators;

public class ReorderTasksRequestValidator : AbstractValidator<ReorderTasksRequest>
{
    public ReorderTasksRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Items are required");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Id).GreaterThan(0);
            item.RuleFor(i => i.OrderIndex).GreaterThanOrEqualTo(0);
        });

        RuleFor(x => x.Items.Select(i => i.Id))
            .Must(ids => ids.Distinct().Count() == ids.Count())
            .WithMessage("Duplicate ids are not allowed");
    }
}

