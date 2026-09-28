using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// A plain JavaScript object anime.js can animate.
/// Property names are used as-is. Read a property back while or after it animates.
/// </summary>
public sealed class JsTarget : IAsyncDisposable
{
    internal JsTarget(IJSObjectReference reference) => Reference = reference;

    internal IJSObjectReference Reference { get; }

    public async Task<double> GetNumber(string propertyName) =>
        await Reference.InvokeAsync<double>("readNumber", propertyName);

    public async Task<string> GetString(string propertyName) =>
        await Reference.InvokeAsync<string>("readString", propertyName);

    public async ValueTask DisposeAsync() => await Reference.DisposeAsync();
}
