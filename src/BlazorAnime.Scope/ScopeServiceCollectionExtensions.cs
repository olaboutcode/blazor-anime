using Microsoft.Extensions.DependencyInjection;

namespace BlazorAnime;

public static class ScopeServiceCollectionExtensions
{
    /// <summary>Registers <see cref="IScope"/>. Referencing this package loads its script.</summary>
    public static IServiceCollection AddBlazorAnimeScope(this IServiceCollection services) =>
        services.AddTransient<IScope, ScopeService>();
}
