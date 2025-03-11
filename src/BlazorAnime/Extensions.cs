namespace BlazorAnime;

using Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    /// <summary>
    /// Register Blazor.Anime services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBlazorAnime(this IServiceCollection services)
    {
        return services.AddTransient<IAnime, Anime>();
    }
}

internal static class InternalExtensions
{
    internal static System.Dynamic.ExpandoObject ToObject(this IEnumerable<Prop> props)
    {
        Dictionary<string, Prop> properties = new();
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
    
    internal static System.Dynamic.ExpandoObject ToObject(this IEnumerable<StgOptionProp> props)
    {
        Dictionary<string, StgOptionProp> properties = new();
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