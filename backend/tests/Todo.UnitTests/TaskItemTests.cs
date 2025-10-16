using FluentAssertions;
using Todo.Domain.Entities;
using Xunit;
using System;


namespace Todo.UnitTests;

public class TaskItemTests
{
    /// <summary>
    /// Verifies that a newly created <see cref="TaskItem"/> initializes with correct default values.
    /// Ensures that Id and OrderIndex start at 0, IsDone is false, and CreatedAt is set
    /// close to the current UTC time (within 5 seconds).
    /// </summary>
    [Fact]
    public void New_TaskItem_Has_Sane_Defaults()
    {
        var t = new TaskItem { Title = "Test" };

        t.Id.Should().Be(0);
        t.IsDone.Should().BeFalse();
        t.OrderIndex.Should().Be(0);
        t.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

