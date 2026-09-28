using Microsoft.AspNetCore.Components;

namespace BlazorAnime;

public static class PropsBuilderExtensions
{
    // Animation Lifecycle Callbacks
    // Hooks for animation events: update, begin, complete, loop, and change events
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
    
    // Animation Configuration
    // Core animation settings: targets, timing, delays, loops, keyframes, and SVG properties
    public static PropsBuilder Targets(
        this PropsBuilder builder,
        params string[] selectors) => builder.Prop("targets", selectors);
    public static PropsBuilder Targets(
        this PropsBuilder builder,
        params ElementReference[] elements) => builder.Prop("targets", elements);
    public static PropsBuilder Targets(
        this PropsBuilder builder,
        params JsTarget[] targets)
    {
        var references = targets.Select(target => target.Reference).ToArray();
        return references.Length == 1
            ? builder.ObjectTarget("targets", references[0])
            : builder.ObjectTarget("targets", references);
    }
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
    public static PropsBuilder EndDelay(
        this PropsBuilder builder,
        Func<StaggerSyntax, StaggerSyntax> configure) =>
        builder.EndDelay(configure(new StaggerSyntax()).ToStagger());
    public static PropsBuilder Duration(
        this PropsBuilder builder,
        int milliseconds) => builder.Prop("duration", milliseconds);
    public static PropsBuilder Duration(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("duration", callback);
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
    public static PropsBuilder Delay(
        this PropsBuilder builder,
        Func<StaggerSyntax, StaggerSyntax> configure) =>
        builder.Delay(configure(new StaggerSyntax()).ToStagger());
    public static PropsBuilder Round(
        this PropsBuilder builder,
        int value) => builder.Prop("round", value);
    public static PropsBuilder Loop(
        this PropsBuilder builder,
        bool shouldLoop) => builder.Prop("loop", shouldLoop);
    public static PropsBuilder Loop(
        this PropsBuilder builder,
        int loopCount) => builder.Prop("loop", loopCount);
    public static PropsBuilder StrokeDashoffset(
        this PropsBuilder builder,
        int value) => builder.Prop("strokeDashoffset", value);
    public static PropsBuilder Keyframes(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("keyframes", build);
    
    // Transform Properties
    // CSS transform functions: translate, rotate, scale, skew, and perspective
    // Available with static values, stagger configurations, and function-based callbacks
    public static PropsBuilder Perspective(
        this PropsBuilder builder,
        int value) => builder.Prop("perspective", value);
    public static PropsBuilder Perspective(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("perspective", build);
    public static PropsBuilder Perspective(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("perspective", build);
    
    // Translation: Move elements along X, Y, and Z axes
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        double value) => builder.Prop("translateX", value);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("translateX", from, to);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("translateX", from, to);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("translateX", value);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("translateX", build);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("translateX", build);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("translateX", callback);
    public static PropsBuilder TranslateX(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("translateX", path);
    
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        double value) => builder.Prop("translateY", value);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("translateY", from, to);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("translateY", from, to);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("translateY", value);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("translateY", build);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("translateY", build);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("translateY", callback);
    public static PropsBuilder TranslateY(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("translateY", path);
    
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        double value) => builder.Prop("translateZ", value);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("translateZ", from, to);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("translateZ", from, to);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("translateZ", value);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("translateZ", build);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("translateZ", build);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("translateZ", callback);
    public static PropsBuilder TranslateZ(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("translateZ", path);
    
    // Rotation: Rotate elements on X, Y, and Z axes (in degrees)
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        double value) => builder.Prop("rotate", value);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("rotate", from, to);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("rotate", from, to);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("rotate", value);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("rotate", build);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("rotate", build);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rotate", callback);
    public static PropsBuilder Rotate(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("rotate", path);
    
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        double value) => builder.Prop("rotateX", value);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("rotateX", from, to);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("rotateX", from, to);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("rotateX", value);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("rotateX", build);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("rotateX", build);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rotateX", callback);
    public static PropsBuilder RotateX(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("rotateX", path);
    
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        double value) => builder.Prop("rotateY", value);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("rotateY", from, to);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("rotateY", from, to);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("rotateY", value);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("rotateY", build);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("rotateY", build);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rotateY", callback);
    public static PropsBuilder RotateY(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("rotateY", path);
    
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        double value) => builder.Prop("rotateZ", value);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("rotateZ", from, to);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("rotateZ", from, to);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("rotateZ", value);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("rotateZ", build);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("rotateZ", build);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rotateZ", callback);
    public static PropsBuilder RotateZ(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("rotateZ", path);

    // Scale: Resize elements uniformly or on individual axes
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        double value) => builder.Prop("scale", value);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("scale", from, to);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("scale", from, to);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("scale", value);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("scale", build);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("scale", build);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("scale", callback);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("scale", path);
    public static PropsBuilder Scale(
        this PropsBuilder builder,
        params Func<PropsBuilder, PropsBuilder>[] build) => builder.Prop("scale", build);
    
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        double value) => builder.Prop("scaleX", value);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("scaleX", from, to);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("scaleX", from, to);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("scaleX", value);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("scaleX", build);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("scaleX", build);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("scaleX", callback);
    public static PropsBuilder ScaleX(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("scaleX", path);
    
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        double value) => builder.Prop("scaleY", value);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("scaleY", from, to);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("scaleY", from, to);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("scaleY", value);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("scaleY", build);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("scaleY", build);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("scaleY", callback);
    public static PropsBuilder ScaleY(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("scaleY", path);
    
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        double value) => builder.Prop("scaleZ", value);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("scaleZ", from, to);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("scaleZ", from, to);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("scaleZ", value);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("scaleZ", build);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("scaleZ", build);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("scaleZ", callback);
    public static PropsBuilder ScaleZ(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("scaleZ", path);
    
    // Skew: Slant elements along X and Y axes (in degrees)
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        double value) => builder.Prop("skewX", value);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("skewX", from, to);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("skewX", from, to);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("skewX", value);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("skewX", build);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("skewX", build);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("skewX", callback);
    public static PropsBuilder SkewX(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("skewX", path);
    
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        double value) => builder.Prop("skewY", value);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("skewY", from, to);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("skewY", from, to);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("skewY", value);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("skewY", build);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("skewY", build);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("skewY", callback);
    public static PropsBuilder SkewY(
        this PropsBuilder builder,
        SvgPathParam path) => builder.Prop("skewY", path);

    // Color Properties
    // Animatable color values: background, text, and border colors
    // Accepts CSS color values (hex, rgb, rgba, hsl, named colors)
    public static PropsBuilder BackgroundColor(
        this PropsBuilder builder,
        string value) => builder.Prop("backgroundColor", value);
    public static PropsBuilder BackgroundColor(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("backgroundColor", from, to);
    public static PropsBuilder BackgroundColor(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("backgroundColor", build);
    public static PropsBuilder BackgroundColor(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("backgroundColor", build);
    public static PropsBuilder BackgroundColor(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("backgroundColor", callback);
    
    public static PropsBuilder Color(
        this PropsBuilder builder,
        string value) => builder.Prop("color", value);
    public static PropsBuilder Color(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("color", from, to);
    public static PropsBuilder Color(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("color", build);
    public static PropsBuilder Color(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("color", build);
    public static PropsBuilder Color(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("color", callback);
    
    public static PropsBuilder BorderColor(
        this PropsBuilder builder,
        string value) => builder.Prop("borderColor", value);
    public static PropsBuilder BorderColor(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("borderColor", from, to);
    public static PropsBuilder BorderColor(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("borderColor", build);
    public static PropsBuilder BorderColor(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("borderColor", build);
    public static PropsBuilder BorderColor(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("borderColor", callback);
    
    // Opacity
    // Element transparency from 0 (transparent) to 1 (opaque)
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        double value) => builder.Prop("opacity", value);
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("opacity", from, to);
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        Stagger value) => builder.Prop("opacity", value);
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("opacity", build);
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("opacity", build);
    public static PropsBuilder Opacity(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("opacity", callback);
    
    // Border Properties
    // Border styling: radius (rounded corners) and width
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        string value) => builder.Prop("borderRadius", value);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        double value) => builder.Prop("borderRadius", value);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("borderRadius", from, to);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("borderRadius", from, to);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("borderRadius", build);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("borderRadius", build);
    public static PropsBuilder BorderRadius(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("borderRadius", callback);
    
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        double value) => builder.Prop("borderWidth", value);
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("borderWidth", from, to);
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        string value) => builder.Prop("borderWidth", value);
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("borderWidth", from, to);
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("borderWidth", build);
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("borderWidth", build);
    public static PropsBuilder BorderWidth(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("borderWidth", callback);
    
    // Dimension Properties
    // Element size: width and height
    public static PropsBuilder Width(
        this PropsBuilder builder,
        double value) => builder.Prop("width", value);
    public static PropsBuilder Width(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("width", from, to);
    public static PropsBuilder Width(
        this PropsBuilder builder,
        string value) => builder.Prop("width", value);
    public static PropsBuilder Width(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("width", from, to);
    public static PropsBuilder Width(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("width", build);
    public static PropsBuilder Width(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("width", build);
    public static PropsBuilder Width(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("width", callback);
    
    public static PropsBuilder Height(
        this PropsBuilder builder,
        double value) => builder.Prop("height", value);
    public static PropsBuilder Height(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("height", from, to);
    public static PropsBuilder Height(
        this PropsBuilder builder,
        string value) => builder.Prop("height", value);
    public static PropsBuilder Height(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("height", from, to);
    public static PropsBuilder Height(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("height", build);
    public static PropsBuilder Height(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("height", build);
    public static PropsBuilder Height(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("height", callback);
    
    // Position Properties
    // CSS positioning values for positioned elements (absolute, relative, fixed)
    public static PropsBuilder Top(
        this PropsBuilder builder,
        double value) => builder.Prop("top", value);
    public static PropsBuilder Top(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("top", from, to);
    public static PropsBuilder Top(
        this PropsBuilder builder,
        string value) => builder.Prop("top", value);
    public static PropsBuilder Top(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("top", from, to);
    public static PropsBuilder Top(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("top", build);
    public static PropsBuilder Top(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("top", build);
    public static PropsBuilder Top(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("top", callback);
    
    public static PropsBuilder Left(
        this PropsBuilder builder,
        double value) => builder.Prop("left", value);
    public static PropsBuilder Left(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("left", from, to);
    public static PropsBuilder Left(
        this PropsBuilder builder,
        string value) => builder.Prop("left", value);
    public static PropsBuilder Left(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("left", from, to);
    public static PropsBuilder Left(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("left", build);
    public static PropsBuilder Left(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("left", build);
    public static PropsBuilder Left(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("left", callback);
    
    public static PropsBuilder Right(
        this PropsBuilder builder,
        double value) => builder.Prop("right", value);
    public static PropsBuilder Right(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("right", from, to);
    public static PropsBuilder Right(
        this PropsBuilder builder,
        string value) => builder.Prop("right", value);
    public static PropsBuilder Right(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("right", from, to);
    public static PropsBuilder Right(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("right", build);
    public static PropsBuilder Right(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("right", build);
    public static PropsBuilder Right(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("right", callback);
    
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        double value) => builder.Prop("bottom", value);
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("bottom", from, to);
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        string value) => builder.Prop("bottom", value);
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("bottom", from, to);
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("bottom", build);
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("bottom", build);
    public static PropsBuilder Bottom(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("bottom", callback);
    
    // Margin Properties
    // Outer spacing around elements (top, left, right, bottom)
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        double value) => builder.Prop("marginTop", value);
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("marginTop", from, to);
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        string value) => builder.Prop("marginTop", value);
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("marginTop", from, to);
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("marginTop", build);
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("marginTop", build);
    public static PropsBuilder MarginTop(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("marginTop", callback);
    
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        double value) => builder.Prop("marginLeft", value);
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("marginLeft", from, to);
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        string value) => builder.Prop("marginLeft", value);
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("marginLeft", from, to);
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("marginLeft", build);
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("marginLeft", build);
    public static PropsBuilder MarginLeft(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("marginLeft", callback);
    
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        double value) => builder.Prop("marginRight", value);
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("marginRight", from, to);
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        string value) => builder.Prop("marginRight", value);
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("marginRight", from, to);
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("marginRight", build);
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("marginRight", build);
    public static PropsBuilder MarginRight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("marginRight", callback);
    
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        double value) => builder.Prop("marginBottom", value);
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("marginBottom", from, to);
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        string value) => builder.Prop("marginBottom", value);
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("marginBottom", from, to);
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("marginBottom", build);
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("marginBottom", build);
    public static PropsBuilder MarginBottom(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("marginBottom", callback);
    
    // Padding Properties
    // Inner spacing inside elements (top, left, right, bottom)
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        double value) => builder.Prop("paddingTop", value);
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("paddingTop", from, to);
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        string value) => builder.Prop("paddingTop", value);
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("paddingTop", from, to);
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("paddingTop", build);
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("paddingTop", build);
    public static PropsBuilder PaddingTop(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("paddingTop", callback);
    
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        double value) => builder.Prop("paddingLeft", value);
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("paddingLeft", from, to);
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        string value) => builder.Prop("paddingLeft", value);
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("paddingLeft", from, to);
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("paddingLeft", build);
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("paddingLeft", build);
    public static PropsBuilder PaddingLeft(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("paddingLeft", callback);
    
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        double value) => builder.Prop("paddingRight", value);
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("paddingRight", from, to);
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        string value) => builder.Prop("paddingRight", value);
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("paddingRight", from, to);
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("paddingRight", build);
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("paddingRight", build);
    public static PropsBuilder PaddingRight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("paddingRight", callback);
    
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        double value) => builder.Prop("paddingBottom", value);
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("paddingBottom", from, to);
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        string value) => builder.Prop("paddingBottom", value);
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("paddingBottom", from, to);
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("paddingBottom", build);
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("paddingBottom", build);
    public static PropsBuilder PaddingBottom(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("paddingBottom", callback);
    
    // Typography Properties
    // Text styling: size, spacing, and line height
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        double value) => builder.Prop("fontSize", value);
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("fontSize", from, to);
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        string value) => builder.Prop("fontSize", value);
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("fontSize", from, to);
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("fontSize", build);
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("fontSize", build);
    public static PropsBuilder FontSize(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("fontSize", callback);
    
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        double value) => builder.Prop("letterSpacing", value);
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("letterSpacing", from, to);
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        string value) => builder.Prop("letterSpacing", value);
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("letterSpacing", from, to);
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("letterSpacing", build);
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("letterSpacing", build);
    public static PropsBuilder LetterSpacing(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("letterSpacing", callback);
    
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        double value) => builder.Prop("lineHeight", value);
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("lineHeight", from, to);
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        string value) => builder.Prop("lineHeight", value);
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("lineHeight", from, to);
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("lineHeight", build);
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("lineHeight", build);
    public static PropsBuilder LineHeight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("lineHeight", callback);
    
    public static PropsBuilder FontWeight(
        this PropsBuilder builder,
        double value) => builder.Prop("fontWeight", value);
    public static PropsBuilder FontWeight(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("fontWeight", from, to);
    public static PropsBuilder FontWeight(
        this PropsBuilder builder,
        string value) => builder.Prop("fontWeight", value);
    public static PropsBuilder FontWeight(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("fontWeight", from, to);
    public static PropsBuilder FontWeight(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("fontWeight", build);
    public static PropsBuilder FontWeight(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("fontWeight", build);
    public static PropsBuilder FontWeight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("fontWeight", callback);
    
    public static PropsBuilder WordSpacing(
        this PropsBuilder builder,
        double value) => builder.Prop("wordSpacing", value);
    public static PropsBuilder WordSpacing(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("wordSpacing", from, to);
    public static PropsBuilder WordSpacing(
        this PropsBuilder builder,
        string value) => builder.Prop("wordSpacing", value);
    public static PropsBuilder WordSpacing(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("wordSpacing", from, to);
    public static PropsBuilder WordSpacing(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("wordSpacing", build);
    public static PropsBuilder WordSpacing(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("wordSpacing", build);
    public static PropsBuilder WordSpacing(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("wordSpacing", callback);
    
    public static PropsBuilder TextIndent(
        this PropsBuilder builder,
        double value) => builder.Prop("textIndent", value);
    public static PropsBuilder TextIndent(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("textIndent", from, to);
    public static PropsBuilder TextIndent(
        this PropsBuilder builder,
        string value) => builder.Prop("textIndent", value);
    public static PropsBuilder TextIndent(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("textIndent", from, to);
    public static PropsBuilder TextIndent(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("textIndent", build);
    public static PropsBuilder TextIndent(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("textIndent", build);
    public static PropsBuilder TextIndent(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("textIndent", callback);

    // Visual Effects
    // Shadows, filters, and other visual enhancements
    public static PropsBuilder BoxShadow(
        this PropsBuilder builder,
        string value) => builder.Prop("boxShadow", value);
    public static PropsBuilder BoxShadow(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("boxShadow", from, to);
    public static PropsBuilder BoxShadow(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("boxShadow", build);
    public static PropsBuilder BoxShadow(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("boxShadow", build);
    public static PropsBuilder BoxShadow(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("boxShadow", callback);
    
    public static PropsBuilder TextShadow(
        this PropsBuilder builder,
        string value) => builder.Prop("textShadow", value);
    public static PropsBuilder TextShadow(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("textShadow", from, to);
    public static PropsBuilder TextShadow(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("textShadow", build);
    public static PropsBuilder TextShadow(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("textShadow", build);
    public static PropsBuilder TextShadow(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("textShadow", callback);
    
    public static PropsBuilder Filter(
        this PropsBuilder builder,
        string value) => builder.Prop("filter", value);
    public static PropsBuilder Filter(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("filter", from, to);
    public static PropsBuilder Filter(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("filter", build);
    public static PropsBuilder Filter(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("filter", build);
    public static PropsBuilder Filter(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("filter", callback);

    // Min/Max Dimensions
    // Constraints for element sizing
    public static PropsBuilder MinWidth(
        this PropsBuilder builder,
        double value) => builder.Prop("minWidth", value);
    public static PropsBuilder MinWidth(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("minWidth", from, to);
    public static PropsBuilder MinWidth(
        this PropsBuilder builder,
        string value) => builder.Prop("minWidth", value);
    public static PropsBuilder MinWidth(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("minWidth", from, to);
    public static PropsBuilder MinWidth(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("minWidth", build);
    public static PropsBuilder MinWidth(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("minWidth", build);
    public static PropsBuilder MinWidth(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("minWidth", callback);
    
    public static PropsBuilder MaxWidth(
        this PropsBuilder builder,
        double value) => builder.Prop("maxWidth", value);
    public static PropsBuilder MaxWidth(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("maxWidth", from, to);
    public static PropsBuilder MaxWidth(
        this PropsBuilder builder,
        string value) => builder.Prop("maxWidth", value);
    public static PropsBuilder MaxWidth(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("maxWidth", from, to);
    public static PropsBuilder MaxWidth(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("maxWidth", build);
    public static PropsBuilder MaxWidth(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("maxWidth", build);
    public static PropsBuilder MaxWidth(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("maxWidth", callback);
    
    public static PropsBuilder MinHeight(
        this PropsBuilder builder,
        double value) => builder.Prop("minHeight", value);
    public static PropsBuilder MinHeight(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("minHeight", from, to);
    public static PropsBuilder MinHeight(
        this PropsBuilder builder,
        string value) => builder.Prop("minHeight", value);
    public static PropsBuilder MinHeight(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("minHeight", from, to);
    public static PropsBuilder MinHeight(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("minHeight", build);
    public static PropsBuilder MinHeight(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("minHeight", build);
    public static PropsBuilder MinHeight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("minHeight", callback);
    
    public static PropsBuilder MaxHeight(
        this PropsBuilder builder,
        double value) => builder.Prop("maxHeight", value);
    public static PropsBuilder MaxHeight(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("maxHeight", from, to);
    public static PropsBuilder MaxHeight(
        this PropsBuilder builder,
        string value) => builder.Prop("maxHeight", value);
    public static PropsBuilder MaxHeight(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("maxHeight", from, to);
    public static PropsBuilder MaxHeight(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("maxHeight", build);
    public static PropsBuilder MaxHeight(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("maxHeight", build);
    public static PropsBuilder MaxHeight(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("maxHeight", callback);

    // Z-Index
    // Stacking order for positioned elements
    public static PropsBuilder ZIndex(
        this PropsBuilder builder,
        int value) => builder.Prop("zIndex", value);
    public static PropsBuilder ZIndex(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("zIndex", build);
    public static PropsBuilder ZIndex(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("zIndex", build);
    public static PropsBuilder ZIndex(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("zIndex", callback);

    // Outline Properties
    // Element outline styling (similar to border but doesn't affect layout)
    public static PropsBuilder OutlineWidth(
        this PropsBuilder builder,
        double value) => builder.Prop("outlineWidth", value);
    public static PropsBuilder OutlineWidth(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("outlineWidth", from, to);
    public static PropsBuilder OutlineWidth(
        this PropsBuilder builder,
        string value) => builder.Prop("outlineWidth", value);
    public static PropsBuilder OutlineWidth(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("outlineWidth", from, to);
    public static PropsBuilder OutlineWidth(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("outlineWidth", build);
    public static PropsBuilder OutlineWidth(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("outlineWidth", build);
    public static PropsBuilder OutlineWidth(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("outlineWidth", callback);
    
    public static PropsBuilder OutlineColor(
        this PropsBuilder builder,
        string value) => builder.Prop("outlineColor", value);
    public static PropsBuilder OutlineColor(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("outlineColor", from, to);
    public static PropsBuilder OutlineColor(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("outlineColor", build);
    public static PropsBuilder OutlineColor(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("outlineColor", build);
    public static PropsBuilder OutlineColor(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("outlineColor", callback);
    
    public static PropsBuilder OutlineOffset(
        this PropsBuilder builder,
        double value) => builder.Prop("outlineOffset", value);
    public static PropsBuilder OutlineOffset(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("outlineOffset", from, to);
    public static PropsBuilder OutlineOffset(
        this PropsBuilder builder,
        string value) => builder.Prop("outlineOffset", value);
    public static PropsBuilder OutlineOffset(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("outlineOffset", from, to);
    public static PropsBuilder OutlineOffset(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("outlineOffset", build);
    public static PropsBuilder OutlineOffset(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("outlineOffset", build);
    public static PropsBuilder OutlineOffset(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("outlineOffset", callback);

    // Transform Origin
    // Pivot point for transform operations
    public static PropsBuilder TransformOrigin(
        this PropsBuilder builder,
        string value) => builder.Prop("transformOrigin", value);
    public static PropsBuilder TransformOrigin(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("transformOrigin", from, to);
    public static PropsBuilder TransformOrigin(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("transformOrigin", build);
    public static PropsBuilder TransformOrigin(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("transformOrigin", build);
    public static PropsBuilder TransformOrigin(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("transformOrigin", callback);

    // Background Properties
    // Background image positioning and sizing
    public static PropsBuilder BackgroundPosition(
        this PropsBuilder builder,
        string value) => builder.Prop("backgroundPosition", value);
    public static PropsBuilder BackgroundPosition(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("backgroundPosition", from, to);
    public static PropsBuilder BackgroundPosition(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("backgroundPosition", build);
    public static PropsBuilder BackgroundPosition(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("backgroundPosition", build);
    public static PropsBuilder BackgroundPosition(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("backgroundPosition", callback);
    
    public static PropsBuilder BackgroundSize(
        this PropsBuilder builder,
        string value) => builder.Prop("backgroundSize", value);
    public static PropsBuilder BackgroundSize(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("backgroundSize", from, to);
    public static PropsBuilder BackgroundSize(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("backgroundSize", build);
    public static PropsBuilder BackgroundSize(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("backgroundSize", build);
    public static PropsBuilder BackgroundSize(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("backgroundSize", callback);

    // Flexbox Properties
    // Flexible box layout properties
    public static PropsBuilder FlexGrow(
        this PropsBuilder builder,
        double value) => builder.Prop("flexGrow", value);
    public static PropsBuilder FlexGrow(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("flexGrow", from, to);
    public static PropsBuilder FlexGrow(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("flexGrow", build);
    public static PropsBuilder FlexGrow(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("flexGrow", build);
    public static PropsBuilder FlexGrow(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("flexGrow", callback);
    
    public static PropsBuilder FlexShrink(
        this PropsBuilder builder,
        double value) => builder.Prop("flexShrink", value);
    public static PropsBuilder FlexShrink(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("flexShrink", from, to);
    public static PropsBuilder FlexShrink(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("flexShrink", build);
    public static PropsBuilder FlexShrink(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("flexShrink", build);
    public static PropsBuilder FlexShrink(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("flexShrink", callback);
    
    public static PropsBuilder FlexBasis(
        this PropsBuilder builder,
        double value) => builder.Prop("flexBasis", value);
    public static PropsBuilder FlexBasis(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("flexBasis", from, to);
    public static PropsBuilder FlexBasis(
        this PropsBuilder builder,
        string value) => builder.Prop("flexBasis", value);
    public static PropsBuilder FlexBasis(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("flexBasis", from, to);
    public static PropsBuilder FlexBasis(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("flexBasis", build);
    public static PropsBuilder FlexBasis(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("flexBasis", build);
    public static PropsBuilder FlexBasis(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("flexBasis", callback);
    
    public static PropsBuilder Order(
        this PropsBuilder builder,
        int value) => builder.Prop("order", value);
    public static PropsBuilder Order(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("order", build);
    public static PropsBuilder Order(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("order", build);
    public static PropsBuilder Order(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("order", callback);

    // Gap Properties
    // Spacing between flex/grid items
    public static PropsBuilder Gap(
        this PropsBuilder builder,
        double value) => builder.Prop("gap", value);
    public static PropsBuilder Gap(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("gap", from, to);
    public static PropsBuilder Gap(
        this PropsBuilder builder,
        string value) => builder.Prop("gap", value);
    public static PropsBuilder Gap(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("gap", from, to);
    public static PropsBuilder Gap(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("gap", build);
    public static PropsBuilder Gap(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("gap", build);
    public static PropsBuilder Gap(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("gap", callback);
    
    public static PropsBuilder RowGap(
        this PropsBuilder builder,
        double value) => builder.Prop("rowGap", value);
    public static PropsBuilder RowGap(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("rowGap", from, to);
    public static PropsBuilder RowGap(
        this PropsBuilder builder,
        string value) => builder.Prop("rowGap", value);
    public static PropsBuilder RowGap(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("rowGap", from, to);
    public static PropsBuilder RowGap(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("rowGap", build);
    public static PropsBuilder RowGap(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("rowGap", build);
    public static PropsBuilder RowGap(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("rowGap", callback);
    
    public static PropsBuilder ColumnGap(
        this PropsBuilder builder,
        double value) => builder.Prop("columnGap", value);
    public static PropsBuilder ColumnGap(
        this PropsBuilder builder,
        double from, double to) => builder.Prop("columnGap", from, to);
    public static PropsBuilder ColumnGap(
        this PropsBuilder builder,
        string value) => builder.Prop("columnGap", value);
    public static PropsBuilder ColumnGap(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("columnGap", from, to);
    public static PropsBuilder ColumnGap(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("columnGap", build);
    public static PropsBuilder ColumnGap(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("columnGap", build);
    public static PropsBuilder ColumnGap(
        this PropsBuilder builder,
        Func<int, int, double> callback) => builder.Prop("columnGap", callback);

    // Clip Path
    // Shape clipping for elements
    public static PropsBuilder ClipPath(
        this PropsBuilder builder,
        string value) => builder.Prop("clipPath", value);
    public static PropsBuilder ClipPath(
        this PropsBuilder builder,
        string from, string to) => builder.Prop("clipPath", from, to);
    public static PropsBuilder ClipPath(
        this PropsBuilder builder,
        Action<PropsBuilder> build) => builder.Prop("clipPath", build);
    public static PropsBuilder ClipPath(
        this PropsBuilder builder,
        Action<PropsBuilder>[] build) => builder.Prop("clipPath", build);
    public static PropsBuilder ClipPath(
        this PropsBuilder builder,
        Func<int, int, string> callback) => builder.Prop("clipPath", callback);

    // Unit strings and relative operators on the transforms anime.js treats as first-class.
    public static PropsBuilder TranslateX(this PropsBuilder builder, string value) => builder.Prop("translateX", value);
    public static PropsBuilder TranslateX(this PropsBuilder builder, Relative value) => builder.Prop("translateX", value);
    public static PropsBuilder TranslateY(this PropsBuilder builder, string value) => builder.Prop("translateY", value);
    public static PropsBuilder TranslateY(this PropsBuilder builder, Relative value) => builder.Prop("translateY", value);
    public static PropsBuilder TranslateZ(this PropsBuilder builder, string value) => builder.Prop("translateZ", value);
    public static PropsBuilder TranslateZ(this PropsBuilder builder, Relative value) => builder.Prop("translateZ", value);
    public static PropsBuilder Rotate(this PropsBuilder builder, string value) => builder.Prop("rotate", value);
    public static PropsBuilder Rotate(this PropsBuilder builder, Relative value) => builder.Prop("rotate", value);
    public static PropsBuilder RotateX(this PropsBuilder builder, string value) => builder.Prop("rotateX", value);
    public static PropsBuilder RotateX(this PropsBuilder builder, Relative value) => builder.Prop("rotateX", value);
    public static PropsBuilder RotateY(this PropsBuilder builder, string value) => builder.Prop("rotateY", value);
    public static PropsBuilder RotateY(this PropsBuilder builder, Relative value) => builder.Prop("rotateY", value);
    public static PropsBuilder RotateZ(this PropsBuilder builder, string value) => builder.Prop("rotateZ", value);
    public static PropsBuilder RotateZ(this PropsBuilder builder, Relative value) => builder.Prop("rotateZ", value);
    public static PropsBuilder Scale(this PropsBuilder builder, string value) => builder.Prop("scale", value);
    public static PropsBuilder Scale(this PropsBuilder builder, Relative value) => builder.Prop("scale", value);
    public static PropsBuilder ScaleX(this PropsBuilder builder, string value) => builder.Prop("scaleX", value);
    public static PropsBuilder ScaleX(this PropsBuilder builder, Relative value) => builder.Prop("scaleX", value);
    public static PropsBuilder ScaleY(this PropsBuilder builder, string value) => builder.Prop("scaleY", value);
    public static PropsBuilder ScaleY(this PropsBuilder builder, Relative value) => builder.Prop("scaleY", value);
    public static PropsBuilder ScaleZ(this PropsBuilder builder, string value) => builder.Prop("scaleZ", value);
    public static PropsBuilder ScaleZ(this PropsBuilder builder, Relative value) => builder.Prop("scaleZ", value);
    public static PropsBuilder SkewX(this PropsBuilder builder, string value) => builder.Prop("skewX", value);
    public static PropsBuilder SkewX(this PropsBuilder builder, Relative value) => builder.Prop("skewX", value);
    public static PropsBuilder SkewY(this PropsBuilder builder, string value) => builder.Prop("skewY", value);
    public static PropsBuilder SkewY(this PropsBuilder builder, Relative value) => builder.Prop("skewY", value);

    // SVG attributes used by morphing, line drawing, and shape animation.
    public static PropsBuilder D(this PropsBuilder builder, string value) => builder.Prop("d", value);
    public static PropsBuilder D(this PropsBuilder builder, string from, string to) => builder.Prop("d", from, to);
    public static PropsBuilder D(this PropsBuilder builder, params Action<PropsBuilder>[] build) => builder.Prop("d", build);
    public static PropsBuilder Fill(this PropsBuilder builder, string value) => builder.Prop("fill", value);
    public static PropsBuilder Fill(this PropsBuilder builder, string from, string to) => builder.Prop("fill", from, to);
    public static PropsBuilder Stroke(this PropsBuilder builder, string value) => builder.Prop("stroke", value);
    public static PropsBuilder Stroke(this PropsBuilder builder, string from, string to) => builder.Prop("stroke", from, to);
    public static PropsBuilder StrokeWidth(this PropsBuilder builder, double value) => builder.Prop("strokeWidth", value);
    public static PropsBuilder StrokeWidth(this PropsBuilder builder, double from, double to) => builder.Prop("strokeWidth", from, to);
    public static PropsBuilder StrokeWidth(this PropsBuilder builder, string value) => builder.Prop("strokeWidth", value);
    public static PropsBuilder Cx(this PropsBuilder builder, double value) => builder.Prop("cx", value);
    public static PropsBuilder Cx(this PropsBuilder builder, double from, double to) => builder.Prop("cx", from, to);
    public static PropsBuilder Cy(this PropsBuilder builder, double value) => builder.Prop("cy", value);
    public static PropsBuilder Cy(this PropsBuilder builder, double from, double to) => builder.Prop("cy", from, to);
    public static PropsBuilder R(this PropsBuilder builder, double value) => builder.Prop("r", value);
    public static PropsBuilder R(this PropsBuilder builder, double from, double to) => builder.Prop("r", from, to);
    public static PropsBuilder Rx(this PropsBuilder builder, double value) => builder.Prop("rx", value);
    public static PropsBuilder Ry(this PropsBuilder builder, double value) => builder.Prop("ry", value);
    public static PropsBuilder X(this PropsBuilder builder, double value) => builder.Prop("x", value);
    public static PropsBuilder X(this PropsBuilder builder, string value) => builder.Prop("x", value);
    public static PropsBuilder Y(this PropsBuilder builder, double value) => builder.Prop("y", value);
    public static PropsBuilder Y(this PropsBuilder builder, string value) => builder.Prop("y", value);
}