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
    
    // anime props
    public static PropsBuilder Targets(
        this PropsBuilder builder,
        params string[] targets) => builder.Prop("targets", targets);
    public static PropsBuilder Points(
        this PropsBuilder builder,
        Action<PropsBuilder> configure) => builder.Prop("points", configure);
    public static PropsBuilder Points(
        this PropsBuilder builder,
        params Action<PropsBuilder>[] build) => builder.Prop("points", build);
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
        params Action<PropsBuilder>[] build) => builder.Prop("keyframes", build);
    
    public static PropsBuilder Perspective(
        this PropsBuilder builder,
        int value) => builder.Prop("perspective", value);
    // Transform properties
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        double value) => builder.Prop("translateX", value);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("translateX", value);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("translateX", callback);
    
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        double value) => builder.Prop("translateY", value);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("translateY", value);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("translateY", callback);
    
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        double value) => builder.Prop("translateZ", value);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("translateZ", value);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("translateZ", callback);
    
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        double value) => builder.Prop("rotate", value);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("rotate", value);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rotate", callback);
    
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        double value) => builder.Prop("rotateX", value);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("rotateX", value);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rotateX", callback);
    
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        double value) => builder.Prop("rotateY", value);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("rotateY", value);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rotateY", callback);
    
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        double value) => builder.Prop("rotateZ", value);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("rotateZ", value);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rotateZ", callback);

    public static PropsBuilder Scale(
        this PropsBuilder builder,
        double value) => builder.Prop("scale", value);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("scale", value);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("scale", callback);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        params Func<PropsBuilder, PropsBuilder>[] build) => builder.Prop("scale", build);
    
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        double value) => builder.Prop("scaleX", value);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("scaleX", value);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("scaleX", callback);
    
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        double value) => builder.Prop("scaleY", value);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("scaleY", value);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("scaleY", callback);
    
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        double value) => builder.Prop("scaleZ", value);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("scaleZ", value);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("scaleZ", callback);
    
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        double value) => builder.Prop("skewX", value);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("skewX", value);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("skewX", callback);
    
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        double value) => builder.Prop("skewY", value);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("skewY", value);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("skewY", callback);

    // Color properties
    public static PropsBuilder BackgroundColor(
        this PropsBuilder builder,
        string value) => builder.Prop("backgroundColor", value);
    public static PropsBuilder BackgroundColor(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("backgroundColor", callback);
    
    public static PropsBuilder Color(
        this PropsBuilder builder,
        string value) => builder.Prop("color", value);
    public static PropsBuilder Color(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("color", callback);
    
    public static PropsBuilder BorderColor(
        this PropsBuilder builder,
        string value) => builder.Prop("borderColor", value);
    public static PropsBuilder BorderColor(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("borderColor", callback);
    
    // Visual properties
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        double value) => builder.Prop("opacity", value);
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("opacity", value);
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("opacity", callback);
    
    // Border properties
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        string value) => builder.Prop("borderRadius", value);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        double value) => builder.Prop("borderRadius", value);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("borderRadius", callback);
    
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        double value) => builder.Prop("borderWidth", value);
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        string value) => builder.Prop("borderWidth", value);
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("borderWidth", callback);
    
    // Dimensions
    public static PropsBuilder Width(
        this PropsBuilder builder,
        double value) => builder.Prop("width", value);
    public static PropsBuilder Width(
        this PropsBuilder builder,
        string value) => builder.Prop("width", value);
    public static PropsBuilder Width(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("width", callback);
    
    public static PropsBuilder Height(
        this PropsBuilder builder,
        double value) => builder.Prop("height", value);
    public static PropsBuilder Height(
        this PropsBuilder builder,
        string value) => builder.Prop("height", value);
    public static PropsBuilder Height(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("height", callback);
    
    // Position properties
    public static PropsBuilder Top(
        this PropsBuilder builder,
        double value) => builder.Prop("top", value);
    public static PropsBuilder Top(
        this PropsBuilder builder,
        string value) => builder.Prop("top", value);
    public static PropsBuilder Top(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("top", callback);
    
    public static PropsBuilder Left(
        this PropsBuilder builder,
        double value) => builder.Prop("left", value);
    public static PropsBuilder Left(
        this PropsBuilder builder,
        string value) => builder.Prop("left", value);
    public static PropsBuilder Left(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("left", callback);
    
    public static PropsBuilder Right(
        this PropsBuilder builder,
        double value) => builder.Prop("right", value);
    public static PropsBuilder Right(
        this PropsBuilder builder,
        string value) => builder.Prop("right", value);
    public static PropsBuilder Right(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("right", callback);
    
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        double value) => builder.Prop("bottom", value);
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        string value) => builder.Prop("bottom", value);
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("bottom", callback);
    
    // Margin properties
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        double value) => builder.Prop("marginTop", value);
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        string value) => builder.Prop("marginTop", value);
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("marginTop", callback);
    
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        double value) => builder.Prop("marginLeft", value);
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        string value) => builder.Prop("marginLeft", value);
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("marginLeft", callback);
    
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        double value) => builder.Prop("marginRight", value);
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        string value) => builder.Prop("marginRight", value);
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("marginRight", callback);
    
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        double value) => builder.Prop("marginBottom", value);
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        string value) => builder.Prop("marginBottom", value);
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("marginBottom", callback);
    
    // Padding properties
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        double value) => builder.Prop("paddingTop", value);
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        string value) => builder.Prop("paddingTop", value);
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("paddingTop", callback);
    
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        double value) => builder.Prop("paddingLeft", value);
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        string value) => builder.Prop("paddingLeft", value);
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("paddingLeft", callback);
    
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        double value) => builder.Prop("paddingRight", value);
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        string value) => builder.Prop("paddingRight", value);
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("paddingRight", callback);
    
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        double value) => builder.Prop("paddingBottom", value);
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        string value) => builder.Prop("paddingBottom", value);
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("paddingBottom", callback);
    
    // Typography properties
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        double value) => builder.Prop("fontSize", value);
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        string value) => builder.Prop("fontSize", value);
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("fontSize", callback);
    
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        double value) => builder.Prop("letterSpacing", value);
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        string value) => builder.Prop("letterSpacing", value);
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("letterSpacing", callback);
    
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        double value) => builder.Prop("lineHeight", value);
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        string value) => builder.Prop("lineHeight", value);
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("lineHeight", callback);
}