namespace BlazorAnime;

using Microsoft.JSInterop;

public abstract partial class Prop
{
    public Prop(string name, object value)
    {
        Checks.EnsureValidProp(name, value);
        Name = LowerFirstChar(name);
        _value = value;
    }

    protected string Name { get; init; }
    private object _value { get; init; }
    
    protected object GetPrimValue() => _value;
    
    public string GetName() => Name;

    public virtual object GetValue()
    {
        return new
        {
            name = Name,
            value = new { propType = "setter", value = _value }
        };
    }

    private static string LowerFirstChar(string input) =>
        string.IsNullOrEmpty(input)
        ? input
        : char.ToLower(input[0]) + input[1..];
}

public class GenProp<T>(string name, T value) : Prop(name, value) where T : notnull;

public class SvgProp(string name, IJSObjectReference value) : Prop(name, value)
{
    public override object GetValue()
    {
        return new
        {
            name = Name,
            value = new
            {
                propType = "svgSetter",
                value = GetPrimValue()
            }
        };
    }
}

public class StgProp(string name, object value, object options) : Prop(name, value)
{
    public override object GetValue()
    {
        return new
        {
            name = Name,
            value = new
            {
                propType = "stagger",
                value = new { value = GetPrimValue(), options }
            }
        };
    }
}

public class StgOptionProp(string name, object value) : Prop(name, value)
{
    public override object GetValue()
    {
        return new
        {
            name = Name,
            value = new { propType = "setter", value = GetPrimValue() }
        };
    }
}

public class CallbackProp(string name, string callbackName, int paramCount, object dotNetRef) : Prop(name, callbackName)
{
    public override object GetValue()
    {
        return new
        {
            name = Name,
            value = new
            {
                propType = "callback",
                paramCount,
                value = new { callback = GetPrimValue(), dotNetRef }
            }
        };
    }
}