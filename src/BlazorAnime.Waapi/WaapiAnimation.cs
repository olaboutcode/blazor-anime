using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// One <c>waapi.animate</c> animation. It runs on the browser timeline and does not follow <c>engine.speed</c>.
/// Pass <see cref="Handle"/> to <c>Timeline.Sync</c>.
/// </summary>
public sealed class WaapiAnimation : IAsyncDisposable
{
    /// <summary>The object <c>timeline.sync</c> accepts. It is not thenable.</summary>
    public IJSObjectReference Handle { get; }

    public Task Play() => Js.InvokeVoidAsync("play").AsTask();
    public Task Pause() => Js.InvokeVoidAsync("pause").AsTask();
    public Task Resume() => Js.InvokeVoidAsync("resume").AsTask();
    public Task Reverse() => Js.InvokeVoidAsync("reverse").AsTask();
    public Task Alternate() => Js.InvokeVoidAsync("alternate").AsTask();
    public Task Restart() => Js.InvokeVoidAsync("restart").AsTask();
    public Task Seek(double time) => Js.InvokeVoidAsync("seek", time).AsTask();
    public Task Complete() => Js.InvokeVoidAsync("complete").AsTask();
    public Task Cancel() => Js.InvokeVoidAsync("cancel").AsTask();
    public Task Revert() => Js.InvokeVoidAsync("revert").AsTask();
    public Task CommitStyles() => Js.InvokeVoidAsync("commitStyles").AsTask();

    /// <summary>Resolves when the animation finishes. An infinite loop never resolves.</summary>
    public Task Finished() => Js.InvokeAsync<object?>("whenFinished").AsTask();

    public Task<double> Duration() => Js.InvokeAsync<double>("duration").AsTask();
    public Task<double> CurrentTime() => Js.InvokeAsync<double>("currentTime").AsTask();
    public Task<double> Progress() => Js.InvokeAsync<double>("progress").AsTask();
    public Task<double> Speed() => Js.InvokeAsync<double>("speed").AsTask();
    public Task SetSpeed(double speed) => Js.InvokeVoidAsync("setSpeed", speed).AsTask();
    public Task<bool> Paused() => Js.InvokeAsync<bool>("paused").AsTask();
    public Task<bool> Completed() => Js.InvokeAsync<bool>("completed").AsTask();
    public Task<bool> Reversed() => Js.InvokeAsync<bool>("reversed").AsTask();
    public Task<bool> Persist() => Js.InvokeAsync<bool>("persist").AsTask();

    public async Task<IReadOnlyList<IJSObjectReference>> Targets()
    {
        var targets = await Js.InvokeAsync<IJSObjectReference[]>("targets");
        return targets ?? [];
    }

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

    internal WaapiAnimation(IJSObjectReference js, IJSObjectReference handle, IEnumerable<IDisposable> callbacks)
    {
        Js = js;
        Handle = handle;
        _callbacks = [.. callbacks];
    }

    private readonly IJSObjectReference Js;
    private readonly List<IDisposable> _callbacks;
    private bool _disposed;
}

internal sealed class WaapiCallbackRelay : IDisposable
{
    private readonly Action<WaapiState> _callback;
    private readonly DotNetObjectReference<WaapiCallbackRelay> _reference;
    private bool _disposed;

    public WaapiCallbackRelay(Action<WaapiState> callback)
    {
        _callback = callback;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<WaapiCallbackRelay> Reference => _reference;

    [JSInvokable]
    public void Run(WaapiState? state)
    {
        if (_disposed)
            return;
        _callback(state ?? new WaapiState());
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
