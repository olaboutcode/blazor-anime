using Xunit;

namespace BlazorAnime.Tests;

public sealed class TextBuilderTests
{
    [Fact]
    public void Empty_OmitsWordsAndTheOtherKeys()
    {
        var built = new TextSplitBuilder().Build();

        Assert.Empty(built);
    }

    [Fact]
    public void WordsFalse_IsSentAndCharsTrueStaysTrue()
    {
        var built = new TextSplitBuilder()
            .Words(false)
            .Chars(true)
            .Lines(false)
            .IncludeSpaces(true)
            .Accessible(false)
            .Debug(true)
            .Build();

        Assert.Equal(false, built["words"]);
        Assert.Equal(true, built["chars"]);
        Assert.Equal(false, built["lines"]);
        Assert.Equal(true, built["includeSpaces"]);
        Assert.Equal(false, built["accessible"]);
        Assert.Equal(true, built["debug"]);
    }

    [Fact]
    public void Part_KeepsClassWrapAndClone()
    {
        var built = new TextSplitBuilder()
            .Words(part => part.Class("split-word").Wrap("clip").Clone("top"))
            .Lines("<span data-line=\"{i}\">{value}</span>")
            .Build();

        var words = Assert.IsType<Dictionary<string, object>>(built["words"]);
        Assert.Equal("split-word", words["class"]);
        Assert.Equal("clip", words["wrap"]);
        Assert.Equal("top", words["clone"]);
        Assert.Equal("<span data-line=\"{i}\">{value}</span>", built["lines"]);
        Assert.False(built.ContainsKey("chars"));
    }

    [Fact]
    public void ClassFalse_IsSent()
    {
        var built = new TextSplitBuilder()
            .Chars(part => part.Class(false).Wrap(true).Clone(false))
            .Build();

        var chars = Assert.IsType<Dictionary<string, object>>(built["chars"]);
        Assert.Equal(false, chars["class"]);
        Assert.Equal(true, chars["wrap"]);
        Assert.Equal(false, chars["clone"]);
    }

    [Theory]
    [InlineData("blend")]
    [InlineData("none")]
    public void Wrap_RejectsUnknownValues(string wrap)
    {
        var builder = new TextSplitBuilder();
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.Words(part => part.Wrap(wrap)));
    }

    [Fact]
    public void Template_RequiresValueToken()
    {
        Assert.Throws<ArgumentException>(() => new TextSplitBuilder().Words("<span></span>"));
    }

    [Fact]
    public void EmptyPart_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TextSplitBuilder().Words(_ => { }));
    }

    [Fact]
    public void Scramble_OmitsOverrideAndKeepsThePassedSettings()
    {
        var built = new TextScrambleBuilder()
            .Text("Hello")
            .Chars("uppercase")
            .Ease(Easing.Out(3))
            .From("center")
            .Reversed(true)
            .Cursor("_")
            .Perturbation(0.5)
            .Seed(42)
            .RevealRate(60)
            .SettleDuration(300)
            .SettleRate(30)
            .Duration(800)
            .RevealDelay(20)
            .Delay(10)
            .Build();

        Assert.Equal("Hello", built["text"]);
        Assert.Equal("uppercase", built["chars"]);
        Assert.Equal("out(3)", built["ease"]);
        Assert.Equal("center", built["from"]);
        Assert.Equal(true, built["reversed"]);
        Assert.Equal("_", built["cursor"]);
        Assert.Equal(0.5, built["perturbation"]);
        Assert.Equal(42d, built["seed"]);
        Assert.Equal(60d, built["revealRate"]);
        Assert.Equal(300, built["settleDuration"]);
        Assert.Equal(30d, built["settleRate"]);
        Assert.Equal(800, built["duration"]);
        Assert.Equal(20, built["revealDelay"]);
        Assert.Equal(10, built["delay"]);
        Assert.False(built.ContainsKey("override"));
    }

    [Fact]
    public void Scramble_OverrideFalseAndSpringMarker()
    {
        var built = new TextScrambleBuilder()
            .Override(false)
            .Ease(Easing.Spring(1, 80, 10, 0))
            .From(2)
            .Cursor(95)
            .Build();

        Assert.Equal(false, built["override"]);
        Assert.Equal(2, built["from"]);
        Assert.Equal(95, built["cursor"]);
        var ease = Assert.IsType<Dictionary<string, object>>(built["ease"]);
        Assert.Equal("easeFn", ease["propType"]);
    }

    [Fact]
    public void Scramble_RejectsPerturbationAndFrom()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TextScrambleBuilder().Perturbation(1.1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TextScrambleBuilder().From("middle"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TextScrambleBuilder().RevealRate(0));
    }

    [Fact]
    public void OnChange_StoresTheRelay()
    {
        var builder = new TextScrambleBuilder().OnChange((_, _) => { });
        var built = builder.Build();

        Assert.NotNull(built["onChange"]);
        Assert.Single(builder.Callbacks);
        foreach (var callback in builder.Callbacks)
            callback.Dispose();
    }
}
