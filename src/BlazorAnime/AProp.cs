namespace BlazorAnime;

using Microsoft.JSInterop;

public abstract class AProp(string name, object value)
{
    protected string Name { get; } = LowerFirstChar(name);
    protected object Value { get; } = value;

    public string GetName() => Name;

    public virtual object GetValue()
    {
        return new
        {
            name = Name,
            value = new { propType = "setter", value = Value }
        };
    }

    private static string LowerFirstChar(string input) =>
        string.IsNullOrEmpty(input)
        ? input
        : (char.ToLower(input[0]) + input[1..]);
}

public class GenProp<T>(string name, T value) : AProp(name, value) where T : notnull;

public class SvgProp(string name, IJSObjectReference value) : AProp(name, value)
{
    protected new string Name { get; } = name;
    protected new IJSObjectReference Value { get; } = value;

    public override object GetValue()
    {
        return new
        {
            name = Name,
            value = new
            {
                propType = "svgSetter",
                value = Value
            }
        };
    }
}

public class StgProp(string name, object value, object options) : AProp(name, value)
{
    protected object Options { get; } = options;

    public override object GetValue()
    {
        return new
        {
            name = Name,
            value = new
            {
                propType = "stagger",
                value = new { value = Value, options = Options }
            }
        };
    }
}

public class StgOptionProp(string name, object value) : AProp(name, value)
{
    public override object GetValue()
    {
        return new
        {
            name = Name,
            value = new { propType = "setter", value = Value }
        };
    }
}

public class CallbackProp(string name, string callbackName, int paramCount, object dotNetRef) : AProp(name, callbackName)
{
    protected object DotNetRef { get; } = dotNetRef;
    private int ParamCount { get; } = paramCount;

    public override object GetValue()
    {
        return new
        {
            name = Name,
            value = new
            {
                propType = "callback",
                paramCount = ParamCount,
                value = new { callback = Value, dotNetRef = DotNetRef }
            }
        };
    }
}