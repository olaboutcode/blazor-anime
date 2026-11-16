using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// One <c>createDraggable</c> instance. Commands return when the call has been sent.
/// They do not wait on the release animation.
/// </summary>
public sealed class Draggable : IAsyncDisposable
{
    public Task Disable() => Js.InvokeVoidAsync("disable").AsTask();
    public Task Enable() => Js.InvokeVoidAsync("enable").AsTask();

    /// <summary>Sets the horizontal position. <paramref name="muteUpdateCallback"/> skips <c>onUpdate</c>.</summary>
    public Task SetX(double x, bool muteUpdateCallback = false) =>
        Js.InvokeVoidAsync("setX", x, muteUpdateCallback).AsTask();

    /// <summary>Sets the vertical position. <paramref name="muteUpdateCallback"/> skips <c>onUpdate</c>.</summary>
    public Task SetY(double y, bool muteUpdateCallback = false) =>
        Js.InvokeVoidAsync("setY", y, muteUpdateCallback).AsTask();

    public Task Stop() => Js.InvokeVoidAsync("stop").AsTask();

    /// <summary>Stops, clears velocity, and returns both axes to 0.</summary>
    public Task Reset() => Js.InvokeVoidAsync("reset").AsTask();

    /// <summary>Re-reads container bounds, snap, and cursor settings.</summary>
    public Task Refresh() => Js.InvokeVoidAsync("refresh").AsTask();

    /// <summary>Resets the drag, removes the pointer listeners, and restores styles.</summary>
    public Task Revert() => Js.InvokeVoidAsync("revert").AsTask();

    /// <summary>
    /// Scrolls the container until the target is in view.
    /// A null argument keeps the anime.js default.
    /// </summary>
    public Task ScrollInView(int? durationMilliseconds = null, double? gap = null, Easing? ease = null) =>
        View("scrollInView", durationMilliseconds, gap, ease);

    /// <summary>
    /// Animates the target back inside the container.
    /// A null argument keeps the anime.js default.
    /// </summary>
    public Task AnimateInView(int? durationMilliseconds = null, double? gap = null, Easing? ease = null) =>
        View("animateInView", durationMilliseconds, gap, ease);

    public Task<double> X() => Js.InvokeAsync<double>("x").AsTask();
    public Task<double> Y() => Js.InvokeAsync<double>("y").AsTask();
    public Task<double> ProgressX() => Js.InvokeAsync<double>("progressX").AsTask();
    public Task<double> ProgressY() => Js.InvokeAsync<double>("progressY").AsTask();
    public Task<double> Velocity() => Js.InvokeAsync<double>("velocity").AsTask();
    public Task<double> Angle() => Js.InvokeAsync<double>("angle").AsTask();
    public Task<double> DestX() => Js.InvokeAsync<double>("destX").AsTask();
    public Task<double> DestY() => Js.InvokeAsync<double>("destY").AsTask();
    public Task<double> DeltaX() => Js.InvokeAsync<double>("deltaX").AsTask();
    public Task<double> DeltaY() => Js.InvokeAsync<double>("deltaY").AsTask();
    public Task<bool> Enabled() => Js.InvokeAsync<bool>("enabled").AsTask();
    public Task<bool> Grabbed() => Js.InvokeAsync<bool>("grabbed").AsTask();
    public Task<bool> Dragged() => Js.InvokeAsync<bool>("dragged").AsTask();
    public Task<bool> Contained() => Js.InvokeAsync<bool>("contained").AsTask();
    public Task<string> XProp() => Js.InvokeAsync<string>("xProp").AsTask();
    public Task<string> YProp() => Js.InvokeAsync<string>("yProp").AsTask();

    /// <summary>The dragged element. The reference is kept until disposal.</summary>
    public Task<IJSObjectReference?> Target() => Element("target");

    /// <summary>The element that receives the pointer. The reference is kept until disposal.</summary>
    public Task<IJSObjectReference?> Trigger() => Element("trigger");

    /// <summary>The container element. The reference is kept until disposal.</summary>
    public Task<IJSObjectReference?> Container() => Element("container");

    /// <summary>The scrolling element, or the window. The reference is kept until disposal.</summary>
    public Task<IJSObjectReference?> ScrollContainer() => Element("scrollContainer");

    /// <summary>Reverts the drag and releases the JavaScript handles, including pointer listeners.</summary>
    public async ValueTask DisposeAsync()
    {
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

        foreach (var reference in _elements.Values)
            await DisposeReference(reference);
        _elements.Clear();

        try
        {
            await Js.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }

        foreach (var handle in _callbacks)
            handle.Dispose();
    }

    internal Draggable(IJSObjectReference js, IEnumerable<IDisposable> callbacks)
    {
        Js = js;
        _callbacks = [.. callbacks];
    }

    private Task View(string name, int? durationMilliseconds, double? gap, Easing? ease) =>
        Js.InvokeVoidAsync(
            name,
            durationMilliseconds,
            gap,
            ease is null ? null : DraggableBuilder.EaseMarker(ease)).AsTask();

    private async Task<IJSObjectReference?> Element(string name)
    {
        if (_elements.TryGetValue(name, out var cached))
            return cached;

        var reference = await Js.InvokeAsync<IJSObjectReference?>(name);
        if (reference is not null)
            _elements[name] = reference;
        return reference;
    }

    private static async Task DisposeReference(IJSObjectReference reference)
    {
        try
        {
            await reference.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private IJSObjectReference Js { get; }
    private readonly List<IDisposable> _callbacks;
    private readonly Dictionary<string, IJSObjectReference> _elements = [];
}
