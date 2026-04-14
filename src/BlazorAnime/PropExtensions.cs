namespace BlazorAnime;

public static class PropExtensions
{
    internal static Dictionary<string, object> ToObject(this IEnumerable<Prop> props)
    {
        Dictionary<string, object> properties = [];
        foreach (var prop in props)
        {
            properties[prop.GetName()] = prop.GetValue();
        }
        return properties;
    }
}