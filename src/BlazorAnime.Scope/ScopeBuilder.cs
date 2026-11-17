using Microsoft.AspNetCore.Components;

namespace BlazorAnime;

/// <summary>Parameters for <c>createScope</c>. The root limits selectors. Defaults apply to instances created while the scope is entered.</summary>
public sealed class ScopeBuilder
{
    /// <summary>Selectors resolve inside this element. Omit it to use the document.</summary>
    public ScopeBuilder Root(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        RootValue = selector;
        HasRoot = true;
        return this;
    }

    /// <summary>Selectors resolve inside this element.</summary>
    public ScopeBuilder Root(ElementReference element)
    {
        RootValue = element;
        HasRoot = true;
        return this;
    }

    /// <summary>
    /// Named media query. <paramref name="name"/> is the key on <see cref="ScopeContext.Matches"/>.
    /// <paramref name="query"/> is the media query string, for example <c>(max-width: 200px)</c>.
    /// </summary>
    public ScopeBuilder MediaQuery(string name, string query)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        MediaQueries[name] = query;
        return this;
    }

    /// <summary>Value defaults for timers, animations, and timelines created inside the scope.</summary>
    public ScopeBuilder Defaults(Action<ScopeDefaultsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new ScopeDefaultsBuilder();
        configure(builder);
        DefaultsValue = builder.Build();
        return this;
    }

    internal object? RootValue { get; private set; }
    internal bool HasRoot { get; private set; }
    internal Dictionary<string, string> MediaQueries { get; } = [];
    internal Dictionary<string, object>? DefaultsValue { get; private set; }

    internal Dictionary<string, object> Build()
    {
        var result = new Dictionary<string, object>();
        if (RootValue is string selector)
            result["root"] = selector;
        if (MediaQueries.Count > 0)
            result["mediaQueries"] = new Dictionary<string, string>(MediaQueries);
        if (DefaultsValue is { Count: > 0 })
            result["defaults"] = DefaultsValue;
        return result;
    }
}

/// <summary>Value parameters copied onto instances created while a scope is entered.</summary>
public sealed class ScopeDefaultsBuilder
{
    public ScopeDefaultsBuilder Duration(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds), "Duration must be zero or greater.");
        Values["duration"] = milliseconds;
        return this;
    }

    public ScopeDefaultsBuilder Delay(int milliseconds)
    {
        Values["delay"] = milliseconds;
        return this;
    }

    public ScopeDefaultsBuilder Loop(bool loop)
    {
        Values["loop"] = loop;
        return this;
    }

    public ScopeDefaultsBuilder Loop(int count)
    {
        Values["loop"] = count;
        return this;
    }

    public ScopeDefaultsBuilder LoopDelay(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds), "Loop delay must be zero or greater.");
        Values["loopDelay"] = milliseconds;
        return this;
    }

    public ScopeDefaultsBuilder Reversed(bool reversed = true)
    {
        Values["reversed"] = reversed;
        return this;
    }

    public ScopeDefaultsBuilder Alternate(bool alternate = true)
    {
        Values["alternate"] = alternate;
        return this;
    }

    public ScopeDefaultsBuilder Autoplay(bool autoplay)
    {
        Values["autoplay"] = autoplay;
        return this;
    }

    public ScopeDefaultsBuilder PlaybackRate(double rate)
    {
        if (!double.IsFinite(rate))
            throw new ArgumentOutOfRangeException(nameof(rate), "Playback rate must be finite.");
        Values["playbackRate"] = rate;
        return this;
    }

    public ScopeDefaultsBuilder FrameRate(double rate)
    {
        if (!double.IsFinite(rate) || rate < 0)
            throw new ArgumentOutOfRangeException(nameof(rate), "Frame rate must be zero or greater.");
        Values["frameRate"] = rate;
        return this;
    }

    public ScopeDefaultsBuilder Ease(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        Values["ease"] = EaseMarker(easing);
        return this;
    }

    public ScopeDefaultsBuilder PlaybackEase(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        Values["playbackEase"] = EaseMarker(easing);
        return this;
    }

    /// <summary><paramref name="composition"/> is <c>none</c>, <c>replace</c>, or <c>blend</c>.</summary>
    public ScopeDefaultsBuilder Composition(string composition)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(composition);
        if (composition is not ("none" or "replace" or "blend"))
            throw new ArgumentOutOfRangeException(nameof(composition), "Composition must be none, replace, or blend.");
        Values["composition"] = composition;
        return this;
    }

    internal Dictionary<string, object> Build() => Values;
    private Dictionary<string, object> Values { get; } = [];

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
}
