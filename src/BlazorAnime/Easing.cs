namespace BlazorAnime;

public sealed class Easing
{
    /// <summary>
    /// Samples <paramref name="ease"/> from 0 to 1. JavaScript interpolates those samples on every frame.
    /// </summary>
    public static Easing Curve(Func<double, double> ease, int samples = 64)
    {
        ArgumentNullException.ThrowIfNull(ease);
        if (samples < 2)
            throw new ArgumentOutOfRangeException(nameof(samples), "A curve needs at least two samples.");

        var table = new double[samples];
        var last = samples - 1;
        for (var i = 0; i < samples; i++)
            table[i] = ease(i / (double)last);
        return new Easing(table);
    }

    /// <summary>Passes an easing name the v4 parser accepts, for example "inElastic(1, .5)".</summary>
    public static Easing Raw(string definition) => new(definition);

    public static Easing Spring() => Function("spring");
    public static Easing Spring(double mass, double stiffness, double damping, double velocity) =>
        Function("spring", mass, stiffness, damping, velocity);
    public static Easing SpringBounce(double bounce, int durationMilliseconds) =>
        Function("spring", bounce: bounce, duration: durationMilliseconds);
    public static Easing Steps(int steps) => Function("steps", steps);
    public static Easing CubicBezier(double x1, double y1, double x2, double y2) =>
        Function("cubicBezier", x1, y1, x2, y2);
    public static Easing In(double power) => new($"in({power})");
    public static Easing Out(double power) => new($"out({power})");
    public static Easing InOut(double power) => new($"inOut({power})");
    public static Easing InElastic(double amplitude = 1, double period = 0.3)
        => new($"inElastic({amplitude}, {period})");
    public static Easing OutElastic(double amplitude = 1, double period = 0.3)
        => new($"outElastic({amplitude}, {period})");
    public static Easing InOutElastic(double amplitude = 1, double period = 0.3)
        => new($"inOutElastic({amplitude}, {period})");
    public static Easing OutInElastic(double amplitude = 1, double period = 0.3)
        => new($"outInElastic({amplitude}, {period})");

    public static Easing Linear { get; } = new("linear");
    public static Easing InQuad { get; } = new("inQuad");
    public static Easing InCubic { get; } = new("inCubic");
    public static Easing InQuart { get; } = new("inQuart");
    public static Easing InQuint { get; } = new("inQuint");
    public static Easing InSine { get; } = new("inSine");
    public static Easing InExpo { get; } = new("inExpo");
    public static Easing InCirc { get; } = new("inCirc");
    public static Easing InBack { get; } = new("inBack");
    public static Easing InBounce { get; } = new("inBounce");

    public static Easing OutQuad { get; } = new("outQuad");
    public static Easing OutCubic { get; } = new("outCubic");
    public static Easing OutQuart { get; } = new("outQuart");
    public static Easing OutQuint { get; } = new("outQuint");
    public static Easing OutSine { get; } = new("outSine");
    public static Easing OutExpo { get; } = new("outExpo");
    public static Easing OutCirc { get; } = new("outCirc");
    public static Easing OutBack { get; } = new("outBack");
    public static Easing OutBounce { get; } = new("outBounce");

    public static Easing InOutQuad { get; } = new("inOutQuad");
    public static Easing InOutCubic { get; } = new("inOutCubic");
    public static Easing InOutQuart { get; } = new("inOutQuart");
    public static Easing InOutQuint { get; } = new("inOutQuint");
    public static Easing InOutSine { get; } = new("inOutSine");
    public static Easing InOutExpo { get; } = new("inOutExpo");
    public static Easing InOutCirc { get; } = new("inOutCirc");
    public static Easing InOutBack { get; } = new("inOutBack");
    public static Easing InOutBounce { get; } = new("inOutBounce");

    public static Easing OutInQuad { get; } = new("outInQuad");
    public static Easing OutInCubic { get; } = new("outInCubic");
    public static Easing OutInQuart { get; } = new("outInQuart");
    public static Easing OutInQuint { get; } = new("outInQuint");
    public static Easing OutInSine { get; } = new("outInSine");
    public static Easing OutInExpo { get; } = new("outInExpo");
    public static Easing OutInCirc { get; } = new("outInCirc");
    public static Easing OutInBack { get; } = new("outInBack");
    public static Easing OutInBounce { get; } = new("outInBounce");

    public string GetValue() => _name ?? throw new InvalidOperationException(
        "A sampled curve has no anime.js expression. Pass it to Ease().");

    internal bool IsCurve => _samples is not null;
    internal bool IsFunction => _function is not null;
    internal double[] Samples => _samples ?? throw new InvalidOperationException(
        "This easing is an anime.js expression.");
    internal EaseFunction FunctionSpec => _function ?? throw new InvalidOperationException(
        "This easing is not a function marker.");

    private Easing(string name) => _name = name;
    private Easing(double[] samples) => _samples = samples;
    private Easing(EaseFunction function) => _function = function;
    private readonly string? _name;
    private readonly double[]? _samples;
    private readonly EaseFunction? _function;

    private static Easing Function(
        string fn,
        double? a = null,
        double? b = null,
        double? c = null,
        double? d = null,
        double? bounce = null,
        int? duration = null) =>
        new(new EaseFunction(fn, a, b, c, d, bounce, duration));
}

internal sealed record EaseFunction(
    string Fn,
    double? A,
    double? B,
    double? C,
    double? D,
    double? Bounce,
    int? Duration);
