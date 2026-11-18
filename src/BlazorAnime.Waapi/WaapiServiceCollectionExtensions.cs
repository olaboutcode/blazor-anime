using Microsoft.Extensions.DependencyInjection;

namespace BlazorAnime;

public static class WaapiServiceCollectionExtensions
{
    /// <summary>Registers <see cref="IWaapi"/>. Referencing this package loads its script.</summary>
    public static IServiceCollection AddBlazorAnimeWaapi(this IServiceCollection services) =>
        services.AddTransient<IWaapi, WaapiService>();
}
