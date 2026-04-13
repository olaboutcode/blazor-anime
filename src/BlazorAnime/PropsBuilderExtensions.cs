namespace BlazorAnime;

public static class PropsBuilderExtensions
{
    // callbacks
    public static PropsBuilder Update(
        this PropsBuilder builder,
        Action<AnimationState> callback) => builder.Prop("update", callback);
    public static PropsBuilder Complete(
        this PropsBuilder builder,
        Action<AnimationState> callback) => builder.Prop("complete", callback);
    public static PropsBuilder Begin(
        this PropsBuilder builder,
        Action<AnimationState> callback) => builder.Prop("begin", callback);
    public static PropsBuilder LoopBegin(
        this PropsBuilder builder,
        Action<AnimationState> callback) => builder.Prop("loopBegin", callback);
    public static PropsBuilder LoopComplete(
        this PropsBuilder builder,
        Action<AnimationState> callback) => builder.Prop("loopComplete", callback);
    public static PropsBuilder Change(
        this PropsBuilder builder,
        Action<AnimationState> callback) => builder.Prop("change", callback);
    public static PropsBuilder ChangeBegin(
        this PropsBuilder builder,
        Action<AnimationState> callback) => builder.Prop("changeBegin", callback);
    public static PropsBuilder ChangeComplete(
        this PropsBuilder builder,
        Action<AnimationState> callback) => builder.Prop("changeComplete", callback);
    
    // props
    public static PropsBuilder Targets(
        this PropsBuilder builder,
        params string[] targets) => builder.Prop("targets", targets);
    public static PropsBuilder EndDelay(
        this PropsBuilder builder,
        int milliseconds) => builder.Prop("endDelay", milliseconds);
    public static PropsBuilder EndDelay(
        this PropsBuilder builder,
        Stagger stagger) => builder.Prop("endDelay", stagger);
    public static PropsBuilder EndDelay(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("endDelay", callback);
    public static PropsBuilder Duration(
        this PropsBuilder builder,
        int milliseconds) => builder.Prop("duration", milliseconds);
    public static PropsBuilder AutoPlay(
        this PropsBuilder builder,
        bool autoPlay) => builder.Prop("autoplay", autoPlay);
    public static PropsBuilder Delay(
        this PropsBuilder builder,
        int milliseconds) => builder.Prop("delay", milliseconds);
    public static PropsBuilder Delay(
        this PropsBuilder builder,
        Stagger stagger) => builder.Prop("delay", stagger);
    public static PropsBuilder Delay(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("delay", callback);
    public static PropsBuilder Round(
        this PropsBuilder builder,
        int value) => builder.Prop("round", value);
    public static PropsBuilder Loop(
        this PropsBuilder builder,
        bool loop) => builder.Prop("loop", loop);
    public static PropsBuilder Loop(
        this PropsBuilder builder,
        int count) => builder.Prop("loop", count);
    public static PropsBuilder StrokeDashoffset(
        this PropsBuilder builder,
        int value) => builder.Prop("strokeDashoffset", value);
    public static PropsBuilder Keyframes(
        this PropsBuilder builder,
        params Action<PropsBuilder>[] builders) => builder.Prop("keyframes", builders);
    public static PropsBuilder Perspective(
        this PropsBuilder builder,
        int value) => builder.Prop("perspective", value);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        double value) => builder.Prop("translateX", value);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        double value) => builder.Prop("translateY", value);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        double value) => builder.Prop("rotate", value);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        double value) => builder.Prop("scale", value);
    public static PropsBuilder BackgroundColor(
        this PropsBuilder builder,
        string value) => builder.Prop("backgroundColor", value);
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        double value) => builder.Prop("opacity", value);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        string value) => builder.Prop("borderRadius", value);
}