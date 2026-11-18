using Microsoft.Extensions.DependencyInjection;

namespace BlazorAnime;

public static class TextServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorAnimeText(this IServiceCollection services) =>
        services.AddTransient<IText, TextService>();
}
