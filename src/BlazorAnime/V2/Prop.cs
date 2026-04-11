namespace BlazorAnime.V2;

public class AnimeProp
{
    internal AnimeProp(string name, object value)
    {
        this.Name = name;
        this.Value = value;
    }

    internal string Name { get; init; }
    internal object Value { get; init; }
}

public static class Prop
{
    public static AnimeProp Targets(string value) => ("targets", value).GetValue();
    public static AnimeProp Targets(object value) => ("targets", value).GetValue();
    public static AnimeProp Targets(object[] value) => ("targets", value).GetValue();
    public static AnimeProp Targets(string[] value) => ("targets", value).GetValue();
    
    public static AnimeProp Duration(float value) => ("duration", value).GetValue();
    public static AnimeProp Duration(float from, float to) => ("duration", new { from, to }).GetValue();
    public static AnimeProp Duration(List<(string name, object value)> value) => ("duration", value).GetValue();

    public static AnimeProp Delay(float value) => ("delay", value).GetValue();
    public static AnimeProp Delay(Stagger stagger) => ("delay", stagger).GetValue();
    public static AnimeProp Delay(Func<int, int, double> callback) => ("delay", callback).GetValue();

    public static AnimeProp EndDelay(float value) => ("endDelay", value).GetValue();
    public static AnimeProp EndDelay(Stagger stagger) => ("endDelay", stagger).GetValue();
    public static AnimeProp EndDelay(Func<int, int, double> callback) => ("endDelay", callback).GetValue();

    public static AnimeProp Round(int value) => ("round", value).GetValue();
    public static AnimeProp Loopp(bool value) => ("loop", value).GetValue();
    public static AnimeProp Loop(bool value) => ("loop", value).GetValue();
    public static AnimeProp AutoPlay(bool value) => ("autoPlay", value).GetValue();

    public static AnimeProp Update(
        Action<AnimationState> callback) => ("update", callback).GetValue();
    public static AnimeProp Begin(
        Action<AnimationState> callback) => ("begin", callback).GetValue();
    public static AnimeProp LoopBegin(
        Action<AnimationState> callback) => ("loopBegin", callback).GetValue();
    public static AnimeProp LoopComplete(
        Action<AnimationState> callback) => ("loopComplete", callback).GetValue();
    public static AnimeProp Complete(
        Action<AnimationState> callback) => ("complete", callback).GetValue();
    public static AnimeProp Change(
        Action<AnimationState> callback) => ("change", callback).GetValue();
    public static AnimeProp ChangeBegin(
        Action<AnimationState> callback) => ("changeBegin", callback).GetValue();
    public static AnimeProp ChangeComplete(
        Action<AnimationState> callback) => ("changeComplete", callback).GetValue();

    public static AnimeProp Points(
        List<(string name, string value)> value) => ("points", value).GetValue();
    public static AnimeProp Points(List<List<AnimeProp>> value) =>
        ("points", value.Select(v => v.GetValue())).GetValue();

    public static AnimeProp Create(string name, float value) => ("create", value).GetValue();
    public static AnimeProp Create(string name, string value) => ("create", value).GetValue();
    public static AnimeProp Create(string name, object value) => ("create", value).GetValue();
    public static AnimeProp Create(string name, float[] value) => ("create", value).GetValue();
    public static AnimeProp Create(string name, string[] value) => ("create", value).GetValue();
    public static AnimeProp Create(string name, object[] value) => ("create", value).GetValue();
    public static AnimeProp Create(string name, float from, float to) => Create<float[]>(name, [from, to]);
    public static AnimeProp Create(string name, string from, string to) => Create<string[]>(name, [from, to]);
    public static AnimeProp Create(string name, Stagger value) => Create<Stagger>(name, value);
    public static AnimeProp Create(string name, PathParam value) => Create<PathParam>(name, value);
    public static AnimeProp Create(string name, List<List<AnimeProp>> value) =>
        ("create", value.Select(v => v.GetValue())).GetValue();

    private static AnimeProp Create<T>(string name, T value) => (name, value).GetValue();
}

public static class Value
{
    public static AnimeProp Add(float value) => ("value", $"+={value}").GetValue();
    public static AnimeProp Subtract(float value) => ("value", $"-={value}").GetValue();
    public static AnimeProp Multiply(float value) => ("value", $"*={value}").GetValue();
}

public static class Direction
{
    public static AnimeProp Normal { get; } = ("direction", "normal").GetValue();
    public static AnimeProp Reverse { get; } = ("direction", "reverse").GetValue();
    public static AnimeProp Alternate { get; } = ("direction", "alternate").GetValue();
}

public static class Easing
{
    public static AnimeProp Steps(int steps) => ("Easing", $"steps({steps})").GetValue();
    public static AnimeProp CubicBezier(double x1, double y1, double x2, double y2) =>
        ("Easing", $"cubicBezier({x1}, {y1}, {x2}, {y2})").GetValue();
    public static AnimeProp EaseInElastic(double amplitude, double period)
        => ("Easing", $"easeInElastic({amplitude}, {period})").GetValue();
    public static AnimeProp EaseOutElastic(double amplitude, double period)
        => ("Easing", $"easeOutElastic({amplitude}, {period})").GetValue();
    public static AnimeProp EaseInOutElastic(double amplitude, double period)
        => ("Easing", $"easeInOutElastic({amplitude}, {period})").GetValue();
    public static AnimeProp EaseOutInElastic(double amplitude, double period)
        => ("Easing", $"easeOutInElastic({amplitude}, {period})").GetValue();
    public static AnimeProp Spring(double mass, double stiffness, double damping, double velocity)
        => ("Easing", $"spring({mass}, {stiffness}, {damping}, {velocity})").GetValue();

    public static AnimeProp Spring() => ("Easing", "spring").GetValue();
    public static AnimeProp Linear { get; } = ("Easing", "linear").GetValue();
    public static AnimeProp EaseInQuad { get; } = ("Easing", "easeInQuad").GetValue();
    public static AnimeProp EaseInCubic { get; } = ("Easing", "easeInCubic").GetValue();
    public static AnimeProp EaseInQuart { get; } = ("Easing", "easeInQuart").GetValue();
    public static AnimeProp EaseInQuint { get; } = ("Easing", "easeInQuint").GetValue();
    public static AnimeProp EaseInSine { get; } = ("Easing", "easeInSine").GetValue();
    public static AnimeProp EaseInExpo { get; } = ("Easing", "easeInExpo").GetValue();
    public static AnimeProp EaseInCirc { get; } = ("Easing", "easeInCirc").GetValue();
    public static AnimeProp EaseInBack { get; } = ("Easing", "easeInBack").GetValue();
    public static AnimeProp EaseInBounce { get; } = ("Easing", "easeInBounce").GetValue();

    public static AnimeProp EaseOutQuad { get; } = ("Easing", "easeOutQuad").GetValue();
    public static AnimeProp EaseOutCubic { get; } = ("Easing", "easeOutCubic").GetValue();
    public static AnimeProp EaseOutQuart { get; } = ("Easing", "easeOutQuart").GetValue();
    public static AnimeProp EaseOutQuint { get; } = ("Easing", "easeOutQuint").GetValue();
    public static AnimeProp EaseOutSine { get; } = ("Easing", "easeOutSine").GetValue();
    public static AnimeProp EaseOutExpo { get; } = ("Easing", "easeOutExpo").GetValue();
    public static AnimeProp EaseOutCirc { get; } = ("Easing", "easeOutCirc").GetValue();
    public static AnimeProp EaseOutBack { get; } = ("Easing", "easeOutBack").GetValue();
    public static AnimeProp EaseOutBounce { get; } = ("Easing", "easeOutBounce").GetValue();

    public static AnimeProp EaseInOutQuad { get; } = ("Easing", "easeInOutQuad").GetValue();
    public static AnimeProp EaseInOutCubic { get; } = ("Easing", "easeInOutCubic").GetValue();
    public static AnimeProp EaseInOutQuart { get; } = ("Easing", "easeInOutQuart").GetValue();
    public static AnimeProp EaseInOutQuint { get; } = ("Easing", "easeInOutQuint").GetValue();
    public static AnimeProp EaseInOutSine { get; } = ("Easing", "easeInOutSine").GetValue();
    public static AnimeProp EaseInOutExpo { get; } = ("Easing", "easeInOutExpo").GetValue();
    public static AnimeProp EaseInOutCirc { get; } = ("Easing", "easeInOutCirc").GetValue();
    public static AnimeProp EaseInOutBack { get; } = ("Easing", "easeInOutBack").GetValue();
    public static AnimeProp EaseInOutBounce { get; } = ("Easing", "easeInOutBounce").GetValue();

    public static AnimeProp EaseOutInQuad { get; } = ("Easing", "easeOutInQuad").GetValue();
    public static AnimeProp EaseOutInCubic { get; } = ("Easing", "easeOutInCubic").GetValue();
    public static AnimeProp EaseOutInQuart { get; } = ("Easing", "easeOutInQuart").GetValue();
    public static AnimeProp EaseOutInQuint { get; } = ("Easing", "easeOutInQuint").GetValue();
    public static AnimeProp EaseOutInSine { get; } = ("Easing", "easeOutInSine").GetValue();
    public static AnimeProp EaseOutInExpo { get; } = ("Easing", "easeOutInExpo").GetValue();
    public static AnimeProp EaseOutInCirc { get; } = ("Easing", "easeOutInCirc").GetValue();
    public static AnimeProp EaseOutInBack { get; } = ("Easing", "easeOutInBack").GetValue();
    public static AnimeProp EaseOutInBounce { get; } = ("Easing", "easeOutInBounce").GetValue();
}