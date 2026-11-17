using Microsoft.AspNetCore.Components;

namespace BlazorAnime;

public sealed class ScrollObserverBuilder
{
    public ScrollObserverBuilder Container(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        ContainerValue = selector;
        HasContainer = true;
        return this;
    }

    public ScrollObserverBuilder Container(ElementReference element)
    {
        ContainerValue = element;
        HasContainer = true;
        return this;
    }

    public ScrollObserverBuilder Target(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        TargetValue = selector;
        HasTarget = true;
        return this;
    }

    public ScrollObserverBuilder Target(ElementReference element)
    {
        TargetValue = element;
        HasTarget = true;
        return this;
    }

    public ScrollObserverBuilder Axis(string axis)
    {
        if (axis is not ("x" or "y"))
            throw new ArgumentOutOfRangeException(nameof(axis), "Axis is x or y.");
        Settings["axis"] = axis;
        return this;
    }

    public ScrollObserverBuilder Debug(bool debug)
    {
        Settings["debug"] = debug;
        return this;
    }

    public ScrollObserverBuilder Repeat(bool repeat)
    {
        Settings["repeat"] = repeat;
        return this;
    }

    public ScrollObserverBuilder Enter(string position) => Threshold("enter", position);

    public ScrollObserverBuilder Enter(double value) => Threshold("enter", value);

    public ScrollObserverBuilder Enter(string target, string container) => Threshold("enter", target, container);

    public ScrollObserverBuilder Leave(string position) => Threshold("leave", position);

    public ScrollObserverBuilder Leave(double value) => Threshold("leave", value);

    public ScrollObserverBuilder Leave(string target, string container) => Threshold("leave", target, container);

    public ScrollObserverBuilder Sync(bool sync)
    {
        Settings["sync"] = sync;
        return this;
    }

    public ScrollObserverBuilder Sync(string methods)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(methods);
        var parts = methods.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || parts.Any(part => !MethodNames.Contains(part)))
            throw new ArgumentOutOfRangeException(nameof(methods), "Sync methods are play, pause, resume, restart, reset, reverse, complete, alternate, or cancel.");
        Settings["sync"] = string.Join(' ', parts);
        return this;
    }

    public ScrollObserverBuilder Sync(double smooth)
    {
        if (!double.IsFinite(smooth) || smooth < 0)
            throw new ArgumentOutOfRangeException(nameof(smooth));
        Settings["sync"] = smooth;
        return this;
    }

    public ScrollObserverBuilder Sync(Easing ease)
    {
        ArgumentNullException.ThrowIfNull(ease);
        Settings["sync"] = EaseMarker(ease);
        return this;
    }

    public ScrollObserverBuilder OnEnter(Action<ScrollState> callback) => Callback("onEnter", callback);

    public ScrollObserverBuilder OnEnterForward(Action<ScrollState> callback) => Callback("onEnterForward", callback);

    public ScrollObserverBuilder OnEnterBackward(Action<ScrollState> callback) => Callback("onEnterBackward", callback);

    public ScrollObserverBuilder OnLeave(Action<ScrollState> callback) => Callback("onLeave", callback);

    public ScrollObserverBuilder OnLeaveForward(Action<ScrollState> callback) => Callback("onLeaveForward", callback);

    public ScrollObserverBuilder OnLeaveBackward(Action<ScrollState> callback) => Callback("onLeaveBackward", callback);

    public ScrollObserverBuilder OnUpdate(Action<ScrollState> callback) => Callback("onUpdate", callback);

    public ScrollObserverBuilder OnSyncComplete(Action<ScrollState> callback) => Callback("onSyncComplete", callback);

    public ScrollObserverBuilder OnResize(Action<ScrollState> callback) => Callback("onResize", callback);

    internal bool HasContainer { get; private set; }
    internal object? ContainerValue { get; private set; }
    internal bool HasTarget { get; private set; }
    internal object? TargetValue { get; private set; }
    internal Dictionary<string, object> Settings { get; } = [];
    internal List<IDisposable> Callbacks { get; } = [];

    internal Dictionary<string, object> Build()
    {
        var built = new Dictionary<string, object>(Settings);
        if (ContainerValue is string container)
            built["container"] = container;
        if (TargetValue is string target)
            built["target"] = target;
        return built;
    }

    private ScrollObserverBuilder Threshold(string name, string position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(position);
        Settings[name] = position;
        return this;
    }

    private ScrollObserverBuilder Threshold(string name, double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value));
        Settings[name] = value;
        return this;
    }

    private ScrollObserverBuilder Threshold(string name, string target, string container)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(target);
        ArgumentException.ThrowIfNullOrWhiteSpace(container);
        Settings[name] = new Dictionary<string, string>
        {
            ["target"] = target,
            ["container"] = container
        };
        return this;
    }

    private ScrollObserverBuilder Callback(string name, Action<ScrollState> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        var relay = new ScrollCallbackRelay(callback);
        Callbacks.Add(relay);
        Settings[name] = relay.Reference;
        return this;
    }

    private static readonly HashSet<string> MethodNames =
    [
        "play", "pause", "resume", "restart", "reset", "reverse", "complete", "alternate", "cancel"
    ];

    private static object EaseMarker(Easing easing)
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
}
