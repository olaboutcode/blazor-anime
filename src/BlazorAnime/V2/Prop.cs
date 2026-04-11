namespace BlazorAnime.V2;

public static class Prop
{
    public static (string name, string value) Targets(string value) => ("targets", value);
    public static (string name, object value) Targets(object value) => ("targets", value);
    public static (string name, object[] value) Targets(object[] value) => ("targets", value);
    public static (string name, string[] value) Targets(string[] value) => ("targets", value);
    
    public static (string name, float value) Duration(float value) => ("duration", value);
    public static (string name, object value) Duration(float from, float to) => ("duration", new { from, to });
    public static (string name, List<(string name, object value)> value) Duration(
        List<(string name, object value)> value) => ("duration", value);

    public static (string name, float value) Delay(float value) => ("delay", value);
    public static (string name, Stagger stagger) Delay(Stagger stagger) => ("delay", stagger);
    public static (string name, Func<int, int, double> callback) Delay(
        Func<int, int, double> callback) => ("delay", callback);

    public static (string name, float value) EndDelay(float value) => ("endDelay", value);
    public static (string name, Stagger stagger) EndDelay(Stagger stagger) => ("endDelay", stagger);
    public static (string name, Func<int, int, double> callback) EndDelay(
        Func<int, int, double> callback) => ("endDelay", callback);

    public static (string name, int value) Round(int value) => ("round", value);
    public static (string name, bool value) Loopp(bool value) => ("loop", value);
    public static (string name, bool value) Loop(bool value) => ("loop", value);
    public static (string name, bool value) AutoPlay(bool value) => ("autoPlay", value);

    public static (string name, Action<AnimationState> callback) Update(
        Action<AnimationState> callback) => ("update", callback);
    public static (string name, Action<AnimationState> callback) Begin(
        Action<AnimationState> callback) => ("begin", callback);
    public static (string name, Action<AnimationState> callback) LoopBegin(
        Action<AnimationState> callback) => ("loopBegin", callback);
    public static (string name, Action<AnimationState> callback) LoopComplete(
        Action<AnimationState> callback) => ("loopComplete", callback);
    public static (string name, Action<AnimationState> callback) Complete(
        Action<AnimationState> callback) => ("complete", callback);
    public static (string name, Action<AnimationState> callback) Change(
        Action<AnimationState> callback) => ("change", callback);
    public static (string name, Action<AnimationState> callback) ChangeBegin(
        Action<AnimationState> callback) => ("changeBegin", callback);
    public static (string name, Action<AnimationState> callback) ChangeComplete(
        Action<AnimationState> callback) => ("changeComplete", callback);

    public static (string name, List<(string name, string value)> value) Points(
        List<(string name, string value)> value) => ("points", value);
    public static (string name, object[] value) Points(
        List<List<(string name, string value)>> value) =>
            ("points", value.Select(v => v.GetValue()).ToArray());

    public static (string name, float value) Create(string name, float value) => Create<float>(name, value);
    public static (string name, string value) Create(string name, string value) => Create<string>(name, value);
    public static (string name, object value) Create(string name, object value) => Create<object>(name, value);
    public static (string name, float[] value) Create(string name, float[] value) => Create<float[]>(name, value);
    public static (string name, string[] value) Create(string name, string[] value) => Create<string[]>(name, value);
    public static (string name, object[] value) Create(string name, object[] value) => Create<object[]>(name, value);
    public static (string name, float[] value) Create(string name, float from, float to) => Create<float[]>(name, [from, to]);
    public static (string name, string[] value) Create(string name, string from, string to) => Create<string[]>(name, [from, to]);
    public static (string name, Stagger value) Create(string name, Stagger value) => Create<Stagger>(name, value);
    public static (string name, PathParam value) Create(string name, PathParam value) => Create<PathParam>(name, value);
    public static (string name, object[] value) Create(string name, List<List<(string name, string value)>> value) =>
        (name, value.Select(v => v.GetValue()).ToArray());

    private static (string name, T value) Create<T>(string name, T value) => (name, value);
}

public static class Value
{
    public static (string name, string value) Add(float value) => ("value", $"+={value}");
    public static (string name, string value) Subtract(float value) => ("value", $"-={value}");
    public static (string name, string value) Multiply(float value) => ("value", $"*={value}");
}

public static class Direction
{
    public static (string name, string value) Normal { get; } = ("direction", "normal");
    public static (string name, string value) Reverse { get; } = ("direction", "reverse");
    public static (string name, string value) Alternate { get; } = ("direction", "alternate");
}

public static class Easing
{
    public static (string name, string value) Steps(int steps) => ("Easing", $"steps({steps})");
    public static (string name, string value) CubicBezier(double x1, double y1, double x2, double y2) =>
        ("Easing", $"cubicBezier({x1}, {y1}, {x2}, {y2})");
    public static (string name, string value) EaseInElastic(double amplitude, double period)
        => ("Easing", $"easeInElastic({amplitude}, {period})");
    public static (string name, string value) EaseOutElastic(double amplitude, double period)
        => ("Easing", $"easeOutElastic({amplitude}, {period})");
    public static (string name, string value) EaseInOutElastic(double amplitude, double period)
        => ("Easing", $"easeInOutElastic({amplitude}, {period})");
    public static (string name, string value) EaseOutInElastic(double amplitude, double period)
        => ("Easing", $"easeOutInElastic({amplitude}, {period})");
    public static (string name, string value) Spring(double mass, double stiffness, double damping, double velocity)
        => ("Easing", $"spring({mass}, {stiffness}, {damping}, {velocity})");

    public static (string name, string value) Spring() => ("Easing", "spring");
    public static (string name, string value) Linear { get; } = ("Easing", "linear");
    public static (string name, string value) EaseInQuad { get; } = ("Easing", "easeInQuad");
    public static (string name, string value) EaseInCubic { get; } = ("Easing", "easeInCubic");
    public static (string name, string value) EaseInQuart { get; } = ("Easing", "easeInQuart");
    public static (string name, string value) EaseInQuint { get; } = ("Easing", "easeInQuint");
    public static (string name, string value) EaseInSine { get; } = ("Easing", "easeInSine");
    public static (string name, string value) EaseInExpo { get; } = ("Easing", "easeInExpo");
    public static (string name, string value) EaseInCirc { get; } = ("Easing", "easeInCirc");
    public static (string name, string value) EaseInBack { get; } = ("Easing", "easeInBack");
    public static (string name, string value) EaseInBounce { get; } = ("Easing", "easeInBounce");

    public static (string name, string value) EaseOutQuad { get; } = ("Easing", "easeOutQuad");
    public static (string name, string value) EaseOutCubic { get; } = ("Easing", "easeOutCubic");
    public static (string name, string value) EaseOutQuart { get; } = ("Easing", "easeOutQuart");
    public static (string name, string value) EaseOutQuint { get; } = ("Easing", "easeOutQuint");
    public static (string name, string value) EaseOutSine { get; } = ("Easing", "easeOutSine");
    public static (string name, string value) EaseOutExpo { get; } = ("Easing", "easeOutExpo");
    public static (string name, string value) EaseOutCirc { get; } = ("Easing", "easeOutCirc");
    public static (string name, string value) EaseOutBack { get; } = ("Easing", "easeOutBack");
    public static (string name, string value) EaseOutBounce { get; } = ("Easing", "easeOutBounce");

    public static (string name, string value) EaseInOutQuad { get; } = ("Easing", "easeInOutQuad");
    public static (string name, string value) EaseInOutCubic { get; } = ("Easing", "easeInOutCubic");
    public static (string name, string value) EaseInOutQuart { get; } = ("Easing", "easeInOutQuart");
    public static (string name, string value) EaseInOutQuint { get; } = ("Easing", "easeInOutQuint");
    public static (string name, string value) EaseInOutSine { get; } = ("Easing", "easeInOutSine");
    public static (string name, string value) EaseInOutExpo { get; } = ("Easing", "easeInOutExpo");
    public static (string name, string value) EaseInOutCirc { get; } = ("Easing", "easeInOutCirc");
    public static (string name, string value) EaseInOutBack { get; } = ("Easing", "easeInOutBack");
    public static (string name, string value) EaseInOutBounce { get; } = ("Easing", "easeInOutBounce");

    public static (string name, string value) EaseOutInQuad { get; } = ("Easing", "easeOutInQuad");
    public static (string name, string value) EaseOutInCubic { get; } = ("Easing", "easeOutInCubic");
    public static (string name, string value) EaseOutInQuart { get; } = ("Easing", "easeOutInQuart");
    public static (string name, string value) EaseOutInQuint { get; } = ("Easing", "easeOutInQuint");
    public static (string name, string value) EaseOutInSine { get; } = ("Easing", "easeOutInSine");
    public static (string name, string value) EaseOutInExpo { get; } = ("Easing", "easeOutInExpo");
    public static (string name, string value) EaseOutInCirc { get; } = ("Easing", "easeOutInCirc");
    public static (string name, string value) EaseOutInBack { get; } = ("Easing", "easeOutInBack");
    public static (string name, string value) EaseOutInBounce { get; } = ("Easing", "easeOutInBounce");
}