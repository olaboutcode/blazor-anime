using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// One <c>createScope</c> instance.
/// <see cref="Add"/> enters the scope, awaits the constructor, then leaves.
/// Instances created during that await are registered on this scope.
/// </summary>
public sealed class Scope : IAsyncDisposable
{
    /// <summary>Runs now, and again on <see cref="Refresh"/> and when a media query changes.</summary>
    public async Task Add(Func<ScopeContext, Task> constructor)
    {
        ArgumentNullException.ThrowIfNull(constructor);
        var relay = new ScopeConstructorRelay(this, constructor);
        Track(relay);
        await Js.InvokeVoidAsync("add", relay.Reference);
    }

    /// <summary>Runs once. Its instances stay across <see cref="Refresh"/> and are reverted on <see cref="Revert"/>.</summary>
    public async Task AddOnce(Func<ScopeContext, Task> constructor)
    {
        ArgumentNullException.ThrowIfNull(constructor);
        var relay = new ScopeConstructorRelay(this, constructor);
        Track(relay);
        await Js.InvokeVoidAsync("addOnce", relay.Reference);
    }

    /// <summary>Registers a method that runs inside the scope when <see cref="Invoke"/> calls it.</summary>
    public Task AddMethod(string name, Func<ScopeCall, Task> method)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(method);
        var relay = new ScopeMethodRelay(method);
        Track(relay);
        return Js.InvokeVoidAsync("addMethod", name, relay.Reference).AsTask();
    }

    /// <summary>Enters the scope and runs the method registered with <see cref="AddMethod"/>.</summary>
    public Task Invoke(string name, ScopeCall? call = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Js.InvokeVoidAsync("invoke", name, call).AsTask();
    }

    /// <summary>Reverts repeating instances, runs their cleanups, and runs each <see cref="Add"/> constructor again.</summary>
    public Task Refresh() => Js.InvokeVoidAsync("refresh").AsTask();

    /// <summary>Reverts every registered instance, runs every cleanup, and drops the media listeners.</summary>
    public Task Revert() => Js.InvokeVoidAsync("revert").AsTask();

    /// <summary>The root element. The reference is kept until disposal.</summary>
    public async Task<IJSObjectReference?> Root()
    {
        if (_root is not null)
            return _root;
        _root = await Js.InvokeAsync<IJSObjectReference?>("root");
        return _root;
    }

    /// <summary>The latest media query matches.</summary>
    public async Task<IReadOnlyDictionary<string, bool>> Matches()
    {
        var matches = await Js.InvokeAsync<Dictionary<string, bool>>("matches");
        return matches ?? new Dictionary<string, bool>();
    }

    /// <summary>Reverts the scope and releases the JavaScript handles.</summary>
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

        foreach (var relay in _relays)
            relay.Dispose();
        _relays.Clear();

        try
        {
            if (_root is not null)
                await _root.DisposeAsync();
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

    internal Scope(IJSObjectReference js)
    {
        Js = js;
        State = new ScopeState(
            key => Js.InvokeAsync<int>("stateGet", key).AsTask(),
            (key, value) => Js.InvokeVoidAsync("stateSet", key, value).AsTask());
    }

    internal void Track(IDisposable relay) => _relays.Add(relay);
    internal ScopeState State { get; }
    private IJSObjectReference Js { get; }
    private IJSObjectReference? _root;
    private bool _disposed;
    private readonly List<IDisposable> _relays = [];
}

internal sealed class ScopeConstructorRelay : IDisposable
{
    private readonly Scope _scope;
    private readonly Func<ScopeContext, Task> _constructor;
    private readonly DotNetObjectReference<ScopeConstructorRelay> _reference;
    private bool _disposed;

    public ScopeConstructorRelay(Scope scope, Func<ScopeContext, Task> constructor)
    {
        _scope = scope;
        _constructor = constructor;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<ScopeConstructorRelay> Reference => _reference;

    [JSInvokable]
    public async Task<DotNetObjectReference<ScopeCleanupRelay>[]> Run(Dictionary<string, bool> matches)
    {
        var context = new ScopeContext(matches ?? new Dictionary<string, bool>(), _scope.State, _scope);
        await _constructor(context);
        return context.TakeCleanups().Select(relay => relay.Reference).ToArray();
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

internal sealed class ScopeCleanupRelay : IDisposable
{
    private readonly Func<Task> _cleanup;
    private readonly DotNetObjectReference<ScopeCleanupRelay> _reference;
    private bool _disposed;

    public ScopeCleanupRelay(Func<Task> cleanup)
    {
        _cleanup = cleanup;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<ScopeCleanupRelay> Reference => _reference;

    [JSInvokable]
    public Task Run(Dictionary<string, bool>? matches = null) => _cleanup();

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

internal sealed class ScopeMethodRelay : IDisposable
{
    private readonly Func<ScopeCall, Task> _method;
    private readonly DotNetObjectReference<ScopeMethodRelay> _reference;
    private bool _disposed;

    public ScopeMethodRelay(Func<ScopeCall, Task> method)
    {
        _method = method;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<ScopeMethodRelay> Reference => _reference;

    [JSInvokable]
    public Task Run(ScopeCall? call) => _method(call ?? new ScopeCall());

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
