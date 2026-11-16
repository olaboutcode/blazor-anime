using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// Bridges a draggable callback to JavaScript without requiring
/// <see cref="JSInvokableAttribute"/> on the caller's method.
/// The snapshot is a copy. anime.js does not wait for the .NET call.
/// </summary>
internal sealed class DragCallbackRelay : IDisposable
{
    private readonly Action<DragState> _callback;
    private readonly DotNetObjectReference<DragCallbackRelay> _reference;

    public DragCallbackRelay(Action<DragState> callback)
    {
        _callback = callback;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<DragCallbackRelay> Reference => _reference;

    [JSInvokable]
    public void Invoke(DragState state) => _callback(state);

    public void Dispose() => _reference.Dispose();
}
