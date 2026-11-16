using Microsoft.AspNetCore.Components;

namespace BlazorAnime;

/// <summary>How a dragged axis composes with another tween of the same property.</summary>
public enum DragComposition
{
    /// <summary>The drag replaces the other tween.</summary>
    Replace,

    /// <summary>The drag is ignored while another tween owns the property.</summary>
    None,

    /// <summary>The drag is added to the other tween.</summary>
    Blend
}

/// <summary>
/// Settings for one axis of <c>createDraggable</c>.
/// An omitted axis stays enabled and maps to <c>translateX</c> or <c>translateY</c>.
/// </summary>
public sealed class DraggableAxis
{
    /// <summary>CSS property this axis writes, for example <c>rotate</c>.</summary>
    public DraggableAxis MapTo(string property)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(property);
        _mapTo = property;
        return this;
    }

    /// <summary>Rounds this axis to <paramref name="decimalPlaces"/>.</summary>
    public DraggableAxis ModifierRound(int decimalPlaces)
    {
        _modifier = DraggableBuilder.ModifierMarker(decimalPlaces);
        return this;
    }

    /// <summary>Snaps this axis to the nearest multiple of <paramref name="increment"/>.</summary>
    public DraggableAxis ModifierSnap(double increment)
    {
        _modifier = DraggableBuilder.SnapMarker(increment);
        return this;
    }

    /// <summary>Negates this axis inside the tween.</summary>
    public DraggableAxis ModifierInvert()
    {
        _modifier = DraggableBuilder.InvertMarker();
        return this;
    }

    /// <summary>How this axis combines with another tween of the mapped property.</summary>
    public DraggableAxis Composition(DragComposition composition)
    {
        _composition = DraggableBuilder.CompositionName(composition);
        return this;
    }

    /// <summary>Snap grid for this axis, in pixels. This wins over <see cref="DraggableBuilder.Snap(double)"/>.</summary>
    public DraggableAxis Snap(double increment)
    {
        _snap = DraggableBuilder.SnapValue(increment);
        return this;
    }

    /// <summary>Snap points for this axis. The drag moves to the nearest value.</summary>
    public DraggableAxis Snap(IReadOnlyList<double> points)
    {
        _snap = DraggableBuilder.SnapPoints(points);
        return this;
    }

    internal Dictionary<string, object> ToParameters()
    {
        var result = new Dictionary<string, object>();
        if (_mapTo is not null)
            result["mapTo"] = _mapTo;
        if (_modifier is not null)
            result["modifier"] = _modifier;
        if (_composition is not null)
            result["composition"] = _composition;
        if (_snap is not null)
            result["snap"] = _snap;
        return result;
    }

    private string? _mapTo;
    private Dictionary<string, object>? _modifier;
    private string? _composition;
    private object? _snap;
}

/// <summary>
/// Settings for <c>createDraggable</c>. An empty builder drags both axes.
/// Function-valued settings are not called from .NET during the pointer event.
/// </summary>
public sealed class DraggableBuilder
{
    /// <summary>Enables the horizontal axis.</summary>
    public DraggableBuilder X() => X(true);

    /// <summary>Enables or disables the horizontal axis.</summary>
    public DraggableBuilder X(bool enabled)
    {
        _parameters["x"] = enabled;
        return this;
    }

    /// <summary>Map, modifier, composition, or snap for the horizontal axis.</summary>
    public DraggableBuilder X(Action<DraggableAxis> configure) => Axis("x", configure);

    /// <summary>Enables the vertical axis.</summary>
    public DraggableBuilder Y() => Y(true);

    /// <summary>Enables or disables the vertical axis.</summary>
    public DraggableBuilder Y(bool enabled)
    {
        _parameters["y"] = enabled;
        return this;
    }

    /// <summary>Map, modifier, composition, or snap for the vertical axis.</summary>
    public DraggableBuilder Y(Action<DraggableAxis> configure) => Axis("y", configure);

    /// <summary>Rounds both axes to <paramref name="decimalPlaces"/>.</summary>
    public DraggableBuilder ModifierRound(int decimalPlaces)
    {
        _parameters["modifier"] = ModifierMarker(decimalPlaces);
        return this;
    }

    /// <summary>
    /// Snaps both axes to the nearest multiple of <paramref name="increment"/>.
    /// An axis <see cref="DraggableAxis.ModifierSnap"/> wins on that axis.
    /// </summary>
    public DraggableBuilder ModifierSnap(double increment)
    {
        _parameters["modifier"] = SnapMarker(increment);
        return this;
    }

    /// <summary>Negates both axes. An axis <see cref="DraggableAxis.ModifierInvert"/> wins on that axis.</summary>
    public DraggableBuilder ModifierInvert()
    {
        _parameters["modifier"] = InvertMarker();
        return this;
    }

    /// <summary>Element that receives the pointer. The dragged element is the default.</summary>
    public DraggableBuilder Trigger(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        _parameters["trigger"] = selector;
        return this;
    }

    /// <summary>Element that receives the pointer.</summary>
    public DraggableBuilder Trigger(ElementReference element)
    {
        _parameters["trigger"] = element;
        return this;
    }

    /// <summary>Element the drag stays inside.</summary>
    public DraggableBuilder Container(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        _parameters["container"] = selector;
        return this;
    }

    /// <summary>Element the drag stays inside.</summary>
    public DraggableBuilder Container(ElementReference element)
    {
        _parameters["container"] = element;
        return this;
    }

    /// <summary>Bounds in pixels: top, right, bottom, left.</summary>
    public DraggableBuilder Container(double top, double right, double bottom, double left)
    {
        EnsureFinite(top, nameof(top));
        EnsureFinite(right, nameof(right));
        EnsureFinite(bottom, nameof(bottom));
        EnsureFinite(left, nameof(left));
        _parameters["container"] = new[] { top, right, bottom, left };
        return this;
    }

    /// <summary>Same padding on every side of the container, in pixels.</summary>
    public DraggableBuilder ContainerPadding(double padding)
    {
        EnsureFinite(padding, nameof(padding));
        _parameters["containerPadding"] = padding;
        return this;
    }

    /// <summary>Padding in pixels: top, right, bottom, left.</summary>
    public DraggableBuilder ContainerPadding(double top, double right, double bottom, double left)
    {
        EnsureFinite(top, nameof(top));
        EnsureFinite(right, nameof(right));
        EnsureFinite(bottom, nameof(bottom));
        EnsureFinite(left, nameof(left));
        _parameters["containerPadding"] = new[] { top, right, bottom, left };
        return this;
    }

    /// <summary>How far a drag can pass the container edge while the pointer is down. 0 to 1.</summary>
    public DraggableBuilder ContainerFriction(double friction) =>
        Number("containerFriction", friction);

    /// <summary>How far the release animation can pass the container edge. 0 to 1.</summary>
    public DraggableBuilder ReleaseContainerFriction(double friction) =>
        Number("releaseContainerFriction", friction);

    /// <summary>Multiplier applied to pointer movement.</summary>
    public DraggableBuilder DragSpeed(double speed) => Number("dragSpeed", speed);

    /// <summary>Pixels of movement before a press becomes a drag. Used for mouse and touch.</summary>
    public DraggableBuilder DragThreshold(double threshold)
    {
        EnsureFinite(threshold, nameof(threshold));
        _parameters["dragThreshold"] = threshold;
        return this;
    }

    /// <summary>Separate mouse and touch thresholds, in pixels.</summary>
    public DraggableBuilder DragThreshold(double mouse, double touch)
    {
        EnsureFinite(mouse, nameof(mouse));
        EnsureFinite(touch, nameof(touch));
        _parameters["dragThreshold"] = new Dictionary<string, object>
        {
            ["mouse"] = mouse,
            ["touch"] = touch
        };
        return this;
    }

    /// <summary>Speed of automatic scrolling while the pointer is near the edge.</summary>
    public DraggableBuilder ScrollSpeed(double speed) => Number("scrollSpeed", speed);

    /// <summary>Distance from the scroll edge that starts automatic scrolling, in pixels.</summary>
    public DraggableBuilder ScrollThreshold(double threshold) => Number("scrollThreshold", threshold);

    /// <summary>Lowest release velocity.</summary>
    public DraggableBuilder MinVelocity(double velocity) => Number("minVelocity", velocity);

    /// <summary>Highest release velocity.</summary>
    public DraggableBuilder MaxVelocity(double velocity) => Number("maxVelocity", velocity);

    /// <summary>Multiplier applied to the release velocity.</summary>
    public DraggableBuilder VelocityMultiplier(double multiplier) => Number("velocityMultiplier", multiplier);

    /// <summary>Mass of the default release spring.</summary>
    public DraggableBuilder ReleaseMass(double mass) => Number("releaseMass", mass);

    /// <summary>Stiffness of the default release spring.</summary>
    public DraggableBuilder ReleaseStiffness(double stiffness) => Number("releaseStiffness", stiffness);

    /// <summary>Damping of the default release spring.</summary>
    public DraggableBuilder ReleaseDamping(double damping) => Number("releaseDamping", damping);

    /// <summary>
    /// Ease of the release animation. A spring replaces the default release spring.
    /// </summary>
    public DraggableBuilder ReleaseEase(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        _parameters["releaseEase"] = EaseMarker(easing);
        return this;
    }

    /// <summary><c>false</c> leaves the cursor unchanged. <c>true</c> uses grab and grabbing.</summary>
    public DraggableBuilder Cursor(bool enabled)
    {
        _parameters["cursor"] = enabled;
        return this;
    }

    /// <summary>CSS cursors used while hovering and while dragging.</summary>
    public DraggableBuilder Cursor(string? onHover, string? onGrab)
    {
        if (onHover is null && onGrab is null)
            throw new ArgumentException("A cursor needs onHover or onGrab.");

        var cursor = new Dictionary<string, object>();
        if (onHover is not null)
            cursor["onHover"] = onHover;
        if (onGrab is not null)
            cursor["onGrab"] = onGrab;
        _parameters["cursor"] = cursor;
        return this;
    }

    /// <summary>Snap grid for both axes, in pixels. An axis snap wins on that axis.</summary>
    public DraggableBuilder Snap(double increment)
    {
        _parameters["snap"] = SnapValue(increment);
        return this;
    }

    /// <summary>Snap points for both axes. The drag moves to the nearest value.</summary>
    public DraggableBuilder Snap(IReadOnlyList<double> points)
    {
        _parameters["snap"] = SnapPoints(points);
        return this;
    }

    public DraggableBuilder OnGrab(Action<DragState> callback) => Callback("onGrab", callback);
    public DraggableBuilder OnDrag(Action<DragState> callback) => Callback("onDrag", callback);
    public DraggableBuilder OnRelease(Action<DragState> callback) => Callback("onRelease", callback);
    public DraggableBuilder OnUpdate(Action<DragState> callback) => Callback("onUpdate", callback);
    public DraggableBuilder OnSettle(Action<DragState> callback) => Callback("onSettle", callback);
    public DraggableBuilder OnSnap(Action<DragState> callback) => Callback("onSnap", callback);
    public DraggableBuilder OnResize(Action<DragState> callback) => Callback("onResize", callback);
    public DraggableBuilder OnAfterResize(Action<DragState> callback) => Callback("onAfterResize", callback);

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

    internal static Dictionary<string, object> ModifierMarker(int decimalPlaces) => new()
    {
        ["propType"] = "modifier",
        ["fn"] = "round",
        ["decimals"] = decimalPlaces
    };

    internal static Dictionary<string, object> SnapMarker(double increment)
    {
        if (!double.IsFinite(increment) || increment == 0)
            throw new ArgumentOutOfRangeException(nameof(increment), "Snap increment must be a non-zero finite number.");
        return new Dictionary<string, object>
        {
            ["propType"] = "modifier",
            ["fn"] = "snap",
            ["increment"] = increment
        };
    }

    internal static Dictionary<string, object> InvertMarker() => new()
    {
        ["propType"] = "modifier",
        ["fn"] = "invert"
    };

    internal static string CompositionName(DragComposition composition) => composition switch
    {
        DragComposition.Replace => "replace",
        DragComposition.None => "none",
        DragComposition.Blend => "blend",
        _ => throw new ArgumentOutOfRangeException(nameof(composition))
    };

    internal static double SnapValue(double increment)
    {
        if (!double.IsFinite(increment) || increment == 0)
            throw new ArgumentOutOfRangeException(nameof(increment), "Snap increment must be a non-zero finite number.");
        return increment;
    }

    internal static double[] SnapPoints(IReadOnlyList<double> points)
    {
        ArgumentNullException.ThrowIfNull(points);
        if (points.Count == 0)
            throw new ArgumentException("At least one snap point is required.", nameof(points));

        var values = new double[points.Count];
        for (var index = 0; index < points.Count; index++)
        {
            var point = points[index];
            if (!double.IsFinite(point))
                throw new ArgumentOutOfRangeException(nameof(points), "Snap points must be finite numbers.");
            values[index] = point;
        }

        return values;
    }

    private DraggableBuilder Axis(string name, Action<DraggableAxis> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var axis = new DraggableAxis();
        configure(axis);
        _parameters[name] = axis.ToParameters();
        return this;
    }

    private DraggableBuilder Number(string name, double value)
    {
        EnsureFinite(value, name);
        _parameters[name] = value;
        return this;
    }

    private DraggableBuilder Callback(string name, Action<DragState> callback)
    {
        ValidationChecks.EnsureAcceptableCallback(callback);
        var relay = new DragCallbackRelay(callback);
        Callbacks.Add(relay);
        _parameters[name] = new Dictionary<string, object>
        {
            ["propType"] = "callback",
            ["dotNetRef"] = relay.Reference
        };
        return this;
    }

    private static void EnsureFinite(double value, string name)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(name, "Value must be a finite number.");
    }

    private readonly Dictionary<string, object> _parameters = [];
}
