namespace BlazorAnime;

/// <summary>
/// Settings for <c>splitText</c>. Omitted keys stay omitted, so anime.js keeps its defaults.
/// An omitted <c>words</c> value still splits words.
/// </summary>
public sealed class TextSplitBuilder
{
    public TextSplitBuilder Lines(bool enabled)
    {
        _settings["lines"] = enabled;
        return this;
    }

    public TextSplitBuilder Lines(string template) => Template("lines", template);

    public TextSplitBuilder Lines(Action<TextSplitPart> configure) => Part("lines", configure);

    public TextSplitBuilder Words(bool enabled)
    {
        _settings["words"] = enabled;
        return this;
    }

    public TextSplitBuilder Words(string template) => Template("words", template);

    public TextSplitBuilder Words(Action<TextSplitPart> configure) => Part("words", configure);

    public TextSplitBuilder Chars(bool enabled)
    {
        _settings["chars"] = enabled;
        return this;
    }

    public TextSplitBuilder Chars(string template) => Template("chars", template);

    public TextSplitBuilder Chars(Action<TextSplitPart> configure) => Part("chars", configure);

    public TextSplitBuilder IncludeSpaces(bool includeSpaces)
    {
        _settings["includeSpaces"] = includeSpaces;
        return this;
    }

    public TextSplitBuilder Accessible(bool accessible)
    {
        _settings["accessible"] = accessible;
        return this;
    }

    public TextSplitBuilder Debug(bool debug)
    {
        _settings["debug"] = debug;
        return this;
    }

    internal Dictionary<string, object> Build() => _settings;

    private TextSplitBuilder Template(string key, string template)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(template);
        if (!template.Contains("{value}", StringComparison.Ordinal))
            throw new ArgumentException("A split template must contain {value}.", nameof(template));
        _settings[key] = template;
        return this;
    }

    private TextSplitBuilder Part(string key, Action<TextSplitPart> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var part = new TextSplitPart();
        configure(part);
        if (part.Settings.Count == 0)
            throw new ArgumentException("A split part needs a class, wrap, or clone.", nameof(configure));
        _settings[key] = part.Settings;
        return this;
    }

    private readonly Dictionary<string, object> _settings = [];
}

/// <summary>Class, wrap, and clone for one split level.</summary>
public sealed class TextSplitPart
{
    public TextSplitPart Class(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Settings["class"] = name;
        return this;
    }

    public TextSplitPart Class(bool enabled)
    {
        if (enabled)
            throw new ArgumentOutOfRangeException(nameof(enabled), "Pass false or a class name.");
        Settings["class"] = false;
        return this;
    }

    public TextSplitPart Wrap(bool wrap)
    {
        Settings["wrap"] = wrap;
        return this;
    }

    public TextSplitPart Wrap(string wrap)
    {
        if (wrap is not ("hidden" or "clip" or "visible" or "scroll" or "auto"))
            throw new ArgumentOutOfRangeException(nameof(wrap));
        Settings["wrap"] = wrap;
        return this;
    }

    public TextSplitPart Clone(bool clone)
    {
        Settings["clone"] = clone;
        return this;
    }

    public TextSplitPart Clone(string clone)
    {
        if (clone is not ("top" or "right" or "bottom" or "left" or "center"))
            throw new ArgumentOutOfRangeException(nameof(clone));
        Settings["clone"] = clone;
        return this;
    }

    internal Dictionary<string, object> Settings { get; } = [];
}
