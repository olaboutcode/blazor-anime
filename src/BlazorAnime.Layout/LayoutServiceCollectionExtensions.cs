using Microsoft.Extensions.DependencyInjection;

namespace BlazorAnime;

public static class LayoutServiceCollectionExtensions
{
    /// <summary>Registers <see cref="ILayout"/>. Referencing this package loads its script.</summary>
    public static IServiceCollection AddBlazorAnimeLayout(this IServiceCollection services) =>
        services.AddTransient<ILayout, LayoutService>();
}
