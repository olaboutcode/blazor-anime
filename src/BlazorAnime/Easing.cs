namespace BlazorAnime;

public sealed class Easing
{
    /// <summary>Passes an anime.js easing expression through unchanged, for example "easeInElastic(1, .5)".</summary>
    public static Easing Raw(string definition) => new(definition);

    public static Easing Spring() => new("spring");
    public static Easing Steps(int steps) => new($"steps({steps})");
    public static Easing EaseInElastic(double amplitude = 1, double period = 0.5)
        => new($"easeInElastic({amplitude}, {period})");
    public static Easing EaseOutElastic(double amplitude = 1, double period = 0.5)
        => new($"easeOutElastic({amplitude}, {period})");
    public static Easing EaseInOutElastic(double amplitude = 1, double period = 0.5)
        => new($"easeInOutElastic({amplitude}, {period})");
    public static Easing EaseOutInElastic(double amplitude = 1, double period = 0.5)
        => new($"easeOutInElastic({amplitude}, {period})");
    public static Easing CubicBezier(double x1, double y1, double x2, double y2) =>
        new($"cubicBezier({x1}, {y1}, {x2}, {y2})");
    public static Easing Spring(double mass, double stiffness, double damping, double velocity)
        => new($"spring({mass}, {stiffness}, {damping}, {velocity})");

    public static Easing Linear { get; } = new("linear");
    public static Easing EaseInQuad { get; } = new("easeInQuad");
    public static Easing EaseInCubic { get; } = new("easeInCubic");
    public static Easing EaseInQuart { get; } = new("easeInQuart");
    public static Easing EaseInQuint { get; } = new("easeInQuint");
    public static Easing EaseInSine { get; } = new("easeInSine");
    public static Easing EaseInExpo { get; } = new("easeInExpo");
    public static Easing EaseInCirc { get; } = new("easeInCirc");
    public static Easing EaseInBack { get; } = new("easeInBack");
    public static Easing EaseInBounce { get; } = new("easeInBounce");

    public static Easing EaseOutQuad { get; } = new("easeOutQuad");
    public static Easing EaseOutCubic { get; } = new("easeOutCubic");
    public static Easing EaseOutQuart { get; } = new("easeOutQuart");
    public static Easing EaseOutQuint { get; } = new("easeOutQuint");
    public static Easing EaseOutSine { get; } = new("easeOutSine");
    public static Easing EaseOutExpo { get; } = new("easeOutExpo");
    public static Easing EaseOutCirc { get; } = new("easeOutCirc");
    public static Easing EaseOutBack { get; } = new("easeOutBack");
    public static Easing EaseOutBounce { get; } = new("easeOutBounce");

    public static Easing EaseInOutQuad { get; } = new("easeInOutQuad");
    public static Easing EaseInOutCubic { get; } = new("easeInOutCubic");
    public static Easing EaseInOutQuart { get; } = new("easeInOutQuart");
    public static Easing EaseInOutQuint { get; } = new("easeInOutQuint");
    public static Easing EaseInOutSine { get; } = new("easeInOutSine");
    public static Easing EaseInOutExpo { get; } = new("easeInOutExpo");
    public static Easing EaseInOutCirc { get; } = new("easeInOutCirc");
    public static Easing EaseInOutBack { get; } = new("easeInOutBack");
    public static Easing EaseInOutBounce { get; } = new("easeInOutBounce");

    public static Easing EaseOutInQuad { get; } = new("easeOutInQuad");
    public static Easing EaseOutInCubic { get; } = new("easeOutInCubic");
    public static Easing EaseOutInQuart { get; } = new("easeOutInQuart");
    public static Easing EaseOutInQuint { get; } = new("easeOutInQuint");
    public static Easing EaseOutInSine { get; } = new("easeOutInSine");
    public static Easing EaseOutInExpo { get; } = new("easeOutInExpo");
    public static Easing EaseOutInCirc { get; } = new("easeOutInCirc");
    public static Easing EaseOutInBack { get; } = new("easeOutInBack");
    public static Easing EaseOutInBounce { get; } = new("easeOutInBounce");

    public string GetValue() => _name;
    private Easing(string name) { _name = name; }
    private readonly string _name;
}
