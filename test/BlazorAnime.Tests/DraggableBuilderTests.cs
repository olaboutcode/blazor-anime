using Xunit;

namespace BlazorAnime.Tests;

public sealed class DraggableBuilderTests
{
    [Fact]
    public void Empty_OmitsBothAxes()
    {
        var built = new DraggableBuilder().Build();

        Assert.Empty(built);
    }

    [Fact]
    public void Axes_StoreBoolsAndOnlyTheFieldsThatWereSet()
    {
        var built = new DraggableBuilder()
            .X(false)
            .Y(axis => axis
                .MapTo("top")
                .ModifierSnap(8)
                .Composition(DragComposition.Blend)
                .Snap(new[] { 0d, 40, 80 }))
            .Build();

        Assert.Equal(false, built["x"]);
        var y = Assert.IsType<Dictionary<string, object>>(built["y"]);
        Assert.Equal("top", y["mapTo"]);
        Assert.Equal("blend", y["composition"]);
        Assert.Equal(new[] { 0d, 40, 80 }, y["snap"]);
        var snap = Assert.IsType<Dictionary<string, object>>(y["modifier"]);
        Assert.Equal("snap", snap["fn"]);
        Assert.Equal(8d, snap["increment"]);
        Assert.False(y.ContainsKey("duration"));
    }

    [Fact]
    public void Settings_KeepContainerCursorThresholdAndReleaseEase()
    {
        var built = new DraggableBuilder()
            .Container(0, 200, 120, 0)
            .ContainerPadding(8, 4, 8, 4)
            .ContainerFriction(0.5)
            .Cursor("grab", "grabbing")
            .DragThreshold(2, 6)
            .ReleaseEase(Easing.Spring(1, 80, 20, 0))
            .Snap(24)
            .ModifierInvert()
            .Build();

        Assert.Equal(new[] { 0d, 200, 120, 0 }, built["container"]);
        Assert.Equal(new[] { 8d, 4, 8, 4 }, built["containerPadding"]);
        Assert.Equal(0.5, built["containerFriction"]);
        Assert.Equal(24d, built["snap"]);
        var cursor = Assert.IsType<Dictionary<string, object>>(built["cursor"]);
        Assert.Equal("grab", cursor["onHover"]);
        Assert.Equal("grabbing", cursor["onGrab"]);
        var threshold = Assert.IsType<Dictionary<string, object>>(built["dragThreshold"]);
        Assert.Equal(2d, threshold["mouse"]);
        Assert.Equal(6d, threshold["touch"]);
        var ease = Assert.IsType<Dictionary<string, object>>(built["releaseEase"]);
        Assert.Equal("easeFn", ease["propType"]);
        var payload = Assert.IsType<Dictionary<string, object>>(ease["value"]);
        Assert.Equal("spring", payload["fn"]);
        Assert.Equal(80d, payload["stiffness"]);
        var invert = Assert.IsType<Dictionary<string, object>>(built["modifier"]);
        Assert.Equal("invert", invert["fn"]);
    }

    [Fact]
    public void Snap_RejectsZeroAndEmptyPoints()
    {
        var builder = new DraggableBuilder();
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.Snap(0));
        Assert.Throws<ArgumentException>(() => builder.Snap(Array.Empty<double>()));
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.Y(axis => axis.ModifierSnap(0)));
        Assert.Throws<ArgumentException>(() => builder.Cursor(null, null));
    }

    [Fact]
    public void OnGrab_RegistersOneCallback()
    {
        var builder = new DraggableBuilder().OnGrab(_ => { });

        Assert.Single(builder.Callbacks);
        Assert.Equal("callback", Assert.IsType<Dictionary<string, object>>(builder.Build()["onGrab"])["propType"]);
        foreach (var handle in builder.Callbacks)
            handle.Dispose();
    }
}
