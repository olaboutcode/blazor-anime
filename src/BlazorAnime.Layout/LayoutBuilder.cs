using Microsoft.AspNetCore.Components;

namespace BlazorAnime;

/// <summary>
/// Settings for <c>createLayout</c> and for one <see cref="Layout.Animate"/> call.
/// An empty builder leaves anime.js defaults in place.
/// </summary>
public sealed class LayoutBuilder
{
    /// <summary>Selector for the elements this layout tracks. Omitted tracks every child of the root.</summary>
    public LayoutBuilder Children(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        _parameters["children"] = selector;
        return this;
    }

    /// <summary>Selectors for the elements this layout tracks.</summary>
    public LayoutBuilder Children(IReadOnlyList<string> selectors)
    {
        ArgumentNullException.ThrowIfNull(selectors);
        if (selectors.Count == 0)
            throw new ArgumentException("At least one selector is required.", nameof(selectors));
        foreach (var selector in selectors)
            ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        _parameters["children"] = selectors.ToArray();
        return this;
    }

    /// <summary>Element this layout tracks.</summary>
    public LayoutBuilder Children(ElementReference element)
    {
        _parameters["children"] = element;
        return this;
    }

    /// <summary>Elements this layout tracks.</summary>
    public LayoutBuilder Children(IReadOnlyList<ElementReference> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);
        if (elements.Count == 0)
            throw new ArgumentException("At least one element is required.", nameof(elements));
        _parameters["children"] = elements.ToArray();
        return this;
    }

    /// <summary>Duration of the layout transition, in milliseconds.</summary>
    public LayoutBuilder Duration(int milliseconds)
    {
        EnsureDuration(milliseconds);
        _parameters["duration"] = milliseconds;
        return this;
    }

    /// <summary>Delay before the layout transition, in milliseconds.</summary>
    public LayoutBuilder Delay(int milliseconds)
    {
        _parameters["delay"] = milliseconds;
        return this;
    }

    /// <summary>Per-element delay. A .NET function cannot run while anime.js measures the DOM.</summary>
    public LayoutBuilder Delay(Stagger stagger)
    {
        ArgumentNullException.ThrowIfNull(stagger);
        _parameters["delay"] = StaggerMarker(stagger);
        return this;
    }

    /// <summary>Per-element delay built with <see cref="StaggerSyntax"/>.</summary>
    public LayoutBuilder Delay(Func<StaggerSyntax, StaggerSyntax> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        return Delay(configure(new StaggerSyntax()).ToStagger());
    }

    /// <summary>Ease of the layout transition.</summary>
    public LayoutBuilder Ease(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        _parameters["ease"] = EaseMarker(easing);
        return this;
    }

    /// <summary>Extra CSS properties interpolated when their computed values change.</summary>
    public LayoutBuilder Properties(params string[] names)
    {
        ArgumentNullException.ThrowIfNull(names);
        if (names.Length == 0)
            throw new ArgumentException("At least one property is required.", nameof(names));
        foreach (var name in names)
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _parameters["properties"] = names.ToArray();
        return this;
    }

    /// <summary>State applied to elements that appear. Replaces the default opacity of 0.</summary>
    public LayoutBuilder EnterFrom(Action<LayoutStateBuilder> configure) =>
        State("enterFrom", configure);

    /// <summary>State applied to elements that disappear. Replaces the default opacity of 0.</summary>
    public LayoutBuilder LeaveTo(Action<LayoutStateBuilder> configure) =>
        State("leaveTo", configure);

    /// <summary>State applied while elements swap. Replaces the default opacity of 0.</summary>
    public LayoutBuilder SwapAt(Action<LayoutStateBuilder> configure) =>
        State("swapAt", configure);

    /// <summary>Layout id. <see cref="Layout.Id"/> reads this value.</summary>
    public LayoutBuilder Id(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        _parameters["id"] = id;
        return this;
    }

    /// <summary>Layout id. <see cref="Layout.Id"/> reads the decimal text.</summary>
    public LayoutBuilder Id(int id)
    {
        _parameters["id"] = id;
        return this;
    }

    /// <summary><see langword="true"/> repeats forever.</summary>
    public LayoutBuilder Loop(bool loop)
    {
        _parameters["loop"] = loop;
        return this;
    }

    /// <summary>Repeat count. A negative count repeats forever. <c>0</c> plays once.</summary>
    public LayoutBuilder Loop(int count)
    {
        _parameters["loop"] = count;
        return this;
    }

    public LayoutBuilder Alternate(bool alternate)
    {
        _parameters["alternate"] = alternate;
        return this;
    }

    public LayoutBuilder Reversed(bool reversed)
    {
        _parameters["reversed"] = reversed;
        return this;
    }

    /// <summary>Pause between repeats, in milliseconds.</summary>
    public LayoutBuilder LoopDelay(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds), "Loop delay must be zero or greater.");
        _parameters["loopDelay"] = milliseconds;
        return this;
    }

    /// <summary><see langword="false"/> builds the transition and does not play it.</summary>
    public LayoutBuilder Autoplay(bool autoplay)
    {
        _parameters["autoplay"] = autoplay;
        return this;
    }

    /// <summary>Ease applied to the timeline playback, not to each element.</summary>
    public LayoutBuilder PlaybackEase(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        _parameters["playbackEase"] = EaseMarker(easing);
        return this;
    }

    public LayoutBuilder OnBegin(Action<AnimationState> callback) => Callback("onBegin", callback);
    public LayoutBuilder OnBeforeUpdate(Action<AnimationState> callback) => Callback("onBeforeUpdate", callback);
    public LayoutBuilder OnUpdate(Action<AnimationState> callback) => Callback("onUpdate", callback);
    public LayoutBuilder OnLoop(Action<AnimationState> callback) => Callback("onLoop", callback);
    public LayoutBuilder OnPause(Action<AnimationState> callback) => Callback("onPause", callback);
    public LayoutBuilder OnComplete(Action<AnimationState> callback) => Callback("onComplete", callback);
    public LayoutBuilder OnRender(Action<AnimationState> callback) => Callback("onRender", callback);

    internal Dictionary<string, object> Build() => _parameters;
    internal List<IDisposable> Callbacks { get; } = [];

    private LayoutBuilder State(string name, Action<LayoutStateBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var state = new LayoutStateBuilder();
        configure(state);
        if (!state.HasSettings)
            throw new ArgumentException($"{name} needs at least one setting.", nameof(configure));
        _parameters[name] = state.Build();
        return this;
    }

    private LayoutBuilder Callback(string name, Action<AnimationState> callback)
    {
        ValidationChecks.EnsureAcceptableCallback(callback);
        var relay = new StateCallbackRelay(callback);
        Callbacks.Add(relay);
        _parameters[name] = new Dictionary<string, object>
        {
            ["propType"] = "callback",
            ["dotNetRef"] = relay.Reference
        };
        return this;
    }

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

    private static void EnsureDuration(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds), "Duration must be zero or greater.");
    }

    private readonly Dictionary<string, object> _parameters = [];
}

/// <summary>Numeric and string values for <c>enterFrom</c>, <c>leaveTo</c>, or <c>swapAt</c>.</summary>
public sealed class LayoutStateBuilder
{
    public LayoutStateBuilder Duration(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds), "Duration must be zero or greater.");
        Settings["duration"] = milliseconds;
        HasSettings = true;
        return this;
    }

    public LayoutStateBuilder Delay(int milliseconds)
    {
        Settings["delay"] = milliseconds;
        HasSettings = true;
        return this;
    }

    public LayoutStateBuilder Delay(Stagger stagger)
    {
        ArgumentNullException.ThrowIfNull(stagger);
        Settings["delay"] = LayoutBuilder.StaggerMarker(stagger);
        HasSettings = true;
        return this;
    }

    public LayoutStateBuilder Ease(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        Settings["ease"] = LayoutBuilder.EaseMarker(easing);
        HasSettings = true;
        return this;
    }

    /// <summary>One CSS value. <paramref name="name"/> is the anime.js property name, such as <c>opacity</c> or <c>translateX</c>.</summary>
    public LayoutStateBuilder Property(string name, double value)
    {
        Remember(name);
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), "A layout value must be finite.");
        Settings[name] = value;
        HasSettings = true;
        return this;
    }

    /// <summary>One CSS value. <paramref name="name"/> is the anime.js property name.</summary>
    public LayoutStateBuilder Property(string name, string value)
    {
        Remember(name);
        ArgumentNullException.ThrowIfNull(value);
        Settings[name] = value;
        HasSettings = true;
        return this;
    }

    public LayoutStateBuilder Opacity(double value) => Property("opacity", value);

    internal Dictionary<string, object> Build() => Settings;
    internal bool HasSettings { get; private set; }
    private Dictionary<string, object> Settings { get; } = [];

    private static void Remember(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (Reserved.Contains(name))
            throw new ArgumentException($"'{name}' is a timing setting, not a layout property.", nameof(name));
    }

    private static readonly HashSet<string> Reserved = new(StringComparer.Ordinal)
    {
        "delay", "duration", "ease"
    };
}
