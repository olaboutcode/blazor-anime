using Xunit;

namespace BlazorAnime.Tests;

public sealed class LayoutBuilderTests
{
    [Fact]
    public void Empty_OmitsChildrenTimingsAndStates()
    {
        var built = new LayoutBuilder().Build();

        Assert.Empty(built);
    }

    [Fact]
    public void Settings_KeepChildrenEaseStaggerPropertiesAndStates()
    {
        var built = new LayoutBuilder()
            .Children(new[] { ".a", ".b" })
            .Duration(350)
            .Delay(Stagger.Create(40))
            .Ease(Easing.InOut(3.5))
            .Properties("boxShadow")
            .EnterFrom(state => state.Opacity(0).Duration(200))
            .LeaveTo(state => state.Property("opacity", "0").Ease(Easing.Linear))
            .SwapAt(state => state.Opacity(0.2))
            .Build();

        Assert.Equal(new[] { ".a", ".b" }, Assert.IsType<string[]>(built["children"]));
        Assert.Equal(350, built["duration"]);
        Assert.Equal("inOut(3.5)", built["ease"]);
        Assert.Equal(new[] { "boxShadow" }, Assert.IsType<string[]>(built["properties"]));
        Assert.False(built.ContainsKey("composition"));

        var delay = Assert.IsType<Dictionary<string, object>>(built["delay"]);
        Assert.Equal("stagger", delay["propType"]);
        Assert.Equal(40d, delay["value"]);
        Assert.Empty(Assert.IsType<Dictionary<string, object>>(delay["options"]));

        var enter = Assert.IsType<Dictionary<string, object>>(built["enterFrom"]);
        Assert.Equal(0d, enter["opacity"]);
        Assert.Equal(200, enter["duration"]);
        Assert.False(enter.ContainsKey("ease"));

        var leave = Assert.IsType<Dictionary<string, object>>(built["leaveTo"]);
        Assert.Equal("0", leave["opacity"]);
        Assert.Equal("linear", leave["ease"]);

        var swap = Assert.IsType<Dictionary<string, object>>(built["swapAt"]);
        Assert.Equal(0.2, swap["opacity"]);
        Assert.False(swap.ContainsKey("duration"));
    }

    [Fact]
    public void Playback_StoresOverridesWithoutComposition()
    {
        var built = new LayoutBuilder()
            .Id("grid")
            .Loop(true)
            .Alternate(true)
            .Reversed(true)
            .LoopDelay(20)
            .Autoplay(false)
            .PlaybackEase(Easing.Spring())
            .Build();

        Assert.Equal("grid", built["id"]);
        Assert.Equal(true, built["loop"]);
        Assert.Equal(true, built["alternate"]);
        Assert.Equal(true, built["reversed"]);
        Assert.Equal(20, built["loopDelay"]);
        Assert.Equal(false, built["autoplay"]);
        var ease = Assert.IsType<Dictionary<string, object>>(built["playbackEase"]);
        Assert.Equal("easeFn", ease["propType"]);
        var payload = Assert.IsType<Dictionary<string, object>>(ease["value"]);
        Assert.Equal("spring", payload["fn"]);
        Assert.False(built.ContainsKey("composition"));
    }

    [Fact]
    public void Settings_RejectEmptyAndNegativeValues()
    {
        var builder = new LayoutBuilder();
        Assert.Throws<ArgumentException>(() => builder.Children(" "));
        Assert.Throws<ArgumentException>(() => builder.Children(Array.Empty<string>()));
        Assert.Throws<ArgumentException>(() => builder.Properties());
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.Duration(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.LoopDelay(-1));
        Assert.Throws<ArgumentException>(() => builder.EnterFrom(static _ => { }));
        Assert.Throws<ArgumentException>(() => builder.LeaveTo(static state => { state.Property("ease", 1); }));
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.SwapAt(static state => state.Opacity(double.NaN)));
    }

    [Fact]
    public void OnBegin_RegistersOneCallback()
    {
        var builder = new LayoutBuilder().OnBegin(static _ => { });
        var built = builder.Build();

        var marker = Assert.IsType<Dictionary<string, object>>(built["onBegin"]);
        Assert.Equal("callback", marker["propType"]);
        Assert.NotNull(marker["dotNetRef"]);
        var handle = Assert.Single(builder.Callbacks);
        handle.Dispose();
    }
}
