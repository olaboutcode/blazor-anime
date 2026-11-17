using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// One <c>createLayout</c> instance.
/// <see cref="Record"/> measures the current DOM. Render the next markup, then call <see cref="Animate"/>.
/// Commands return when the call has been sent. They do not wait on the timeline.
/// </summary>
public sealed class Layout : IAsyncDisposable
{
    /// <summary>Measures the current DOM and keeps it as the previous state.</summary>
    public Task Record() => Js.InvokeVoidAsync("record").AsTask();

    /// <summary>Measures the DOM again and plays the transition with the settings from <c>Create</c>.</summary>
    public Task Animate() => Js.InvokeVoidAsync("animate", null).AsTask();

    /// <summary>
    /// Measures the DOM again and plays the transition.
    /// Settings here override the ones from <c>Create</c> for this run.
    /// </summary>
    public async Task Animate(Func<LayoutBuilder, LayoutBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(new LayoutBuilder()) ?? throw new ArgumentNullException(nameof(configure));
        var added = builder.Callbacks;
        _callbacks.AddRange(added);
        try
        {
            await Js.InvokeVoidAsync("animate", builder.Build());
        }
        catch
        {
            foreach (var handle in added)
            {
                handle.Dispose();
                _callbacks.Remove(handle);
            }

            throw;
        }
    }

    /// <summary>Completes the running transition and restores measured styles.</summary>
    public Task Revert() => Js.InvokeVoidAsync("revert").AsTask();

    /// <summary>The root element. The reference is kept until disposal.</summary>
    public async Task<IJSObjectReference?> Root()
    {
        if (_root is not null)
            return _root;
        _root = await Js.InvokeAsync<IJSObjectReference?>("root");
        return _root;
    }

    /// <summary>Id passed to <c>Create</c>, or the id anime.js assigned.</summary>
    public Task<string> Id() => Js.InvokeAsync<string>("id").AsTask();

    /// <summary>
    /// Elements that appeared in the latest <see cref="Animate"/>.
    /// The next read of this list releases the previous references.
    /// </summary>
    public Task<IReadOnlyList<IJSObjectReference>> Entering() => Elements("entering");

    /// <summary>Elements that disappeared in the latest <see cref="Animate"/>.</summary>
    public Task<IReadOnlyList<IJSObjectReference>> Leaving() => Elements("leaving");

    /// <summary>Elements that swapped in the latest <see cref="Animate"/>.</summary>
    public Task<IReadOnlyList<IJSObjectReference>> Swapping() => Elements("swapping");

    /// <summary>Elements animated by the latest <see cref="Animate"/>.</summary>
    public Task<IReadOnlyList<IJSObjectReference>> Animating() => Elements("animating");

    /// <summary>Reverts the layout and releases the JavaScript handles.</summary>
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

        if (_root is not null)
            await DisposeReference(_root);
        foreach (var reference in _lists.Values.SelectMany(list => list))
            await DisposeReference(reference);
        _lists.Clear();

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

    internal Layout(IJSObjectReference js, IEnumerable<IDisposable> callbacks)
    {
        Js = js;
        _callbacks = [.. callbacks];
    }

    private async Task<IReadOnlyList<IJSObjectReference>> Elements(string name)
    {
        if (_lists.TryGetValue(name, out var previous))
        {
            foreach (var reference in previous)
                await DisposeReference(reference);
        }

        var next = await Js.InvokeAsync<IJSObjectReference[]>(name) ?? [];
        _lists[name] = next;
        return next;
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
    private readonly Dictionary<string, IJSObjectReference[]> _lists = [];
    private IJSObjectReference? _root;
}
