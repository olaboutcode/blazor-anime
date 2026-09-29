using Microsoft.Extensions.DependencyInjection;

namespace BlazorAnime;

public static class AnimatableServiceCollectionExtensions
{
    /// <summary>Registers <see cref="IAnimatable"/>. Referencing this package loads its script.</summary>
    public static IServiceCollection AddBlazorAnimeAnimatable(this IServiceCollection services) =>
        services.AddTransient<IAnimatable, AnimatableService>();
}
