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