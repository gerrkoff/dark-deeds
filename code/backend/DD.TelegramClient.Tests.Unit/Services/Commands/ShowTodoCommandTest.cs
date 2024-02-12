using System.Diagnostics.CodeAnalysis;
using DD.TelegramClient.Domain.Models.Commands;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services.Commands;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public class ShowTodoCommandTest
{
    [Fact]
    public void Constructor_NoArgs()
    {
        var result = new ShowTodoCommand(" ", new DateTime(2010, 10, 10, 10, 10, 10), 0);

        Assert.Equal(new DateTime(2010, 10, 10), result.From);
        Assert.Equal(new DateTime(2010, 10, 11), result.To);
    }

    [Fact]
    public void Constructor_NoArgsWithAdjustment()
    {
        var result = new ShowTodoCommand(" ", new DateTime(2010, 10, 11, 0, 1, 0), -2);

        Assert.Equal(new DateTime(2010, 10, 10), result.From);
        Assert.Equal(new DateTime(2010, 10, 11), result.To);
    }

    [Fact]
    public void Constructor_4SymbolArgs()
    {
        var result = new ShowTodoCommand("1231", new DateTime(2019, 1, 1), 0);

        Assert.Equal(new DateTime(2019, 12, 31), result.From);
        Assert.Equal(new DateTime(2020, 1, 1), result.To);
    }

    [Fact]
    public void Constructor_4SymbolArgsWithAdjustment()
    {
        var result = new ShowTodoCommand("1231", new DateTime(2019, 1, 1), -1);

        Assert.Equal(new DateTime(2018, 12, 31), result.From);
        Assert.Equal(new DateTime(2019, 1, 1), result.To);
    }

    [Fact]
    public void Constructor_8SymbolArgs()
    {
        var result = new ShowTodoCommand("20301231", new DateTime(), 0);

        Assert.Equal(new DateTime(2030, 12, 31), result.From);
        Assert.Equal(new DateTime(2031, 1, 1), result.To);
    }
}
