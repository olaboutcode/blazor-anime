using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// Settings for <c>waapi.animate</c>. Property names are the anime.js names,
/// for example <c>x</c>, <c>translateX</c>, or <c>opacity</c>.
/// </summary>
public sealed class WaapiAnimationBuilder
{
    public WaapiAnimationBuilder Duration(int milliseconds)
    {
        _parameters["duration"] = milliseconds;
        return this;
    }

    public WaapiAnimationBuilder Duration(Func<int, int, double> callback) =>
        Value("duration", callback);

    public WaapiAnimationBuilder Duration(Func<TargetInfo, double> callback) =>
        Target("duration", callback);

    public WaapiAnimationBuilder Delay(int milliseconds)
    {
        _parameters["delay"] = milliseconds;
        return this;
    }

    public WaapiAnimationBuilder Delay(Stagger stagger)
    {
        ArgumentNullException.ThrowIfNull(stagger);
        _parameters["delay"] = StaggerMarker(stagger);
        return this;
    }

    public WaapiAnimationBuilder Delay(Func<int, int, double> callback) =>
        Value("delay", callback);

    public WaapiAnimationBuilder Delay(Func<TargetInfo, double> callback) =>
        Target("delay", callback);

    /// <summary><c>true</c> repeats forever. The value is sent as anime.js received it.</summary>
    public WaapiAnimationBuilder Loop(bool loop)
    {
        _parameters["loop"] = loop;
        return this;
    }

    /// <summary>Repeat count. <c>1</c> stays <c>1</c>.</summary>
    public WaapiAnimationBuilder Loop(int count)
    {
        _parameters["loop"] = count;
        return this;
    }

    public WaapiAnimationBuilder Alternate(bool alternate)
    {
        _parameters["alternate"] = alternate;
        return this;
    }

    public WaapiAnimationBuilder Reversed(bool reversed)
    {
        _parameters["reversed"] = reversed;
        return this;
    }

    public WaapiAnimationBuilder Persist(bool persist)
    {
        _parameters["persist"] = persist;
        return this;
    }

    /// <summary>Maps to <c>playbackRate</c> and reads back as <c>Speed</c>.</summary>
    public WaapiAnimationBuilder PlaybackRate(double rate)
    {
        if (!double.IsFinite(rate))
            throw new ArgumentOutOfRangeException(nameof(rate));
        _parameters["playbackRate"] = rate;
        return this;
    }

    /// <summary><c>replace</c>, <c>add</c>, or <c>accumulate</c>. Omitted stays anime's default <c>replace</c>.</summary>
    public WaapiAnimationBuilder Composition(string composition)
    {
        if (composition is not ("replace" or "add" or "accumulate"))
            throw new ArgumentOutOfRangeException(nameof(composition), "Composition is replace, add, or accumulate.");
        _parameters["composition"] = composition;
        return this;
    }

    public WaapiAnimationBuilder AutoPlay(bool autoplay)
    {
        _parameters["autoplay"] = autoplay;
        return this;
    }

    /// <summary>Pass a scroll observer handle. This package does not reference Events.</summary>
    public WaapiAnimationBuilder AutoPlay(IJSObjectReference autoplay)
    {
        ArgumentNullException.ThrowIfNull(autoplay);
        _parameters["autoplay"] = autoplay;
        return this;
    }

    public WaapiAnimationBuilder Ease(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        _parameters["ease"] = EaseMarker(easing);
        return this;
    }

    public WaapiAnimationBuilder OnComplete(Action<WaapiState> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ValidationChecks.EnsureAcceptableCallback(callback);
        var relay = new WaapiCallbackRelay(callback);
        Callbacks.Add(relay);
        _parameters["onComplete"] = relay.Reference;
        return this;
    }

    public WaapiAnimationBuilder X(double value) => Property("x", value);
    public WaapiAnimationBuilder X(string value) => Property("x", value);
    public WaapiAnimationBuilder X(Action<WaapiTween> configure) => Property("x", configure);
    public WaapiAnimationBuilder Y(double value) => Property("y", value);
    public WaapiAnimationBuilder Y(string value) => Property("y", value);
    public WaapiAnimationBuilder Y(Action<WaapiTween> configure) => Property("y", configure);
    public WaapiAnimationBuilder Translate(string value) => Property("translate", value);
    public WaapiAnimationBuilder TranslateX(double value) => Property("translateX", value);
    public WaapiAnimationBuilder TranslateX(string value) => Property("translateX", value);
    public WaapiAnimationBuilder TranslateX(Action<WaapiTween> configure) => Property("translateX", configure);
    public WaapiAnimationBuilder TranslateY(double value) => Property("translateY", value);
    public WaapiAnimationBuilder TranslateY(string value) => Property("translateY", value);
    public WaapiAnimationBuilder Rotate(double value) => Property("rotate", value);
    public WaapiAnimationBuilder Rotate(string value) => Property("rotate", value);
    public WaapiAnimationBuilder Rotate(Action<WaapiTween> configure) => Property("rotate", configure);
    public WaapiAnimationBuilder Scale(double value) => Property("scale", value);
    public WaapiAnimationBuilder Scale(Action<WaapiTween> configure) => Property("scale", configure);
    public WaapiAnimationBuilder Opacity(double value) => Property("opacity", value);
    public WaapiAnimationBuilder Opacity(Action<WaapiTween> configure) => Property("opacity", configure);

    public WaapiAnimationBuilder Property(string name, double value)
    {
        Remember(name);
        _parameters[name] = value;
        return this;
    }

    public WaapiAnimationBuilder Property(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Remember(name);
        _parameters[name] = value;
        return this;
    }

    public WaapiAnimationBuilder Property(string name, Func<int, int, double> callback) =>
        Value(name, callback, property: true);

    public WaapiAnimationBuilder Property(string name, Func<int, int, string> callback) =>
        Value(name, callback, property: true);

    public WaapiAnimationBuilder Property(string name, Func<TargetInfo, double> callback) =>
        Target(name, callback, property: true);

    public WaapiAnimationBuilder Property(string name, Func<TargetInfo, string> callback) =>
        Target(name, callback, property: true);

    public WaapiAnimationBuilder Property(string name, Action<WaapiTween> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        Remember(name);
        var tween = new WaapiTween();
        configure(tween);
        if (!tween.HasSettings)
            throw new ArgumentException($"Property '{name}' has no settings.", nameof(configure));
        _parameters[name] = tween.Settings;
        return this;
    }

    internal Dictionary<string, object> Build() => _parameters;
    internal List<IDisposable> Callbacks { get; } = [];

    internal static object EaseMarker(Easing easing)
    {
        if (easing.IsCurve)
        {
            return new Dictionary<string, object>
            {
                ["propType"] = "easeFn",
                ["value"] = new Dictionary<string, object>
                {
                    ["fn"] = "curve",
                    ["value"] = easing.Samples
                }
            };
        }

        if (easing.IsFunction)
        {
            var spec = easing.FunctionSpec;
            object payload = spec.Fn switch
            {
                "steps" => new Dictionary<string, object> { ["fn"] = "steps", ["steps"] = spec.A ?? 0 },
                "cubicBezier" => new Dictionary<string, object>
                {
                    ["fn"] = "cubicBezier",
                    ["x1"] = spec.A ?? 0,
                    ["y1"] = spec.B ?? 0,
                    ["x2"] = spec.C ?? 0,
                    ["y2"] = spec.D ?? 0
                },
                "spring" when spec.Bounce is not null => new Dictionary<string, object>
                {
                    ["fn"] = "spring",
                    ["bounce"] = spec.Bounce.Value,
                    ["duration"] = spec.Duration ?? 0
                },
                "spring" when spec.A is not null => new Dictionary<string, object>
                {
                    ["fn"] = "spring",
                    ["mass"] = spec.A.Value,
                    ["stiffness"] = spec.B ?? 0,
                    ["damping"] = spec.C ?? 0,
                    ["velocity"] = spec.D ?? 0
                },
                _ => new Dictionary<string, object> { ["fn"] = spec.Fn }
            };
            return new Dictionary<string, object>
            {
                ["propType"] = "easeFn",
                ["value"] = payload
            };
        }

        return easing.GetValue();
    }

    internal static Dictionary<string, object> StaggerMarker(Stagger stagger)
    {
        var options = new Dictionary<string, object>();
        foreach (var option in stagger.GetOptions())
            options[option.GetName()] = option.GetValue();
        return new Dictionary<string, object>
        {
            ["propType"] = "stagger",
            ["value"] = stagger.GetValue(),
            ["options"] = options
        };
    }

    private WaapiAnimationBuilder Value(string name, Func<int, int, double> callback) =>
        Value(name, callback, property: false);

    private WaapiAnimationBuilder Value(string name, Func<int, int, double> callback, bool property) =>
        Value(name, callback is null ? null! : (index, total) => (object?)callback(index, total), property);

    private WaapiAnimationBuilder Value(string name, Func<int, int, string> callback, bool property) =>
        Value(name, callback is null ? null! : (index, total) => (object?)callback(index, total), property);

    private WaapiAnimationBuilder Value(string name, Func<int, int, object?> callback, bool property)
    {
        if (property)
            Remember(name);
        ValidationChecks.EnsureAcceptableCallback(callback);
        var relay = new ValueCallbackRelay(callback);
        Callbacks.Add(relay);
        _parameters[name] = new Dictionary<string, object>
        {
            ["propType"] = "callback",
            ["paramCount"] = 2,
            ["callback"] = "InvokeAll",
            ["dotNetRef"] = relay.Reference
        };
        return this;
    }

    private WaapiAnimationBuilder Target(string name, Func<TargetInfo, double> callback) =>
        Target(name, callback, property: false);

    private WaapiAnimationBuilder Target(string name, Func<TargetInfo, double> callback, bool property) =>
        Target(name, callback is null ? null! : target => (object?)callback(target), property);

    private WaapiAnimationBuilder Target(string name, Func<TargetInfo, string> callback, bool property) =>
        Target(name, callback is null ? null! : target => (object?)callback(target), property);

    private WaapiAnimationBuilder Target(string name, Func<TargetInfo, object?> callback, bool property)
    {
        if (property)
            Remember(name);
        ValidationChecks.EnsureAcceptableCallback(callback);
        var relay = new TargetCallbackRelay(callback);
        Callbacks.Add(relay);
        _parameters[name] = new Dictionary<string, object>
        {
            ["propType"] = "callback",
            ["paramCount"] = 3,
            ["callback"] = "InvokeTargets",
            ["dotNetRef"] = relay.Reference
        };
        return this;
    }

    private void Remember(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (Reserved.Contains(name))
            throw new ArgumentException($"'{name}' is a WAAPI setting, not a property.", nameof(name));
    }

    private readonly Dictionary<string, object> _parameters = [];

    private static readonly HashSet<string> Reserved = new(StringComparer.Ordinal)
    {
        "id", "keyframes", "playbackEase", "playbackRate", "frameRate", "loop",
        "reversed", "alternate", "autoplay", "persist", "duration", "delay",
        "loopDelay", "ease", "composition", "modifier",
        "onBegin", "onBeforeUpdate", "onUpdate", "onRender", "onLoop", "onPause", "onComplete"
    };
}

/// <summary>Per-property <c>to</c>, <c>from</c>, duration, delay, ease, and composition.</summary>
public sealed class WaapiTween
{
    public WaapiTween To(double value) => Set("to", value);
    public WaapiTween To(string value) => Set("to", value);

    public WaapiTween To(params string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.Length == 0)
            throw new ArgumentException("A keyframe list needs a value.", nameof(values));
        return Set("to", values);
    }

    public WaapiTween From(double value) => Set("from", value);
    public WaapiTween From(string value) => Set("from", value);

    public WaapiTween Duration(int milliseconds) => Set("duration", milliseconds);

    public WaapiTween Delay(int milliseconds) => Set("delay", milliseconds);

    public WaapiTween Delay(Stagger stagger)
    {
        ArgumentNullException.ThrowIfNull(stagger);
        return Set("delay", WaapiAnimationBuilder.StaggerMarker(stagger));
    }

    public WaapiTween Ease(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        return Set("ease", WaapiAnimationBuilder.EaseMarker(easing));
    }

    public WaapiTween Composition(string composition)
    {
        if (composition is not ("replace" or "add" or "accumulate"))
            throw new ArgumentOutOfRangeException(nameof(composition), "Composition is replace, add, or accumulate.");
        return Set("composition", composition);
    }

    internal Dictionary<string, object> Settings { get; } = [];
    internal bool HasSettings => Settings.Count > 0;

    private WaapiTween Set(string name, object value)
    {
        Settings[name] = value;
        return this;
    }
}
