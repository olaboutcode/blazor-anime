using Microsoft.JSInterop;

namespace BlazorAnime;

internal class PropsBuilder
{
    public PropsBuilder Easing(Easing easing)
    {
        if(easing != null)
            _props.Add(new GenProp<string>("easing", easing.GetValue()));
        return this;
    }
    public PropsBuilder Direction(Direction direction)
    {
        if(direction != null)
            _props.Add(new GenProp<string>("direction", direction.GetValue()));
        return this;
    }

    public PropsBuilder Prop(string property, int value)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(new GenProp<int>(property, value));
        return this;
    }
    public PropsBuilder Prop(string property, bool value)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(new GenProp<bool>(property, value));
        return this;
    }
    public PropsBuilder Prop(string property, double value)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(new GenProp<double>(property, value));
        return this;
    }
    public PropsBuilder Prop(string property, object from, object to)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(new GenProp<object>(property, new[] { from, to }));
        return this;
    }
    public PropsBuilder Prop(string property, string from, string to)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(new GenProp<object>(property, new[] { from, to }));
        return this;
    }
    public PropsBuilder Prop(string property, double from, double to)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(new GenProp<object>(property, new[] { from, to }));
        return this;
    }
    public PropsBuilder Prop(string property, PathParam path)
    {
        if(!string.IsNullOrWhiteSpace(property) && path != null)
            _props.Add(new SvgProp(property, path.ParamRef));
        return this;
    }
    public PropsBuilder Prop(string property, Action<AnimationState> callback)
    {
        _props.Add(CreateStateCallback(property, callback));
        return this;
    }
    public PropsBuilder Prop(string property, Func<int, int, double> callback)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(CreateValueSetterCallback(property, callback));
        return this;
    }
    public PropsBuilder Prop(string property, Stagger stagger)
    {
        if(!string.IsNullOrWhiteSpace(property) && stagger != null)
            _props.Add(new StgProp(property, stagger.GetValue(), stagger.GetOptions().ToObject()));
        return this;
    }
    public PropsBuilder Prop(string property, Action<PropsBuilder> propsAction)
    {
        if (!string.IsNullOrWhiteSpace(property) || propsAction == null)
            return this;
    
        var builder = new PropsBuilder();
        propsAction(builder);
        _props.Add(new GenProp<object>(property, builder._props.ToObject()));
        return this;
    }
    public PropsBuilder Prop(string property, params Action<PropsBuilder>[] propsActions)
    {
        if (!string.IsNullOrWhiteSpace(property)
            || propsActions == null
            || propsActions.Length == 0)
            return this;
    
        var keyframesList = new List<object>();
        foreach (var action in propsActions)
        {
            var builder = new PropsBuilder();
            action(builder);
            keyframesList.Add(builder._props.ToObject());
        }
        _props.Add(new GenProp<object>(property, keyframesList));
        return this;
    }

    private static CallbackProp CreateValueSetterCallback(string propName, Func<int, int, double> callback)
    {
        Checks.EnsureAcceptableCallback(callback);
        return new CallbackProp(
            propName,
            callback.Method.Name,
            callback.Method.GetParameters().Length,
            DotNetObjectReference.Create(callback.Target!));
    }
    private static CallbackProp CreateStateCallback(string propName, Action<AnimationState> callback)
    {
        Checks.EnsureAcceptableCallback(callback);
        return new CallbackProp(
            propName,
            callback.Method.Name,
            callback.Method.GetParameters().Length,
            DotNetObjectReference.Create(callback.Target!));
    }

    protected List<Prop> _props = [];
}

public sealed class Offset
{
    public static Offset Add(double value) => new($"+={value}");
    public static Offset Subtract(double value) => new($"-={value}");
    public static Offset Multiply(double value) => new($"*={value}");
    public string GetValue() => _name;
    private Offset(string name) { _name = name; }
    private readonly string _name;
}

public sealed class Direction
{
    public static Direction Normal { get; } = new("normal");
    public static Direction Reverse { get; } = new("reverse");
    public static Direction Alternate { get; } = new("alternate");
    public string GetValue() => _name;
    private Direction(string name) { _name = name; }
    private readonly string _name;
}

public sealed class Easing
{
    public static Easing Spring() => new("spring");
    public static Easing Steps(int steps) => new($"steps({steps})");
    public static Easing EaseInElastic(double amplitude, double period)
        => new($"easeInElastic({amplitude}, {period})");
    public static Easing EaseOutElastic(double amplitude, double period)
        => new($"easeOutElastic({amplitude}, {period})");
    public static Easing EaseInOutElastic(double amplitude, double period)
        => new($"easeInOutElastic({amplitude}, {period})");
    public static Easing EaseOutInElastic(double amplitude, double period)
        => new($"easeOutInElastic({amplitude}, {period})");
    public static Easing CubicBezier(double x1, double y1, double x2, double y2) =>
        new($"cubicBezier({x1}, {y1}, {x2}, {y2})");
    public static Easing Spring(double mass, double stiffness, double damping, double velocity)
        => new($"spring({mass}, {stiffness}, {damping}, {velocity})");

    public static Easing Linear { get; } = new("linear");
    public static Easing EaseInQuad { get; } = new("easeInQuad");
    public static Easing EaseInCubic { get; } = new("easeInCubic");
    public static Easing EaseInQuart { get; } = new("easeInQuart");
    public static Easing EaseInQuint { get; } = new("easeInQuint");
    public static Easing EaseInSine { get; } = new("easeInSine");
    public static Easing EaseInExpo { get; } = new("easeInExpo");
    public static Easing EaseInCirc { get; } = new("easeInCirc");
    public static Easing EaseInBack { get; } = new("easeInBack");
    public static Easing EaseInBounce { get; } = new("easeInBounce");

    public static Easing EaseOutQuad { get; } = new("easeOutQuad");
    public static Easing EaseOutCubic { get; } = new("easeOutCubic");
    public static Easing EaseOutQuart { get; } = new("easeOutQuart");
    public static Easing EaseOutQuint { get; } = new("easeOutQuint");
    public static Easing EaseOutSine { get; } = new("easeOutSine");
    public static Easing EaseOutExpo { get; } = new("easeOutExpo");
    public static Easing EaseOutCirc { get; } = new("easeOutCirc");
    public static Easing EaseOutBack { get; } = new("easeOutBack");
    public static Easing EaseOutBounce { get; } = new("easeOutBounce");

    public static Easing EaseInOutQuad { get; } = new("easeInOutQuad");
    public static Easing EaseInOutCubic { get; } = new("easeInOutCubic");
    public static Easing EaseInOutQuart { get; } = new("easeInOutQuart");
    public static Easing EaseInOutQuint { get; } = new("easeInOutQuint");
    public static Easing EaseInOutSine { get; } = new("easeInOutSine");
    public static Easing EaseInOutExpo { get; } = new("easeInOutExpo");
    public static Easing EaseInOutCirc { get; } = new("easeInOutCirc");
    public static Easing EaseInOutBack { get; } = new("easeInOutBack");
    public static Easing EaseInOutBounce { get; } = new("easeInOutBounce");

    public static Easing EaseOutInQuad { get; } = new("easeOutInQuad");
    public static Easing EaseOutInCubic { get; } = new("easeOutInCubic");
    public static Easing EaseOutInQuart { get; } = new("easeOutInQuart");
    public static Easing EaseOutInQuint { get; } = new("easeOutInQuint");
    public static Easing EaseOutInSine { get; } = new("easeOutInSine");
    public static Easing EaseOutInExpo { get; } = new("easeOutInExpo");
    public static Easing EaseOutInCirc { get; } = new("easeOutInCirc");
    public static Easing EaseOutInBack { get; } = new("easeOutInBack");
    public static Easing EaseOutInBounce { get; } = new("easeOutInBounce");

    public string GetValue() => _name;
    private Easing(string name) { _name = name; }
    private readonly string _name;
}