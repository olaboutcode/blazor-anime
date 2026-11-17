using Xunit;

namespace BlazorAnime.Tests;

public sealed class ScrollObserverBuilderTests
{
    [Fact]
    public void Empty_OmitsSettings()
    {
        var built = new ScrollObserverBuilder().Build();

        Assert.Empty(built);
    }

    [Fact]
    public void Settings_KeepContainerTargetThresholdsAndSync()
    {
        var built = new ScrollObserverBuilder()
            .Container(".scroll-container")
            .Target(".square")
            .Axis("x")
            .Debug(false)
            .Repeat(true)
            .Enter("bottom top")
            .Leave("top", "bottom")
            .Sync("resume pause reverse reset")
            .Build();

        Assert.Equal(".scroll-container", built["container"]);
        Assert.Equal(".square", built["target"]);
        Assert.Equal("x", built["axis"]);
        Assert.Equal(false, built["debug"]);
        Assert.Equal(true, built["repeat"]);
        Assert.Equal("bottom top", built["enter"]);
        var leave = Assert.IsType<Dictionary<string, string>>(built["leave"]);
        Assert.Equal("top", leave["target"]);
        Assert.Equal("bottom", leave["container"]);
        Assert.Equal("resume pause reverse reset", built["sync"]);
    }

    [Fact]
    public void Sync_KeepsProgressSmoothAndEase()
    {
        Assert.Equal(true, new ScrollObserverBuilder().Sync(true).Build()["sync"]);
        Assert.Equal(0.5, new ScrollObserverBuilder().Sync(0.5).Build()["sync"]);
        Assert.Equal("out(4)", new ScrollObserverBuilder().Sync(Easing.Out(4)).Build()["sync"]);
    }

    [Fact]
    public void AxisAndSync_RejectUnknownValues()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ScrollObserverBuilder().Axis("z"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ScrollObserverBuilder().Sync("spin"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ScrollObserverBuilder().Sync(-1));
    }
}
