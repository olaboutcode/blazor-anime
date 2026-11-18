using System.Text.Json;
using Microsoft.JSInterop;
using Xunit;

namespace BlazorAnime.Tests;

public sealed class WaapiAnimationBuilderTests
{
    [Fact]
    public void Empty_OmitsComposition()
    {
        var built = new WaapiAnimationBuilder().Duration(500).Build();

        Assert.Equal(500, built["duration"]);
        Assert.False(built.ContainsKey("composition"));
        Assert.False(built.ContainsKey("loop"));
        Assert.False(built.ContainsKey("autoplay"));
    }

    [Fact]
    public void Playback_KeepsLoopAlternatePersistRateAndEase()
    {
        var handle = new JsHandle();
        var built = new WaapiAnimationBuilder()
            .Loop(true)
            .Alternate(true)
            .Reversed(false)
            .Persist(true)
            .PlaybackRate(1.5)
            .Ease(Easing.Out(3))
            .AutoPlay(handle)
            .Duration(600)
            .Delay(Stagger.Create(100))
            .Build();

        Assert.Equal(true, built["loop"]);
        Assert.Equal(true, built["alternate"]);
        Assert.Equal(false, built["reversed"]);
        Assert.Equal(true, built["persist"]);
        Assert.Equal(1.5, built["playbackRate"]);
        Assert.Equal("out(3)", built["ease"]);
        Assert.Same(handle, built["autoplay"]);
        Assert.Equal(600, built["duration"]);
        var delay = Assert.IsType<Dictionary<string, object>>(built["delay"]);
        Assert.Equal("stagger", delay["propType"]);
        Assert.Equal(100d, delay["value"]);
    }

    [Fact]
    public void LoopCount_StaysTheNumberThatWasPassed()
    {
        Assert.Equal(1, new WaapiAnimationBuilder().Loop(1).Build()["loop"]);
        Assert.Equal(false, new WaapiAnimationBuilder().AutoPlay(false).Build()["autoplay"]);
    }

    [Fact]
    public void Composition_IsReplaceAddOrAccumulate()
    {
        Assert.Equal("add", new WaapiAnimationBuilder().Composition("add").Build()["composition"]);
        Assert.Equal("accumulate", new WaapiAnimationBuilder().Composition("accumulate").Build()["composition"]);
        Assert.Throws<ArgumentOutOfRangeException>(() => new WaapiAnimationBuilder().Composition("none"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new WaapiTween().Composition("blend"));
    }

    [Fact]
    public void Property_KeepsFromToDurationEaseAndComposition()
    {
        var built = new WaapiAnimationBuilder()
            .X(tween => tween.From(0).To("10rem").Duration(400).Ease(Easing.Out(4)).Composition("add"))
            .TranslateX("12rem")
            .Build();

        var x = Assert.IsType<Dictionary<string, object>>(built["x"]);
        Assert.Equal(0d, x["from"]);
        Assert.Equal("10rem", x["to"]);
        Assert.Equal(400, x["duration"]);
        Assert.Equal("out(4)", x["ease"]);
        Assert.Equal("add", x["composition"]);
        Assert.Equal("12rem", built["translateX"]);
        Assert.False(built.ContainsKey("composition"));
    }

    [Fact]
    public void Ease_Spring_IsAFunctionMarker()
    {
        var built = new WaapiAnimationBuilder()
            .X(20)
            .Ease(Easing.Spring(1, 80, 8, 0))
            .Build();

        var ease = Assert.IsType<Dictionary<string, object>>(built["ease"]);
        Assert.Equal("easeFn", ease["propType"]);
        var payload = Assert.IsType<Dictionary<string, object>>(ease["value"]);
        Assert.Equal("spring", payload["fn"]);
        Assert.Equal(1d, payload["mass"]);
        Assert.Equal(80d, payload["stiffness"]);
    }

    [Fact]
    public void Delay_Function_IsAValueCallback()
    {
        var built = new WaapiAnimationBuilder()
            .Delay((index, total) => index * 10d)
            .Property("x", (TargetInfo target) => (double)target.Index)
            .Build();

        var delay = Assert.IsType<Dictionary<string, object>>(built["delay"]);
        Assert.Equal("callback", delay["propType"]);
        Assert.Equal(2, delay["paramCount"]);
        Assert.Equal("InvokeAll", delay["callback"]);
        var x = Assert.IsType<Dictionary<string, object>>(built["x"]);
        Assert.Equal(3, x["paramCount"]);
        Assert.Equal("InvokeTargets", x["callback"]);
    }

    [Fact]
    public void Stagger_KeepsFromAndStart()
    {
        var built = new WaapiAnimationBuilder()
            .Delay(Stagger.Create(50, options => options.From(StaggerPosition.Center).Start(100)))
            .Build();

        var marker = Assert.IsType<Dictionary<string, object>>(built["delay"]);
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(marker["options"]));
        Assert.Equal("center", json.RootElement.GetProperty("from").GetProperty("value").GetProperty("value").GetString());
        Assert.Equal(100, json.RootElement.GetProperty("start").GetProperty("value").GetProperty("value").GetDouble());
    }

    private sealed class JsHandle : IJSObjectReference
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            throw new NotSupportedException();

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            throw new NotSupportedException();

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
