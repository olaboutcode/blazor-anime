using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class PropsBuilder
{
    internal List<IDisposable> CallbackHandles { get; }

    public PropsBuilder() => CallbackHandles = [];

    private PropsBuilder(List<IDisposable> callbackHandles) => CallbackHandles = callbackHandles;

    private PropsBuilder Nested() => new(CallbackHandles);
    public PropsBuilder Ease(Easing easing)
    {
        if (easing == null)
            return this;
        _props.Add(EaseProp("ease", easing));
        return this;
    }

    internal static Prop EaseProp(string name, Easing easing)
    {
        if (easing.IsCurve)
        {
            return new EaseFnProp(name, new
            {
                fn = "curve",
                value = easing.Samples
            });
        }

        if (easing.IsFunction)
        {
            var spec = easing.FunctionSpec;
            return new EaseFnProp(name, spec.Fn switch
            {
                "steps" => new { fn = "steps", steps = spec.A },
                "cubicBezier" => new { fn = "cubicBezier", x1 = spec.A, y1 = spec.B, x2 = spec.C, y2 = spec.D },
                "spring" when spec.Bounce is not null => new { fn = "spring", bounce = spec.Bounce, duration = spec.Duration },
                "spring" when spec.A is not null => new
                {
                    fn = "spring",
                    mass = spec.A,
                    stiffness = spec.B,
                    damping = spec.C,
                    velocity = spec.D
                },
                _ => new { fn = spec.Fn }
            });
        }

        return new GenProp<string>(name, easing.GetValue());
    }

    public PropsBuilder ModifierRound(int decimalPlaces)
    {
        _props.Add(new EaseFnProp("modifier", new { fn = "round", decimals = decimalPlaces }));
        return this;
    }

    public PropsBuilder Alternate(bool alternate)
    {
        _props.Add(new GenProp<bool>("alternate", alternate));
        return this;
    }

    public PropsBuilder Reversed(bool reversed)
    {
        _props.Add(new GenProp<bool>("reversed", reversed));
        return this;
    }

    public PropsBuilder To(double value) => Prop("to", value);

    public PropsBuilder To(string value) => Prop("to", value);

    public PropsBuilder To(Relative value) => Prop("to", value);

    public PropsBuilder From(double value) => Prop("from", value);

    public PropsBuilder From(string value) => Prop("from", value);

    public PropsBuilder From(Relative value) => Prop("from", value);

    public PropsBuilder FromTo(double from, double to) => Prop("to", from, to);

    public PropsBuilder FromTo(string from, string to) => Prop("to", from, to);

    public PropsBuilder FromTo(object from, object to) => Prop("to", from, to);

    public PropsBuilder Keyframes(IReadOnlyDictionary<string, Func<PropsBuilder, PropsBuilder>> frames)
    {
        if (frames == null || frames.Count == 0)
            return this;

        var map = new Dictionary<string, object>();
        foreach (var (key, build) in frames)
        {
            if (string.IsNullOrWhiteSpace(key) || build == null)
                continue;
            var builder = build(Nested());
            Absorb(builder);
            map[key] = builder._props.ToObject();
        }

        if (map.Count > 0)
            _props.Add(new GenProp<object>("keyframes", map));
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
    public PropsBuilder Prop(string property, Relative value)
    {
        if(!string.IsNullOrWhiteSpace(property) && value != null)
            _props.Add(new GenProp<string>(property, value.GetValue()));
        return this;
    }
    public PropsBuilder Prop(string property, ElementReference element)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(new GenProp<ElementReference>(property, element));
        return this;
    }
    public PropsBuilder Prop(string property, params ElementReference[] elements)
    {
        if(!string.IsNullOrWhiteSpace(property)
            && elements is { Length: > 0 })
            _props.Add(new GenProp<ElementReference[]>(property, elements));
        return this;
    }
    public PropsBuilder Prop(string property, SvgPathParam path)
    {
        if(!string.IsNullOrWhiteSpace(property) && path != null)
            _props.Add(new SvgProp(property, path.ParamRef));
        return this;
    }
    internal PropsBuilder ObjectTarget(string property, object reference)
    {
        if(!string.IsNullOrWhiteSpace(property) && reference != null)
            _props.Add(new ObjectTargetProp(property, reference));
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
    public PropsBuilder Prop(string property, Func<int, int, object> callback)
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
    public PropsBuilder Prop(string property, Func<TargetInfo, double> callback)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(CreateTargetCallback(property, callback));
        return this;
    }
    public PropsBuilder Prop(string property, Func<TargetInfo, string> callback)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(CreateTargetCallback(property, callback));
        return this;
    }
    public PropsBuilder Prop(string property, Func<TargetInfo, object> callback)
    {
        if(!string.IsNullOrWhiteSpace(property))
            _props.Add(CreateTargetCallback(property, callback));
        return this;
    }
    public PropsBuilder Prop(string property, Stagger stagger)
    {
        if(!string.IsNullOrWhiteSpace(property) && stagger != null)
            _props.Add(new StgProp(property, stagger.GetValue(), stagger.GetOptions().ToStaggerOptions()));
        return this;
    }
    public PropsBuilder Prop(string property, Action<PropsBuilder> build)
    {
        if (string.IsNullOrWhiteSpace(property) || build == null)
            return this;
    
        var builder = Nested();
        build(builder);
        _props.Add(new GenProp<object>(property, builder._props.ToObject()));
        return this;
    }
    public PropsBuilder Prop(string property, Func<PropsBuilder, PropsBuilder> build)
    {
        if (string.IsNullOrWhiteSpace(property) || build == null)
            return this;
    
        var builder = build(Nested());
        Absorb(builder);
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
            var builder = Nested();
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
            var builder = builderFunc(Nested());
            Absorb(builder);
            keyframes.Add(builder._props.ToObject());
        }
        _props.Add(new GenProp<object[]>(property, [..keyframes]));
        return this;
    }

    internal IReadOnlyList<Prop> Build() => _props.AsReadOnly();

    private void Absorb(PropsBuilder builder)
    {
        if (!ReferenceEquals(builder.CallbackHandles, CallbackHandles))
            CallbackHandles.AddRange(builder.CallbackHandles);
    }
    private readonly List<Prop> _props = [];

    private CallbackProp CreateValueSetterCallback(string propName, Func<int, int, double> callback) =>
        CreateValueCallback(propName, (index, total) => callback(index, total));

    private CallbackProp CreateValueSetterCallback(string propName, Func<int, int, string> callback) =>
        CreateValueCallback(propName, (index, total) => callback(index, total));

    private CallbackProp CreateValueSetterCallback(string propName, Func<int, int, object> callback) =>
        CreateValueCallback(propName, callback);

    private CallbackProp CreateValueCallback(string propName, Func<int, int, object?> callback)
    {
        ValidationChecks.EnsureAcceptableCallback(callback);
        var relay = new ValueCallbackRelay(callback);
        CallbackHandles.Add(relay);
        return new CallbackProp(propName, "InvokeAll", 2, relay.Reference);
    }

    private CallbackProp CreateTargetCallback(string propName, Func<TargetInfo, double> callback) =>
        CreateTargetCallbackCore(propName, target => callback(target));

    private CallbackProp CreateTargetCallback(string propName, Func<TargetInfo, string> callback) =>
        CreateTargetCallbackCore(propName, target => callback(target));

    private CallbackProp CreateTargetCallback(string propName, Func<TargetInfo, object> callback) =>
        CreateTargetCallbackCore(propName, target => callback(target));

    private CallbackProp CreateTargetCallbackCore(string propName, Func<TargetInfo, object?> callback)
    {
        ValidationChecks.EnsureAcceptableCallback(callback);
        var relay = new TargetCallbackRelay(callback);
        CallbackHandles.Add(relay);
        return new CallbackProp(propName, "InvokeTargets", 3, relay.Reference);
    }

    private CallbackProp CreateStateCallback(string propName, Action<AnimationState> callback)
    {
        ValidationChecks.EnsureAcceptableCallback(callback);
        var relay = new StateCallbackRelay(callback);
        CallbackHandles.Add(relay);
        return new CallbackProp(propName, "Invoke", 1, relay.Reference);
    }
}

public sealed class Relative
{
    public static Relative Add(double value) => new($"+={value}");
    public static Relative Subtract(double value) => new($"-={value}");
    public static Relative Multiply(double value) => new($"*={value}");
    public static Relative Add(string value) => new($"+={value}");
    public static Relative Subtract(string value) => new($"-={value}");
    public static Relative Multiply(string value) => new($"*={value}");
    public string GetValue() => _name;
    private Relative(string name) { _name = name; }
    private readonly string _name;
}


