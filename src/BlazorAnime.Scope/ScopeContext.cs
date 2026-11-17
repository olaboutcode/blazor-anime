namespace BlazorAnime;

/// <summary>The scope state passed to an <c>Add</c> constructor.</summary>
public sealed class ScopeContext
{
    /// <summary>Current media query matches. A missing name is not a match.</summary>
    public IReadOnlyDictionary<string, bool> Matches { get; }

    /// <summary>Integers that survive <c>Refresh</c> and are cleared on <c>Revert</c>.</summary>
    public ScopeState State { get; }

    /// <summary>Runs after the repeating instances are reverted, before the constructor runs again.</summary>
    public void OnCleanup(Action cleanup)
    {
        ArgumentNullException.ThrowIfNull(cleanup);
        OnCleanup(() =>
        {
            cleanup();
            return Task.CompletedTask;
        });
    }

    /// <summary>Runs after the repeating instances are reverted, before the constructor runs again.</summary>
    public void OnCleanup(Func<Task> cleanup)
    {
        ArgumentNullException.ThrowIfNull(cleanup);
        var relay = new ScopeCleanupRelay(cleanup);
        _scope.Track(relay);
        _cleanups.Add(relay);
    }

    internal ScopeContext(IReadOnlyDictionary<string, bool> matches, ScopeState state, Scope scope)
    {
        Matches = matches;
        State = state;
        _scope = scope;
    }

    internal ScopeCleanupRelay[] TakeCleanups() => _cleanups.ToArray();

    private readonly Scope _scope;
    private readonly List<ScopeCleanupRelay> _cleanups = [];
}

/// <summary>Click position passed to a scope method. There is no DOM event.</summary>
public sealed class ScopeCall
{
    public double ClientX { get; init; }
    public double ClientY { get; init; }

    public ScopeCall()
    {
    }

    public ScopeCall(double clientX, double clientY)
    {
        ClientX = clientX;
        ClientY = clientY;
    }
}

/// <summary>A string-to-int map kept on the scope.</summary>
public sealed class ScopeState
{
    public Task<int> Get(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _get(key);
    }

    public Task Set(string key, int value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _set(key, value);
    }

    internal ScopeState(Func<string, Task<int>> get, Func<string, int, Task> set)
    {
        _get = get;
        _set = set;
    }

    private readonly Func<string, Task<int>> _get;
    private readonly Func<string, int, Task> _set;
}
