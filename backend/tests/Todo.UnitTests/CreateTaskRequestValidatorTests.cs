using FluentAssertions;
using Todo.Application.Features.Tasks.Contracts;
using Todo.Application.Features.Tasks.Validators;
using Xunit;

namespace Todo.UnitTests;

public class CreateTaskRequestValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Invalid_when_title_empty_or_whitespace(string title)
    {
        var validator = new CreateTaskRequestValidator();
        var result = validator.Validate(new CreateTaskRequest { Title = title });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTaskRequest.Title));
    }

    [Fact]
    public void Invalid_when_title_exceeds_200_chars()
    {
        var validator = new CreateTaskRequestValidator();
        var req = new CreateTaskRequest { Title = new string('a', 201) };

        var result = validator.Validate(req);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTaskRequest.Title));
    }

    [Fact]
    public void Valid_when_title_ok()
    {
        var validator = new CreateTaskRequestValidator();
        var result = validator.Validate(new CreateTaskRequest { Title = "Buy milk" });

        result.IsValid.Should().BeTrue();
    }
}

