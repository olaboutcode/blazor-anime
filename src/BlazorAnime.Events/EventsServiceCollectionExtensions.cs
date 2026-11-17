using Microsoft.Extensions.DependencyInjection;

namespace BlazorAnime;

public static class EventsServiceCollectionExtensions
{
    /// <summary>Registers <see cref="IEvents"/>. Referencing this package loads its script.</summary>
    public static IServiceCollection AddBlazorAnimeEvents(this IServiceCollection services) =>
        services.AddTransient<IEvents, EventsService>();
}
