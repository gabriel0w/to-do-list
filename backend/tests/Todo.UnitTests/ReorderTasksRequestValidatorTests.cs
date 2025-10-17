using FluentAssertions;
using Todo.Application.Features.Tasks.Contracts;
using Todo.Application.Features.Tasks.Validators;
using Xunit;

namespace Todo.UnitTests;

public class ReorderTasksRequestValidatorTests
{
    [Fact]
    public void Invalid_when_empty_items()
    {
        var validator = new ReorderTasksRequestValidator();
        var result = validator.Validate(new ReorderTasksRequest { Items = new() });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Invalid_when_duplicate_ids()
    {
        var validator = new ReorderTasksRequestValidator();
        var req = new ReorderTasksRequest
        {
            Items = new() { new ReorderItem { Id = 1, OrderIndex = 1 }, new ReorderItem { Id = 1, OrderIndex = 2 } }
        };
        var result = validator.Validate(req);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Valid_when_distinct_ids_and_nonnegative_order()
    {
        var validator = new ReorderTasksRequestValidator();
        var req = new ReorderTasksRequest
        {
            Items = new() { new ReorderItem { Id = 1, OrderIndex = 2 }, new ReorderItem { Id = 2, OrderIndex = 3 } }
        };
        var result = validator.Validate(req);
        result.IsValid.Should().BeTrue();
    }
}

