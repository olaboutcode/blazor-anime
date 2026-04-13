namespace BlazorAnime;

public sealed class Stagger
{
    public static Stagger Create(double delay) => new(delay, []);
    public static Stagger Create(double from, double to) => new(new[] { from, to }, []);
    public static Stagger Create(double value, Action<StaggerOptionsBuilder> configure)
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

    public IReadOnlyList<StgOptionProp> GetOptions() => _options;
    public object GetValue() => _value;

    private Stagger(object value, IReadOnlyList<StgOptionProp> options)
    {
        _options = options;
        _value = value;
    }

    private readonly IReadOnlyList<StgOptionProp> _options;
    private readonly object _value;
}

public sealed class StaggerOptionsBuilder
{
    public StaggerOptionsBuilder Start(double value)
    {
        _options.Add(new StgOptionProp("start", value));
        return this;
    }

    public StaggerOptionsBuilder Direction(Direction value)
    {
        _options.Add(new StgOptionProp("direction", value.GetValue()));
        return this;
    }

    public StaggerOptionsBuilder Easing(Easing value)
    {
        _options.Add(new StgOptionProp("easing", value.GetValue()));
        return this;
    }

    public StaggerOptionsBuilder Grid(int rows, int columns)
    {
        _options.Add(new StgOptionProp("grid", new[] { rows, columns }));
        return this;
    }

    public StaggerOptionsBuilder Axis(StaggerAxis value)
    {
        _options.Add(new StgOptionProp("axis", value));
        return this;
    }

    public StaggerOptionsBuilder From(StaggerPosition value)
    {
        _options.Add(new StgOptionProp("from", value.GetValue()));
        return this;
    }

    internal IReadOnlyList<StgOptionProp> Build() => _options.AsReadOnly();
    private readonly List<StgOptionProp> _options;
    internal StaggerOptionsBuilder() => _options = [];
}

public sealed class StaggerAxis
{
    public static StaggerAxis X { get; } = new("x");
    public static StaggerAxis Y { get; } = new("y");
    public string GetValue() => _name;
    private StaggerAxis(string name) { _name = name; }
    private readonly string _name;
}

public sealed class StaggerPosition
{
    public static StaggerPosition First { get; } = new("first");
    public static StaggerPosition Last { get; } = new("last");
    public static StaggerPosition Center { get; } = new("center");
    public static StaggerPosition Index(int index) => new("index", index);
    public object GetValue() => (object?)_index ?? _name;

    private StaggerPosition(string name, int? index = null)
    {
        _name = name;
        _index = index;
    }
    private readonly string _name;
    private readonly int? _index;
}