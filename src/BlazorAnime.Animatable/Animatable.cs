using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// Property getters and setters from <c>createAnimatable</c>.
/// Values are numbers, or arrays of numbers for colors.
/// </summary>
public sealed class Animatable : IAsyncDisposable
{
    /// <summary>Current value. Throws when the property stores several numbers.</summary>
    public async Task<double> Get(string property)
    {
        var read = await Read(property);
        if (read.Many)
            throw new InvalidOperationException($"'{property}' has several values. Call GetValues.");
        return read.Values[0];
    }

    /// <summary>Current values. A single number comes back as a one-element array.</summary>
    public async Task<double[]> GetValues(string property) => (await Read(property)).Values;

    /// <summary>Animate the property to <paramref name="value"/>.</summary>
    public Task Set(string property, double value, int? durationMilliseconds = null, Easing? ease = null) =>
        Write(property, value, durationMilliseconds, ease);

    /// <summary>Animate the property to several numbers, for example an RGB color.</summary>
    public Task Set(string property, IReadOnlyList<double> values, int? durationMilliseconds = null, Easing? ease = null)
    {
        ArgumentNullException.ThrowIfNull(values);
        return Write(property, values.ToArray(), durationMilliseconds, ease);
    }

    /// <summary>Restores the original values and removes the property methods.</summary>
    public async Task Revert()
    {
        await Js.InvokeVoidAsync("revert");
        await ReleaseHandles();
    }

    /// <summary>
    /// Targets anime.js resolved for this animatable. The list is kept until
    /// <see cref="Revert"/> or disposal.
    /// </summary>
    public Task<IReadOnlyList<IJSObjectReference>> Targets() => _targetsTask ??= LoadTargets();

    /// <summary>
    /// The animation anime.js keeps for each property, keyed by property name.
    /// The map is kept until <see cref="Revert"/> or disposal.
    /// </summary>
    public Task<IReadOnlyDictionary<string, Animation>> Animations() => _animationsTask ??= LoadAnimations();

    /// <summary>Pauses every property animation. Does not restore the original values.</summary>
    public async ValueTask DisposeAsync()
    {
        try
        {
            await Js.InvokeVoidAsync("pause");
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

        await ReleaseHandles();

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

    internal Animatable(IJSObjectReference js, IEnumerable<IDisposable> callbacks)
    {
        Js = js;
        _callbacks = [.. callbacks];
    }

    private async Task<ValueRead> Read(string property)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(property);
        var read = await Js.InvokeAsync<ValueRead>("get", property);
        if (read.Values is not { Length: > 0 })
            throw new InvalidOperationException($"'{property}' has no value.");
        return read;
    }

    private async Task Write(string property, object value, int? durationMilliseconds, Easing? ease)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(property);
        await Js.InvokeVoidAsync(
            "set",
            property,
            value,
            durationMilliseconds,
            ease == null ? null : AnimatableBuilder.EaseMarker(ease));
    }

    private async Task<IReadOnlyList<IJSObjectReference>> LoadTargets()
    {
        var targets = await Js.InvokeAsync<IJSObjectReference[]>("targets");
        return targets ?? [];
    }

    private async Task<IReadOnlyDictionary<string, Animation>> LoadAnimations()
    {
        var listed = await Js.InvokeAsync<AnimationList>("animations");
        var names = listed.Names ?? [];
        var animations = listed.Animations ?? [];
        if (names.Length != animations.Length)
            throw new InvalidOperationException("Animatable returned a mismatched animation list.");

        var map = new Dictionary<string, Animation>(names.Length);
        for (var index = 0; index < names.Length; index++)
            map[names[index]] = new Animation(animations[index], []);
        return map;
    }

    private async Task ReleaseHandles()
    {
        if (_targetsTask is not null)
        {
            var targets = await _targetsTask;
            _targetsTask = null;
            foreach (var target in targets)
                await DisposeReference(target);
        }

        if (_animationsTask is not null)
        {
            var animations = await _animationsTask;
            _animationsTask = null;
            foreach (var animation in animations.Values)
                await animation.DisposeAsync();
        }
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
    private Task<IReadOnlyList<IJSObjectReference>>? _targetsTask;
    private Task<IReadOnlyDictionary<string, Animation>>? _animationsTask;

    private sealed class ValueRead
    {
        public bool Many { get; set; }
        public double[] Values { get; set; } = [];
    }

    private sealed class AnimationList
    {
        public string[] Names { get; set; } = [];
        public IJSObjectReference[] Animations { get; set; } = [];
    }
}
