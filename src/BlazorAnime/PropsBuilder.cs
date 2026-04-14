using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class PropsBuilder
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
    public PropsBuilder Value(Relative value)
    {
        if(value != null)
            _props.Add(new GenProp<string>("value", value.GetValue()));
        return this;
    }
    public PropsBuilder Value(double value)
    {
        return Prop("value", value);
    }
    public PropsBuilder Value(double from, double to)
    {
        return Prop("value", from, to);
    }
    public PropsBuilder Value(params string[] value)
    {
        if(value != null)
            _props.Add(new GenProp<string[]>("value", value));
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
    public PropsBuilder Prop(string property, string value)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(new GenProp<string>(property, value));
        return this;
    }
    public PropsBuilder Prop(string property, params string[] values)
    {
        if(!string.IsNullOrWhiteSpace(property)
            && values != null
            && values.Length > 0)
            _props.Add(new GenProp<string[]>(property, values));
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
    public PropsBuilder Prop(string property, SvgPathParam path)
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
    public PropsBuilder Prop(string property, Func<int, int, string> callback)
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
    public PropsBuilder Prop(string property, Action<PropsBuilder> build)
    {
        if (string.IsNullOrWhiteSpace(property) || build == null)
            return this;
    
        var builder = new PropsBuilder();
        build(builder);
        _props.Add(new GenProp<object>(property, builder._props.ToObject()));
        return this;
    }
    public PropsBuilder Prop(string property, Func<PropsBuilder, PropsBuilder> build)
    {
        if (string.IsNullOrWhiteSpace(property) || build == null)
            return this;
    
        var builder = build(new PropsBuilder());
        _props.Add(new GenProp<object>(property, builder._props.ToObject()));
        return this;
    }
    public PropsBuilder Prop(string property, params Action<PropsBuilder>[] builders)
    {
        if (string.IsNullOrWhiteSpace(property)
            || builders == null
            || builders.Length == 0)
            return this;
    
        var keyframes = new List<object>();
        foreach (var build in builders)
        {
            var builder = new PropsBuilder();
            build(builder);
            keyframes.Add(builder._props.ToObject());
        }
        _props.Add(new GenProp<object>(property, keyframes));
        return this;
    }
    public PropsBuilder Prop(string property, params Func<PropsBuilder, PropsBuilder>[] builders)
    {
        if (string.IsNullOrWhiteSpace(property)
            || builders == null
            || builders.Length == 0)
            return this;
    
        var keyframes = new List<object>();
        foreach (var builderFunc in builders)
        {
            var builder = builderFunc(new PropsBuilder());
            keyframes.Add(builder._props.ToObject());
        }
        _props.Add(new GenProp<object>(property, keyframes));
        return this;
    }

    internal IReadOnlyList<Prop> Build() => _props.AsReadOnly();
    private readonly List<Prop> _props = [];

    private static CallbackProp CreateValueSetterCallback(string propName, Func<int, int, double> callback)
    {
        ValidationChecks.EnsureAcceptableCallback(callback);
        return new CallbackProp(
            propName,
            callback.Method.Name,
            callback.Method.GetParameters().Length,
            DotNetObjectReference.Create(callback.Target!));
    }
    private static CallbackProp CreateValueSetterCallback(string propName, Func<int, int, string> callback)
    {
        ValidationChecks.EnsureAcceptableCallback(callback);
        return new CallbackProp(
            propName,
            callback.Method.Name,
            callback.Method.GetParameters().Length,
            DotNetObjectReference.Create(callback.Target!));
    }
    private static CallbackProp CreateStateCallback(string propName, Action<AnimationState> callback)
    {
        ValidationChecks.EnsureAcceptableCallback(callback);
        return new CallbackProp(
            propName,
            callback.Method.Name,
            callback.Method.GetParameters().Length,
            DotNetObjectReference.Create(callback.Target!));
    }
}

public sealed class Relative
{
    public static Relative Add(double value) => new($"+={value}");
    public static Relative Subtract(double value) => new($"-={value}");
    public static Relative Multiply(double value) => new($"*={value}");
    public string GetValue() => _name;
    private Relative(string name) { _name = name; }
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
