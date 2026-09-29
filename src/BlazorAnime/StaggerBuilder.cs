namespace BlazorAnime;

public sealed class Stagger
{
    public static Stagger Create(double delay) => new(delay, []);
    public static Stagger Create(double from, double to) => new(new[] { from, to }, []);
    public static Stagger Create(string from, string to) => new(new[] { from, to }, []);
    public static Stagger Create(double value, Action<StaggerOptionsBuilder> configure)
    {
        var builder = new StaggerOptionsBuilder();
        configure(builder);
        return new Stagger(value, builder.Build());
    }
    public static Stagger Create(string value, Action<StaggerOptionsBuilder> configure)
    {
        var builder = new StaggerOptionsBuilder();
        configure(builder);
        return new Stagger(value, builder.Build());
    }

    public static Stagger Create(double from, double to, Action<StaggerOptionsBuilder> configure)
    {
        var builder = new StaggerOptionsBuilder();
        configure(builder);
        return new Stagger(new[] { from, to }, builder.Build());
    }
    public static Stagger Create(string from, string to, Action<StaggerOptionsBuilder> configure)
    {
        var builder = new StaggerOptionsBuilder();
        configure(builder);
        return new Stagger(new[] { from, to }, builder.Build());
    }

    internal static Stagger FromParts(object value, IReadOnlyList<Prop> options) =>
        new(value, options);

    internal IReadOnlyList<Prop> GetOptions() => _options;
    internal object GetValue() => _value;

    private Stagger(object value, IReadOnlyList<Prop> options)
    {
        _options = options;
        _value = value;
    }

    private readonly IReadOnlyList<Prop> _options;
    private readonly object _value;
}

public sealed class StaggerOptionsBuilder
{
    public StaggerOptionsBuilder Start(double value)
    {
        _options.Add(new StgOptionProp("start", value));
        return this;
    }

    public StaggerOptionsBuilder Reversed(bool reversed)
    {
        _options.Add(new StgOptionProp("reversed", reversed));
        return this;
    }

    public StaggerOptionsBuilder Ease(Easing value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _options.Add(PropsBuilder.EaseProp("ease", value));
        return this;
    }

    /// <summary>
    /// anime.js reads <c>grid</c> as <c>[columns, rows]</c>.
    /// </summary>
    public StaggerOptionsBuilder Grid(int columns, int rows)
    {
        _options.Add(new StgOptionProp("grid", new[] { columns, rows }));
        return this;
    }

    /// <summary>
    /// anime.js reads <c>grid</c> as <c>[columns, rows]</c>.
    /// </summary>
    public StaggerOptionsBuilder Grid(int[] columnsAndRows)
    {
        ArgumentNullException.ThrowIfNull(columnsAndRows);
        if (columnsAndRows.Length < 2)
            throw new ArgumentException("Grid expects [columns, rows].", nameof(columnsAndRows));
        return Grid(columnsAndRows[0], columnsAndRows[1]);
    }

    public StaggerOptionsBuilder Axis(StaggerAxis value)
    {
        _options.Add(new StgOptionProp("axis", value.GetValue()));
        return this;
    }

    public StaggerOptionsBuilder From(StaggerPosition value)
    {
        _options.Add(new StgOptionProp("from", value.GetValue()));
        return this;
    }

    public StaggerOptionsBuilder From(int index) => From(StaggerPosition.Index(index));

    internal IReadOnlyList<Prop> Build() => _options.AsReadOnly();
    private readonly List<Prop> _options;
    internal StaggerOptionsBuilder() => _options = [];
}

/// <summary>
/// Names from the anime.js stagger examples. Values are the same as <see cref="StaggerPosition"/>.
/// </summary>
public static class StaggerFrom
{
    public static StaggerPosition First => StaggerPosition.First;
    public static StaggerPosition Last => StaggerPosition.Last;
    public static StaggerPosition Center => StaggerPosition.Center;
    public static StaggerPosition Index(int index) => StaggerPosition.Index(index);
}

/// <summary>
/// Fluent stagger description used by <c>Delay(stagger =&gt; stagger.Value(...))</c>.
/// </summary>
public sealed class StaggerSyntax
{
    private object _value = 0d;
    private readonly StaggerOptionsBuilder _options = new();
    private bool _hasOptions;

    public StaggerSyntax Value(double value)
    {
        _value = value;
        return this;
    }

    public StaggerSyntax Value(double from, double to)
    {
        _value = new[] { from, to };
        return this;
    }

    public StaggerSyntax Value(string value)
    {
        _value = value;
        return this;
    }

    public StaggerSyntax Value(string from, string to)
    {
        _value = new[] { from, to };
        return this;
    }

    public StaggerSyntax Start(double value)
    {
        _hasOptions = true;
        _options.Start(value);
        return this;
    }

    public StaggerSyntax Reversed(bool reversed)
    {
        _hasOptions = true;
        _options.Reversed(reversed);
        return this;
    }

    public StaggerSyntax Ease(Easing value)
    {
        _hasOptions = true;
        _options.Ease(value);
        return this;
    }

    public StaggerSyntax Grid(int columns, int rows)
    {
        _hasOptions = true;
        _options.Grid(columns, rows);
        return this;
    }

    public StaggerSyntax Grid(int[] columnsAndRows)
    {
        _hasOptions = true;
        _options.Grid(columnsAndRows);
        return this;
    }

    public StaggerSyntax Axis(StaggerAxis value)
    {
        _hasOptions = true;
        _options.Axis(value);
        return this;
    }

    public StaggerSyntax From(StaggerPosition value)
    {
        _hasOptions = true;
        _options.From(value);
        return this;
    }

    public StaggerSyntax From(int index) => From(StaggerPosition.Index(index));

    internal Stagger ToStagger() =>
        Stagger.FromParts(_value, _hasOptions ? _options.Build() : []);
}

public sealed class StaggerAxis
{
    public static StaggerAxis X { get; } = new("x");
    public static StaggerAxis Y { get; } = new("y");
    internal string GetValue() => _name;
    private StaggerAxis(string name) { _name = name; }
    private readonly string _name;
}

public sealed class StaggerPosition
{
    public static StaggerPosition First { get; } = new("first");
    public static StaggerPosition Last { get; } = new("last");
    public static StaggerPosition Center { get; } = new("center");
    public static StaggerPosition Index(int index) => new("index", index);
    internal object GetValue() => (object?)_index ?? _name;

    private StaggerPosition(string name, int? index = null)
    {
        _name = name;
        _index = index;
    }
    private readonly string _name;
    private readonly int? _index;
}

internal static class StaggerExtensions
{
    internal static Dictionary<string, object> ToStaggerOptions(this IEnumerable<Prop> props)
    {
        var options = new Dictionary<string, object>();
        foreach (var prop in props)
            options[prop.GetName()] = prop.GetValue();
        return options;
    }
}