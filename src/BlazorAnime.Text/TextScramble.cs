using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// The function <c>scrambleText</c> returns. Pass <see cref="Reference"/> to <c>innerHTML</c>.
/// It does not create an animation.
/// </summary>
public sealed class TextScramble : IAsyncDisposable
{
    public IJSObjectReference Reference { get; }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        _disposed = true;
        foreach (var callback in _callbacks)
            callback.Dispose();
        try
        {
            await Reference.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }

    internal TextScramble(IJSObjectReference reference, IEnumerable<IDisposable> callbacks)
    {
        Reference = reference;
        _callbacks = [.. callbacks];
    }

    private readonly List<IDisposable> _callbacks;
    private bool _disposed;
}

/// <summary>Settings for <c>scrambleText</c>. Omitted keys stay omitted.</summary>
public sealed class TextScrambleBuilder
{
    public TextScrambleBuilder Text(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        _settings["text"] = text;
        return this;
    }

    public TextScrambleBuilder Chars(string chars)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chars);
        _settings["chars"] = chars;
        return this;
    }

    public TextScrambleBuilder Ease(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        _settings["ease"] = EaseMarker(easing);
        return this;
    }

    public TextScrambleBuilder From(string origin)
    {
        if (origin is not ("left" or "center" or "right" or "random" or "auto"))
            throw new ArgumentOutOfRangeException(nameof(origin));
        _settings["from"] = origin;
        return this;
    }

    public TextScrambleBuilder From(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));
        _settings["from"] = index;
        return this;
    }

    public TextScrambleBuilder Reversed(bool reversed)
    {
        _settings["reversed"] = reversed;
        return this;
    }

    public TextScrambleBuilder Cursor(bool cursor)
    {
        _settings["cursor"] = cursor;
        return this;
    }

    public TextScrambleBuilder Cursor(int charCode)
    {
        if (charCode < 0)
            throw new ArgumentOutOfRangeException(nameof(charCode));
        _settings["cursor"] = charCode;
        return this;
    }

    public TextScrambleBuilder Cursor(string cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        _settings["cursor"] = cursor;
        return this;
    }

    public TextScrambleBuilder Perturbation(double perturbation)
    {
        if (!double.IsFinite(perturbation) || perturbation < 0 || perturbation > 1)
            throw new ArgumentOutOfRangeException(nameof(perturbation));
        _settings["perturbation"] = perturbation;
        return this;
    }

    public TextScrambleBuilder Seed(double seed)
    {
        if (!double.IsFinite(seed))
            throw new ArgumentOutOfRangeException(nameof(seed));
        _settings["seed"] = seed;
        return this;
    }

    public TextScrambleBuilder Override(bool overrideText)
    {
        _settings["override"] = overrideText;
        return this;
    }

    public TextScrambleBuilder Override(string chars)
    {
        ArgumentNullException.ThrowIfNull(chars);
        _settings["override"] = chars;
        return this;
    }

    public TextScrambleBuilder RevealRate(double charactersPerSecond)
    {
        if (!double.IsFinite(charactersPerSecond) || charactersPerSecond <= 0)
            throw new ArgumentOutOfRangeException(nameof(charactersPerSecond));
        _settings["revealRate"] = charactersPerSecond;
        return this;
    }

    public TextScrambleBuilder SettleDuration(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds));
        _settings["settleDuration"] = milliseconds;
        return this;
    }

    public TextScrambleBuilder SettleRate(double changesPerSecond)
    {
        if (!double.IsFinite(changesPerSecond) || changesPerSecond <= 0)
            throw new ArgumentOutOfRangeException(nameof(changesPerSecond));
        _settings["settleRate"] = changesPerSecond;
        return this;
    }

    public TextScrambleBuilder Duration(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds));
        _settings["duration"] = milliseconds;
        return this;
    }

    public TextScrambleBuilder RevealDelay(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds));
        _settings["revealDelay"] = milliseconds;
        return this;
    }

    public TextScrambleBuilder Delay(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds));
        _settings["delay"] = milliseconds;
        return this;
    }

    public TextScrambleBuilder OnChange(Action<string, double> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        var relay = new TextScrambleRelay(callback);
        Callbacks.Add(relay);
        _settings["onChange"] = relay.Reference;
        return this;
    }

    internal Dictionary<string, object> Build() => _settings;
    internal List<IDisposable> Callbacks { get; } = [];

    internal static object EaseMarker(Easing easing)
    {
        if (easing.IsCurve)
        {
            return new Dictionary<string, object>
            {
                ["propType"] = "easeFn",
                ["value"] = new Dictionary<string, object>
                {
                    ["fn"] = "curve",
                    ["value"] = easing.Samples
                }
            };
        }

        if (easing.IsFunction)
        {
            var spec = easing.FunctionSpec;
            object payload = spec.Fn switch
            {
                "steps" => new Dictionary<string, object> { ["fn"] = "steps", ["steps"] = spec.A ?? 0 },
                "cubicBezier" => new Dictionary<string, object>
                {
                    ["fn"] = "cubicBezier",
                    ["x1"] = spec.A ?? 0,
                    ["y1"] = spec.B ?? 0,
                    ["x2"] = spec.C ?? 0,
                    ["y2"] = spec.D ?? 0
                },
                "spring" when spec.Bounce is not null => new Dictionary<string, object>
                {
                    ["fn"] = "spring",
                    ["bounce"] = spec.Bounce.Value,
                    ["duration"] = spec.Duration ?? 0
                },
                "spring" when spec.A is not null => new Dictionary<string, object>
                {
                    ["fn"] = "spring",
                    ["mass"] = spec.A.Value,
                    ["stiffness"] = spec.B ?? 0,
                    ["damping"] = spec.C ?? 0,
                    ["velocity"] = spec.D ?? 0
                },
                _ => new Dictionary<string, object> { ["fn"] = spec.Fn }
            };
            return new Dictionary<string, object>
            {
                ["propType"] = "easeFn",
                ["value"] = payload
            };
        }

        return easing.GetValue();
    }

    private readonly Dictionary<string, object> _settings = [];
}

internal sealed class TextScrambleRelay : IDisposable
{
    private readonly Action<string, double> _callback;
    private readonly DotNetObjectReference<TextScrambleRelay> _reference;
    private bool _disposed;

    public TextScrambleRelay(Action<string, double> callback)
    {
        _callback = callback;
        _reference = DotNetObjectReference.Create(this);
    }

    public DotNetObjectReference<TextScrambleRelay> Reference => _reference;

    [JSInvokable]
    public void Run(string? text, double progress)
    {
        if (_disposed)
            return;
        _callback(text ?? "", progress);
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
