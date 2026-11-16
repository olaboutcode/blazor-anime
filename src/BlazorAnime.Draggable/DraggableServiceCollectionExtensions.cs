using Microsoft.Extensions.DependencyInjection;

namespace BlazorAnime;

public static class DraggableServiceCollectionExtensions
{
    /// <summary>Registers <see cref="IDraggable"/>. Referencing this package loads its script.</summary>
    public static IServiceCollection AddBlazorAnimeDraggable(this IServiceCollection services) =>
        services.AddTransient<IDraggable, DraggableService>();
}
