using System.Runtime.CompilerServices;
using Microsoft.JSInterop;

namespace BlazorAnime.V2;

internal static class PropTupleExtensions
{
    public static AnimeProp GetValue<T>(this (string name, T value) prop)
    {
        EnsureValidProp(prop.name, prop.value);
        return new AnimeProp(
            LowerFirstChar(prop.name),
            new { propType = "setter", prop.value }
        );
    }

    public static AnimeProp GetValue<T>(this (string name, T[] value) prop)
    {
        EnsureValidProp(prop.name, prop.value);
        return new AnimeProp(
            LowerFirstChar(prop.name),
            new { propType = "setter", prop.value }
        );
    }

    public static AnimeProp GetValue(this (string name, IJSObjectReference value) prop)
    {
        return GetValue(prop);
    }

    public static AnimeProp GetValue(this (string name, object value) prop)
    {
        return GetValue(prop);
    }

    public static AnimeProp GetValue(this (string name, Stagger value) prop)
    {
        EnsureValidProp(prop.name, prop.value);
        return new AnimeProp(
            LowerFirstChar(prop.name),
            new { 
                propType = "stagger", 
                value = new {
                    value = prop.value.GetValue(),
                    options = prop.value.GetOptions()
                }
            }
        );
    }

    public static AnimeProp GetValue(this (string name, Func<int, int, double> callback) prop)
    {
        EnsureAcceptableCallback(prop.callback);
        return new AnimeProp(
            LowerFirstChar(prop.name),
            new {
                propType = "callback",
                paramCount = prop.callback.Method.GetParameters().Length,
                value = new { 
                    callback = prop.callback.Method.Name, 
                    dotNetRef = DotNetObjectReference.Create(prop.callback.Target!)
                }
            }
        );
    }

    public static AnimeProp GetValue(this (string name, Action<AnimationState> callback) prop)
    {
        EnsureAcceptableCallback(prop.callback);
        return new AnimeProp(
            LowerFirstChar(prop.name),
            new { 
                propType = "callback",
                paramCount = prop.callback.Method.GetParameters().Length,
                value = new { 
                    callback = prop.callback.Method.Name, 
                    dotNetRef = DotNetObjectReference.Create(prop.callback.Target!)
                }
            }
        );
    }

    public static object GetValue(this IEnumerable<AnimeProp> props)
    {
        dynamic propsObject = new System.Dynamic.ExpandoObject();
        foreach (var prop in props)
        {
            ((IDictionary<string, object>)propsObject).Add(LowerFirstChar(prop.Name), prop.Value);
        }
        return propsObject;
    }

    public static object GetValue(this (string name, IEnumerable<AnimeProp> value) props)
    {
        return new
        {
            name = LowerFirstChar(props.name),
            value = props.GetValue() 
        };
    }

    public static AnimeProp GetValue(this (string name, IEnumerable<(string name, object value)> value) props)
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
