using System.Text.Json;
using Xunit;

namespace BlazorAnime.Tests;

public sealed class AnimatableBuilderTests
{
    [Fact]
    public void Property_Duration_IsThePropertyValue()
    {
        var built = new AnimatableBuilder()
            .Property("x", 500)
            .Property("y", 500)
            .Ease(Easing.Out(3))
            .Build();

        Assert.Equal(500, built["x"]);
        Assert.Equal(500, built["y"]);
        Assert.Equal("out(3)", built["ease"]);
    }

    [Fact]
    public void Property_Settings_KeepUnitDurationAndEase()
    {
        var built = new AnimatableBuilder()
            .Property("x", property => property.Unit("rem").Duration(400).Ease(Easing.Out(4)))
            .Property("rotate", 1000)
            .Ease(Easing.Out(2))
            .Duration(800)
            .Build();

        var x = Assert.IsType<Dictionary<string, object>>(built["x"]);
        Assert.Equal("rem", x["unit"]);
        Assert.Equal(400, x["duration"]);
        Assert.Equal("out(4)", x["ease"]);
        Assert.Equal(1000, built["rotate"]);
        Assert.Equal("out(2)", built["ease"]);
        Assert.Equal(800, built["duration"]);
    }

    [Fact]
    public void Ease_Spring_IsAFunctionMarker()
    {
        var built = new AnimatableBuilder()
            .Property("x", 100)
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
    public void Property_Stagger_CarriesFromAndStart()
    {
        var built = new AnimatableBuilder()
            .Property("x", Stagger.Create(50, options => options.From(StaggerPosition.Center).Start(100)))
            .Build();

        var marker = Assert.IsType<Dictionary<string, object>>(built["x"]);
        Assert.Equal("stagger", marker["propType"]);
        Assert.Equal(50d, marker["value"]);
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(marker["options"]));
        Assert.Equal("center", json.RootElement.GetProperty("from").GetProperty("value").GetProperty("value").GetString());
        Assert.Equal(100, json.RootElement.GetProperty("start").GetProperty("value").GetProperty("value").GetDouble());
    }

    [Fact]
    public void Property_RejectsSettingNames()
    {
        var builder = new AnimatableBuilder();
        Assert.Throws<ArgumentException>(() => builder.Property("ease", 100));
        Assert.Throws<ArgumentException>(() => builder.Property("onUpdate", 100));
    }

    [Fact]
    public void OnUpdate_RegistersOneCallback()
    {
        var builder = new AnimatableBuilder()
            .Property("x", 100)
            .OnUpdate(_ => { });

        Assert.Single(builder.Callbacks);
        Assert.Equal("callback", Assert.IsType<Dictionary<string, object>>(builder.Build()["onUpdate"])["propType"]);
        foreach (var handle in builder.Callbacks)
            handle.Dispose();
    }
}
