using Xunit;

namespace BlazorAnime.Tests;

public sealed class ScopeBuilderTests
{
    [Fact]
    public void Empty_OmitsRootDefaultsAndMediaQueries()
    {
        var built = new ScopeBuilder().Build();

        Assert.Empty(built);
    }

    [Fact]
    public void Settings_KeepRootMediaQueriesAndValueDefaults()
    {
        var built = new ScopeBuilder()
            .Root(".row")
            .MediaQuery("isSmall", "(max-width: 200px)")
            .MediaQuery("reduceMotion", "(prefers-reduced-motion)")
            .Defaults(defaults => defaults
                .Duration(250)
                .Delay(10)
                .Loop(true)
                .LoopDelay(20)
                .Reversed(true)
                .Alternate(true)
                .Autoplay(false)
                .PlaybackRate(1.5)
                .FrameRate(30)
                .Ease(Easing.Out(4))
                .PlaybackEase(Easing.Linear)
                .Composition("replace"))
            .Build();

        Assert.Equal(".row", built["root"]);
        var queries = Assert.IsType<Dictionary<string, string>>(built["mediaQueries"]);
        Assert.Equal("(max-width: 200px)", queries["isSmall"]);
        Assert.Equal("(prefers-reduced-motion)", queries["reduceMotion"]);

        var defaults = Assert.IsType<Dictionary<string, object>>(built["defaults"]);
        Assert.Equal(250, defaults["duration"]);
        Assert.Equal(10, defaults["delay"]);
        Assert.Equal(true, defaults["loop"]);
        Assert.Equal(20, defaults["loopDelay"]);
        Assert.Equal(true, defaults["reversed"]);
        Assert.Equal(true, defaults["alternate"]);
        Assert.Equal(false, defaults["autoplay"]);
        Assert.Equal(1.5, defaults["playbackRate"]);
        Assert.Equal(30d, defaults["frameRate"]);
        Assert.Equal("out(4)", defaults["ease"]);
        Assert.Equal("linear", defaults["playbackEase"]);
        Assert.Equal("replace", defaults["composition"]);
        Assert.False(defaults.ContainsKey("modifier"));
    }

    [Fact]
    public void Composition_RejectsAFunctionStandIn()
    {
        var builder = new ScopeDefaultsBuilder();

        Assert.Throws<ArgumentOutOfRangeException>(() => builder.Composition("none "));
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.Composition("add"));
    }
}
