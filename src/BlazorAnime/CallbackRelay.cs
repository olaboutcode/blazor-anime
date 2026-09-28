using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// Bridges a C# animation callback to JavaScript without requiring
/// <see cref="JSInvokableAttribute"/> on the caller's method.
/// </summary>
internal sealed class StateCallbackRelay : IDisposable
{
    private readonly Action<AnimationState> _callback;
    private readonly DotNetObjectReference<StateCallbackRelay> _reference;

    public StateCallbackRelay(Action<AnimationState> callback)
    {
        _callback = callback;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<StateCallbackRelay> Reference => _reference;

    [JSInvokable]
    public void Invoke(AnimationState state) => _callback(state);

    public void Dispose() => _reference.Dispose();
}

/// <summary>
/// Evaluates a function-based anime.js value once per target when the animation is created.
/// anime.js calls these functions synchronously, so the values are collected in one round trip
/// and replayed from JavaScript.
/// </summary>
internal sealed class ValueCallbackRelay : IDisposable
{
    private readonly Func<int, int, object?> _callback;
    private readonly DotNetObjectReference<ValueCallbackRelay> _reference;

    public ValueCallbackRelay(Func<int, int, object?> callback)
    {
        _callback = callback;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<ValueCallbackRelay> Reference => _reference;

    [JSInvokable]
    public object?[] InvokeAll(int total)
    {
        var values = new object?[total];
        for (var index = 0; index < total; index++)
            values[index] = _callback(index, total);
        return values;
    }

    public void Dispose() => _reference.Dispose();
}
