namespace BlazorAnime;

public static class PropExtensions
{
    internal static System.Dynamic.ExpandoObject ToObject(this IEnumerable<Prop> props)
    {
        Dictionary<string, Prop> properties = [];
        foreach (var prop in props)
        {
            properties[prop.GetName()] = prop;
        }
        dynamic propsObject = new System.Dynamic.ExpandoObject();
        foreach (var property in properties.Values)
        {
            ((IDictionary<string, object>)propsObject).Add(property.GetName(), property.GetValue());
        }
        return propsObject;
    }
}