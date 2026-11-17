using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// One <c>onScroll</c> observer.
/// Pass <see cref="Handle"/> to <c>AutoPlay</c>. anime.js links that object and does not start playback.
/// </summary>
public sealed class ScrollObserver : IAsyncDisposable
{
    /// <summary>The ScrollObserver object anime.js accepts as <c>autoplay</c>.</summary>
    public IJSObjectReference Handle { get; }

    /// <summary>Links an animation or timeline that was created without this observer as autoplay.</summary>
    public Task Link(Animation animation)
    {
        ArgumentNullException.ThrowIfNull(animation);
        return Js.InvokeVoidAsync("link", animation.Instance).AsTask();
    }

    public Task Refresh() => Js.InvokeVoidAsync("refresh").AsTask();

    public Task Revert() => Js.InvokeVoidAsync("revert").AsTask();

    public Task<double> Progress() => Js.InvokeAsync<double>("progress").AsTask();

    public Task<bool> IsInView() => Js.InvokeAsync<bool>("isInView").AsTask();

    public Task<bool> Repeat() => Js.InvokeAsync<bool>("repeat").AsTask();

    public Task<bool> Horizontal() => Js.InvokeAsync<bool>("horizontal").AsTask();

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        _disposed = true;
        try
        {
            await Js.InvokeVoidAsync("revert");
        }
        catch (JSException)
        {
        }
        catch (JSDisconnectedException)
        {
        }
        catch (ObjectDisposedException)
        {
        }

        foreach (var callback in _callbacks)
            callback.Dispose();

        try
        {
            await Handle.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }

        try
        {
            await Js.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }

    internal ScrollObserver(IJSObjectReference js, IJSObjectReference handle, IEnumerable<IDisposable> callbacks)
    {
        Js = js;
        Handle = handle;
        _callbacks = [.. callbacks];
    }

    private readonly IJSObjectReference Js;
    private readonly List<IDisposable> _callbacks;
    private bool _disposed;
}

internal sealed class ScrollCallbackRelay : IDisposable
{
    private readonly Action<ScrollState> _callback;
    private readonly DotNetObjectReference<ScrollCallbackRelay> _reference;
    private bool _disposed;

    public ScrollCallbackRelay(Action<ScrollState> callback)
    {
        _callback = callback;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<ScrollCallbackRelay> Reference => _reference;

    [JSInvokable]
    public void Run(ScrollState? state)
    {
        if (_disposed)
            return;
        _callback(state ?? new ScrollState());
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        try
        {
            _reference.Dispose();
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
