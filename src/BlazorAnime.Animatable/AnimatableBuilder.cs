namespace BlazorAnime;

/// <summary>
/// Settings for <c>createAnimatable</c>. A property name is the anime.js name,
/// for example <c>x</c>, <c>translateX</c>, or <c>backgroundColor</c>.
/// </summary>
public sealed class AnimatableBuilder
{
    /// <summary>Duration for one property, in milliseconds.</summary>
    public AnimatableBuilder Property(string name, int durationMilliseconds)
    {
        Remember(name);
        _parameters[name] = durationMilliseconds;
        return this;
    }

    /// <summary>Duration for one property. A stagger function is the per-target duration.</summary>
    public AnimatableBuilder Property(string name, Stagger duration)
    {
        ArgumentNullException.ThrowIfNull(duration);
        Remember(name);
        _parameters[name] = StaggerMarker(duration);
        return this;
    }

    /// <summary>Unit, duration, ease, or modifier for one property.</summary>
    public AnimatableBuilder Property(string name, Action<AnimatableProperty> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        Remember(name);
        var property = new AnimatableProperty();
        configure(property);
        if (!property.HasSettings)
            throw new ArgumentException($"Property '{name}' has no settings.", nameof(configure));
        _parameters[name] = property.Settings;
        return this;
    }

    /// <summary>Default ease for every property.</summary>
    public AnimatableBuilder Ease(Easing easing)
    {
        if (easing == null)
            return this;
        _parameters["ease"] = EaseMarker(easing);
        return this;
    }

    /// <summary>Default duration for every property, in milliseconds.</summary>
    public AnimatableBuilder Duration(int durationMilliseconds)
    {
        _parameters["duration"] = durationMilliseconds;
        return this;
    }

    /// <summary>Rounds every property to <paramref name="decimalPlaces"/>.</summary>
    public AnimatableBuilder ModifierRound(int decimalPlaces)
    {
        _parameters["modifier"] = ModifierMarker(decimalPlaces);
        return this;
    }

    public AnimatableBuilder OnBegin(Action<AnimationState> callback) => Callback("onBegin", callback);
    public AnimatableBuilder OnBeforeUpdate(Action<AnimationState> callback) => Callback("onBeforeUpdate", callback);
    public AnimatableBuilder OnUpdate(Action<AnimationState> callback) => Callback("onUpdate", callback);
    public AnimatableBuilder OnRender(Action<AnimationState> callback) => Callback("onRender", callback);
    public AnimatableBuilder OnLoop(Action<AnimationState> callback) => Callback("onLoop", callback);
    public AnimatableBuilder OnPause(Action<AnimationState> callback) => Callback("onPause", callback);
    public AnimatableBuilder OnComplete(Action<AnimationState> callback) => Callback("onComplete", callback);

    internal Dictionary<string, object> Build() => _parameters;
    internal List<IDisposable> Callbacks { get; } = [];
    internal bool HasProperties => _propertyCount > 0;

    private void Remember(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (Reserved.Contains(name))
            throw new ArgumentException($"'{name}' is an animatable setting, not a property.", nameof(name));
        _propertyCount++;
    }

    private AnimatableBuilder Callback(string name, Action<AnimationState> callback)
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

    internal static Dictionary<string, object> ModifierMarker(int decimalPlaces) => new()
    {
        ["propType"] = "modifier",
        ["decimals"] = decimalPlaces
    };

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

    private readonly Dictionary<string, object> _parameters = [];
    private int _propertyCount;

    private static readonly HashSet<string> Reserved = new(StringComparer.Ordinal)
    {
        "id", "keyframes", "playbackEase", "playbackRate", "frameRate", "loop",
        "reversed", "alternate", "autoplay", "persist", "duration", "delay",
        "loopDelay", "ease", "composition", "modifier",
        "onBegin", "onBeforeUpdate", "onUpdate", "onRender", "onLoop", "onPause", "onComplete"
    };
}

/// <summary>Settings that apply to one animatable property.</summary>
public sealed class AnimatableProperty
{
    /// <summary>Unit appended to the animated number, for example <c>rem</c>.</summary>
    public AnimatableProperty Unit(string unit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);
        Settings["unit"] = unit;
        HasSettings = true;
        return this;
    }

    /// <summary>Duration for this property, in milliseconds.</summary>
    public AnimatableProperty Duration(int durationMilliseconds)
    {
        Settings["duration"] = durationMilliseconds;
        HasSettings = true;
        return this;
    }

    /// <summary>Per-target duration.</summary>
    public AnimatableProperty Duration(Stagger duration)
    {
        ArgumentNullException.ThrowIfNull(duration);
        Settings["duration"] = AnimatableBuilder.StaggerMarker(duration);
        HasSettings = true;
        return this;
    }

    public AnimatableProperty Ease(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        Settings["ease"] = AnimatableBuilder.EaseMarker(easing);
        HasSettings = true;
        return this;
    }

    public AnimatableProperty ModifierRound(int decimalPlaces)
    {
        Settings["modifier"] = AnimatableBuilder.ModifierMarker(decimalPlaces);
        HasSettings = true;
        return this;
    }

    internal Dictionary<string, object> Settings { get; } = [];
    internal bool HasSettings { get; private set; }
}
