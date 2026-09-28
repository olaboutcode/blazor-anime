using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace BlazorAnime.Tests;

public sealed class PropsBuilderTests
{
    [Fact]
    public void TranslateX_Number_IsASetter()
    {
        using var root = new Props();
        var built = root.Build(builder => builder
            .Targets(".box")
            .TranslateX(120)
            .Duration(400)
            .Easing(Easing.Linear)
            .AutoPlay(false));

        Assert.Equal([".box"], Strings(Setter(built, "targets")));
        Assert.Equal(120, Setter(built, "translateX").GetDouble());
        Assert.Equal(400, Setter(built, "duration").GetInt32());
        Assert.Equal("linear", Setter(built, "easing").GetString());
        Assert.False(Setter(built, "autoplay").GetBoolean());
    }

    [Fact]
    public void TranslateX_AcceptsUnitsAndRelativeValues()
    {
        using var root = new Props();
        var built = root.Build(builder => builder
            .TranslateX("1turn")
            .Rotate(Relative.Add("2turn"))
            .TranslateY(Relative.Subtract(20)));

        Assert.Equal("1turn", Setter(built, "translateX").GetString());
        Assert.Equal("+=2turn", Setter(built, "rotate").GetString());
        Assert.Equal("-=20", Setter(built, "translateY").GetString());
    }

    [Fact]
    public void TranslateX_FromTo_IsAnArray()
    {
        using var root = new Props();
        var built = root.Build(builder => builder.TranslateX(0, 250));
        var values = Setter(built, "translateX").EnumerateArray().Select(item => item.GetDouble()).ToArray();

        Assert.Equal([0, 250], values);
    }

    [Fact]
    public void PropertyParameters_StayNestedUntilJavaScriptUnwrapsThem()
    {
        using var root = new Props();
        var built = root.Build(builder => builder.TranslateX(parameter => parameter
            .Value(250)
            .Duration(800)
            .Easing(Easing.EaseInOutQuad)));

        var parameter = Setter(built, "translateX");
        Assert.Equal(250, Setter(parameter, "value").GetDouble());
        Assert.Equal(800, Setter(parameter, "duration").GetInt32());
        Assert.Equal("easeInOutQuad", Setter(parameter, "easing").GetString());
    }

    [Fact]
    public void Keyframes_KeepEachFrame()
    {
        using var root = new Props();
        var built = root.Build(builder => builder.Keyframes([
            frame => frame.TranslateY(-40),
            frame => frame.TranslateX(250)
        ]));

        var frames = Setter(built, "keyframes").EnumerateArray().ToArray();
        Assert.Equal(-40, Setter(frames[0], "translateY").GetDouble());
        Assert.Equal(250, Setter(frames[1], "translateX").GetDouble());
    }

    [Fact]
    public void Stagger_GridIsColumnsThenRows()
    {
        using var root = new Props();
        var built = root.Build(builder => builder.Delay(stagger => stagger
            .Value(100)
            .Start(500)
            .From(2)
            .Grid([14, 7])
            .Axis(StaggerAxis.X)
            .Direction(Direction.Reverse)
            .Easing(Easing.EaseOutQuad)));

        var description = StaggerValue(built, "delay");
        Assert.Equal(100, description.GetProperty("value").GetDouble());

        var options = description.GetProperty("options");
        Assert.Equal(500, Option(options, "start").GetDouble());
        Assert.Equal(2, Option(options, "from").GetInt32());
        Assert.Equal([14, 7], Option(options, "grid").EnumerateArray().Select(item => item.GetInt32()).ToArray());
        Assert.Equal("x", Option(options, "axis").GetString());
        Assert.Equal("reverse", Option(options, "direction").GetString());
        Assert.Equal("easeOutQuad", Option(options, "easing").GetString());
    }

    [Fact]
    public void Callbacks_UseTheRelayAndCanBeDisposed()
    {
        var host = new CallbackHost();
        using var root = new Props();
        var built = root.Build(builder => builder
            .Delay(host.Delay)
            .Update(host.OnUpdate));

        var delay = built.GetProperty("delay").GetProperty("value");
        Assert.Equal("callback", delay.GetProperty("propType").GetString());
        Assert.Equal(2, delay.GetProperty("paramCount").GetInt32());
        Assert.Equal("InvokeAll", delay.GetProperty("value").GetProperty("callback").GetString());

        var update = built.GetProperty("update").GetProperty("value");
        Assert.Equal("callback", update.GetProperty("propType").GetString());
        Assert.Equal(1, update.GetProperty("paramCount").GetInt32());
        Assert.Equal("Invoke", update.GetProperty("value").GetProperty("callback").GetString());
    }

    [Fact]
    public void Loop_TrueAndCount_KeepTheirJsonKinds()
    {
        using var infinite = new Props();
        using var counted = new Props();

        Assert.True(Setter(infinite.Build(builder => builder.Loop(true)), "loop").GetBoolean());
        Assert.Equal(3, Setter(counted.Build(builder => builder.Loop(3)), "loop").GetInt32());
    }

    private static JsonElement Setter(JsonElement owner, string name)
    {
        var property = owner.GetProperty(name);
        Assert.Equal(name, property.GetProperty("name").GetString());
        var wrapped = property.GetProperty("value");
        Assert.Equal("setter", wrapped.GetProperty("propType").GetString());
        return wrapped.GetProperty("value");
    }

    private static JsonElement StaggerValue(JsonElement owner, string name)
    {
        var wrapped = owner.GetProperty(name).GetProperty("value");
        Assert.Equal("stagger", wrapped.GetProperty("propType").GetString());
        return wrapped.GetProperty("value");
    }

    private static JsonElement Option(JsonElement options, string name)
    {
        return Setter(options, name);
    }

    private static string[] Strings(JsonElement array) =>
        array.EnumerateArray().Select(item => item.GetString()!).ToArray();

    private sealed class CallbackHost
    {
        public double Delay(int index, int total) => index * 25;

        public void OnUpdate(AnimationState state)
        {
        }
    }

    private sealed class Props : IDisposable
    {
        private readonly List<PropsBuilder> _builders = [];

        public JsonElement Build(Func<PropsBuilder, PropsBuilder> configure)
        {
            var builder = configure(new PropsBuilder());
            _builders.Add(builder);
            var json = JsonSerializer.Serialize(builder.Build().ToObject(), new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
            return JsonDocument.Parse(json).RootElement.Clone();
        }

        public void Dispose()
        {
            foreach (var builder in _builders)
            {
                foreach (var handle in builder.CallbackHandles)
                    handle.Dispose();
            }
        }
    }
}
