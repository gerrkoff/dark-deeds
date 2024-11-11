using System.Diagnostics.CodeAnalysis;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Details.Services;
using Xunit;

namespace DD.Tests.Unit.SharedDetails.Common;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public class TaskPrinterTests
{
    [Fact]
    public void PrintWithSymbolCodes_ReturnTitle()
    {
        var service = new TaskPrinter();

        var result = service.PrintWithSymbolCodes(new TaskDto { Title = "Task text" });

        Assert.Equal("Task text", result);
    }

    [Fact]
    public void PrintWithSymbolCodes_ReturnTime()
    {
        var service = new TaskPrinter();

        var result = service.PrintWithSymbolCodes(new TaskDto
        {
            Title = "Task",
            Date = new DateTime(2000, 10, 10, 0, 0, 0),
            Time = 1060,
        });

        Assert.Equal("17:40 Task", result);
    }

    [Fact]
    public void PrintWithSymbolCodes_ReturnProbable()
    {
        var service = new TaskPrinter();

        var result = service.PrintWithSymbolCodes(new TaskDto
        {
            Title = "Task text",
            IsProbable = true,
        });

        Assert.Equal("Task text ?", result);
    }

    [Fact]
    public void PrintWithSymbolCodes_ReturnAdditional()
    {
        var service = new TaskPrinter();

        var result = service.PrintWithSymbolCodes(new TaskDto
        {
            Title = "Task text",
            Type = TaskTypeDto.Additional,
        });

        Assert.Equal("Task text !", result);
    }

    [Fact]
    public void PrintWithSymbolCodes_ReturnRoutine()
    {
        var service = new TaskPrinter();

        var result = service.PrintWithSymbolCodes(new TaskDto
        {
            Title = "Task text",
            Type = TaskTypeDto.Routine,
        });

        Assert.Equal("Task text *", result);
    }

    [Fact]
    public void PrintWithSymbolCodes_ReturnRoutineAndProbable()
    {
        var service = new TaskPrinter();

        var result = service.PrintWithSymbolCodes(new TaskDto
        {
            Title = "Task text",
            Type = TaskTypeDto.Routine,
            IsProbable = true,
        });

        Assert.Equal("Task text *?", result);
    }

    [Fact]
    public void PrintWithSymbolCodes_ReturnAdditionalAndProbable()
    {
        var service = new TaskPrinter();

        var result = service.PrintWithSymbolCodes(new TaskDto
        {
            Title = "Task text",
            Type = TaskTypeDto.Additional,
            IsProbable = true,
        });

        Assert.Equal("Task text !?", result);
    }

    [Fact]
    public void PrintContent_WithTime()
    {
        var service = new TaskPrinter();

        var result = service.PrintContent(new TaskDto
        {
            Title = "Task text",
            Time = 60,
            Type = TaskTypeDto.Routine,
            IsProbable = true,
        });

        Assert.Equal("01:00 Task text", result);
    }

    [Fact]
    public void PrintContent_WithNoTime()
    {
        var service = new TaskPrinter();

        var result = service.PrintContent(new TaskDto
        {
            Title = "Task text",
        });

        Assert.Equal("Task text", result);
    }
}
