using System.Runtime.CompilerServices;
using Microsoft.JSInterop;

namespace BlazorAnime.V2;

internal static class PropTupleExtensions
{
    public static object GetValue<T>(this (string name, T value) prop)
    {
        EnsureValidProp(prop.name, prop.value);
        return new
        {
            name = LowerFirstChar(prop.name),
            value = new { propType = "setter", value = prop.value }
        };
    }

    public static object GetValue<T>(this (string name, T[] value) prop)
    {
        EnsureValidProp(prop.name, prop.value);
        return new
        {
            name = LowerFirstChar(prop.name),
            value = new { propType = "setter", value = prop.value }
        };
    }

    public static object GetValue(this (string name, IJSObjectReference value) prop)
    {
        return GetValue(prop);
    }

    public static object GetValue(this (string name, object value) prop)
    {
        return GetValue(prop);
    }

    public static object GetValue(this (string name, Stagger value) prop)
    {
        EnsureValidProp(prop.name, prop.value);
        return new
        {
            name = LowerFirstChar(prop.name),
            value = new { 
                propType = "stagger", 
                value = new {
                    value = prop.value.GetValue(),
                    options = prop.value.GetOptions()
                }
            }
        };
    }

    public static object GetValue(this (string name, Func<int, int, double> callback) prop)
    {
        EnsureAcceptableCallback(prop.callback);
        return new
        {
            name = LowerFirstChar(prop.name),
            value = new {
                propType = "callback",
                paramCount = prop.callback.Method.GetParameters().Length,
                value = new { 
                    callback = prop.callback.Method.Name, 
                    dotNetRef = DotNetObjectReference.Create(prop.callback.Target!)
                }
            }
        };
    }

    public static object GetValue(this (string name, Action<AnimationState> callback) prop)
    {
        EnsureAcceptableCallback(prop.callback);
        return new
        {
            name = LowerFirstChar(prop.name),
            value = new { 
                propType = "callback",
                paramCount = prop.callback.Method.GetParameters().Length,
                value = new { 
                    callback = prop.callback.Method.Name, 
                    dotNetRef = DotNetObjectReference.Create(prop.callback.Target!)
                }
            }
        };
    }

    public static object GetValue<T>(this IEnumerable<(string name, T value)> props)
    {
        dynamic propsObject = new System.Dynamic.ExpandoObject();
        foreach (var (pname, pval) in props)
        {
            if (pval is null) continue;
            ((IDictionary<string, object>)propsObject).Add(LowerFirstChar(pname), pval);
        }
        return propsObject;
    }

    public static object GetValue<T>(this (string name, IEnumerable<(string name, T value)> value) props)
    {
        dynamic propsObject = new System.Dynamic.ExpandoObject();
        foreach (var (pname, pval) in props.value)
        {
            if (pval is null) continue;
            ((IDictionary<string, object>)propsObject).Add(LowerFirstChar(pname), pval);
        }
        return new
        {
            name = LowerFirstChar(props.name),
            value = propsObject 
        };
    }

    public static object GetValue(this (string name, IEnumerable<(string name, object value)> value) props)
    {
        return GetValue(props);
    }

    private static void EnsureValidProp<T>(string name, T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNull(value, nameof(value));
    }

    private static void EnsureAcceptableCallback(Delegate callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        bool isLambda = callback.Method.IsDefined(typeof(CompilerGeneratedAttribute), false);
        if (isLambda)
        {
            throw new ArgumentException(
                "Lambda expressions are not supported as callbacks. Please use a named method.");
        }

        if (callback.GetInvocationList().Length > 1)
        {
            throw new ArgumentException(
                "Multicast delegates are not supported as callbacks. Please use a single method.");
        }

        if (callback.Target == null)
        {
            throw new ArgumentException(
                "Static methods are not supported as callbacks. Please use an instance method.");
        }
    }

    private static string LowerFirstChar(string input) =>
        char.ToLower(input[0]) + input[1..];
}
