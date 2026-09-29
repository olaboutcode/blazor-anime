using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// Proxies from <c>createDrawable</c>. Pass the target to <c>Targets</c> and animate <c>Draw</c>.
/// </summary>
public sealed class DrawableTarget : IAsyncDisposable
{
    internal DrawableTarget(IJSObjectReference reference) => Reference = reference;

    internal IJSObjectReference Reference { get; }

    public async ValueTask DisposeAsync() => await Reference.DisposeAsync();
}
