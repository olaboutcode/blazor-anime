namespace BlazorAnime;

using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public abstract partial class Prop
{
    /// <summary>
    /// Can be any CSS selectors
    /// <para>
    /// Pseudo elements can't be targeted using JavaScript.
    /// </para>
    /// </summary>
    /// <param name="element"></param>
    /// <returns>Animation Property</returns>
    public static Prop Targets(string element) => Create("targets", element);
    
    /// <summary>
    /// Can be any CSS selectors
    /// <para>
    /// Pseudo elements can't be targeted using JavaScript.
    /// </para>
    /// </summary>
    /// <param name="elements"></param>
    /// <returns>Animation Property</returns>
    public static Prop Targets(string[] elements) => Create("targets", elements);
    
    /// <summary>
    /// Can be any custom objects, DOM Nodes, or NodeList
    /// <para>
    /// Custom objects with at least one property containing a numerical value.
    /// </para>
    /// </summary>
    /// <param name="elements"></param>
    /// <returns>Animation Property</returns>
    public static Prop Targets(object[] elements) => Create("targets", elements);

    /// <summary>
    /// Defines the duration in milliseconds of the animation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Duration(double value) => Create("duration", value);
    
    /// <summary>
    /// Defines the duration in milliseconds of the animation.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static Prop Duration(double from, double to) => Create("duration", from, to);
    
    /// <summary>
    /// Defines the duration in milliseconds of the animation.
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static Prop Duration(List<Prop> props) => Create("duration", props);

    /// <summary>
    /// Defines the delay in milliseconds of the animation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Delay(double value) => Create("delay", value);
    
    /// <summary>
    /// Defines the delay in milliseconds of the animation with a custom callback
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop Delay(Delegate callback) => Create("delay", callback);

    /// <summary>
    /// Defines the staggered delay in milliseconds of the animation
    /// </summary>
    /// <param name="stagger"></param>
    /// <returns>Animation Property</returns>
    public static Prop Delay(Stagger stagger) =>
        new StgProp("delay", stagger.GetValue(), stagger.GetOptions().ToObject());

    /// <summary>
    /// Adds some extra time in milliseconds at the end of the animation
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop EndDelay(double value) => Create("endDelay", value);
    
    /// <summary>
    /// Adds some extra time in milliseconds at the end of the animation with a custom callback
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop EndDelay(Delegate callback) => Create("delay", callback);
    
    /// <summary>
    /// Adds staggered extra time in milliseconds at the end of the animation
    /// </summary>
    /// <param name="stagger"></param>
    /// <returns>Animation Property</returns>
    public static Prop EndDelay(Stagger stagger) =>
        new StgProp("delay", stagger.GetValue(), stagger.GetOptions().ToObject());

    /// <summary>
    /// Rounds up the value to x decimals.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Round(int value) => Create("round", value);

    /// <summary>
    /// Defines the number of iterations of an animation.
    /// <para>
    /// True - Loop indefinitely
    /// </para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Loop(bool value) => Create("loop", value);
    
    /// <summary>
    /// Defines the number of iterations of an animation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Loop(int value) => Create("loop", value);

    /// <summary>
    /// Autoplay animation
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop AutoPlay(bool value) => Create("autoplay", value);

    /// <summary>
    /// Define an animation prop value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(double value) => Create("value", value);
    
    /// <summary>
    /// Define an animation prop value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(bool value) => Create("value", value);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(string value) => Create("value", value);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="values"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(string[] values) => Create("value", values);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(Color value) => Create("value", value);
    
    /// <summary>
    /// Define an animation prop value
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(List<Prop> props) => Create("value", props);
    
    /// <summary>
    /// Define an animation prop value
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(double from, double to) => Create("value", from, to);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(string from, string to) => Create("value", from, to);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(Color from, Color to) => Create("value", from, to);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static Prop Value(List<List<Prop>> props) => Create("value", props);

    /// <summary>
    /// Animation keyframes are defined using an Array, within the keyframes property.
    /// <para>
    /// If there is no duration specified inside the keyframes,
    /// each keyframe duration will be <br/> equal to the animation's 
    /// total duration divided by the number of keyframes.
    /// </para>
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static Prop Keyframes(List<List<Prop>> props) => Create("keyframes", props);

    /// <summary>
    /// Callback triggered on every frame as soon as the animation starts playing.
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop Update(Delegate callback) =>
        Create("update", callback);

    /// <summary>
    /// Callback is triggered once, when the animation starts playing.
    /// <para>
    /// Begin() callbacks are called if the animation's duration is 0.
    /// </para>
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop Begin(Delegate callback) =>
        Create("begin", callback);
    
    /// <summary>
    /// Callback is triggered once, when the animation is completed.
    /// <para>
    /// Complete() callbacks are called if the animation's duration is 0.
    /// </para>
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop Complete(Delegate callback) =>
        Create("complete", callback);

    

    /// <summary>
    /// LoopBegin() callback is triggered once everytime a loop begin.
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop LoopBegin(Delegate callback) =>
        Create("loopBegin", callback);
    
    /// <summary>
    /// LoopComplete() callback is triggered once everytime a loop is completed.
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop LoopComplete(Delegate callback) =>
        Create("loopComplete", callback);

    /// <summary>
    /// Callback triggered on every frames in between the animation's delay and endDelay.
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop Change(Delegate callback) =>
        Create("change", callback);
    
    /// <summary>
    /// changeBegin() callback is triggered everytime the animation starts changing.
    /// <para>
    /// Animation direction will affect the order in which changeBegin() is triggered.
    /// </para>
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop ChangeBegin(Delegate callback) =>
        Create("changeBegin", callback);
    
    /// <summary>
    /// changeComplete() callback is triggered everytime the animation stops changing.
    /// <para>
    /// Animation direction will affect the order in which changeComplete() is triggered.
    /// </para>
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static Prop ChangeComplete(Delegate callback) =>
        Create("changeComplete", callback);

    /// <summary>
    /// Creates path drawing animation using the 'stroke-dashoffset' property.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop StrokeDashoffset(int value) => Create("strokeDashoffset", value);

    /// <summary>
    /// Creates transition between two svg shapes.
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static Prop Points(List<Prop> props) => Create("points", props);
    
    /// <summary>
    /// Creates transition between two svg shapes.
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static Prop Points(List<List<Prop>> props) => Create("points", props);

    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, bool value) => new GenProp<bool>(name, value);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, int value) => new GenProp<int>(name, value);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, double value) => new GenProp<double>(name, value);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, string value) => new GenProp<string>(name, value);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, object value) => new GenProp<object>(name, value);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, Color value) => new GenProp<string>(name, value.GetValue());
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, string[] value) => new GenProp<string[]>(name, value);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, double[] value) => new GenProp<double[]>(name, value);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, object[] value) => new GenProp<object[]>(name, value);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, IEnumerable<Prop> props) => new GenProp<object>(name, props.ToObject());
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, double from, double to) => new GenProp<object>(name, new[] { from, to });
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, string from, string to) => new GenProp<string[]>(name, [from, to]);
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, Color from, Color to) =>
        new GenProp<object>(name, new[] { from.GetValue(), to.GetValue() });
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, List<List<Prop>> props) =>
        new GenProp<object>(name, props.Select(p => p.ToObject()));
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="stagger"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, Stagger stagger) =>
        new StgProp(name, stagger.GetValue(), stagger.GetOptions().ToObject());

    /// <summary>
    /// Animate any CSS property with a custom callback
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="propName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    /// Throws ArgumentException if the method is not found in the caller object
    public static Prop Create(string propName, Delegate callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        bool isLambda = callback.Method.IsDefined(typeof(CompilerGeneratedAttribute), false);
        if (isLambda)
        {
            throw new ArgumentException(
                "Lambda expressions are not supported as callbacks. Please use a named method.");
        }

        if (callback.GetInvocationList().Length > 1)
        {
            throw new ArgumentException(
                "Multicast delegates are not supported as callbacks. Please use a single method.");
        }

        object target = callback.Target
            ?? throw new ArgumentException("Callback target cannot be null.");

        var callerType = target.GetType();
        var methodName = callback.Method.Name;
        var paramsCount = callerType.GetMethod(methodName)?.GetParameters().Length
            ?? throw new ArgumentException($"Method {methodName} not found in {callerType.FullName}");
        
        return new CallbackProp(
            propName,
            methodName,
            paramsCount,
            DotNetObjectReference.Create(target));
    }
    
    /// <summary>
    /// Animate any CSS property.
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static Prop Create(string name, PathParam value) => new SvgProp(name, value.ParamRef);

    /// <summary>
    /// Adds, subtracts or multiplies the original value.
    /// </summary>
    public abstract class RelativeValue
    {
        /// <summary>
        /// Adds to the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static Prop Add(double value) => Create("value", $"+={value}");
        
        /// <summary>
        /// Adds to the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static Prop Add(string value) => Create("value", $"+={value}");

        /// <summary>
        /// Subtracts from the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static Prop Subtract(double value) => Create("value", $"-={value}");
        
        /// <summary>
        /// Subtracts from the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static Prop Subtract(string value) => Create("value", $"-={value}");

        /// <summary>
        /// Multiplies the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static Prop Multiply(double value) => Create("value", $"*={value}");
        
        /// <summary>
        /// Multiplies the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static Prop Multiply(string value) => Create("value", $"*={value}");
    }

    /// <summary>
    /// Defines the direction of the animation.
    /// </summary>
    public abstract class Direction
    {
        /// <summary>
        /// Defines the direction of the animation.
        /// <para>
        /// Animation progress goes from 0 to 100%
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop Normal { get; } = Create("direction", "normal");

        /// <summary>
        /// Defines the direction of the animation.
        /// <para>
        /// Animation progress goes from 100% to 0%
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop Reverse { get; } = Create("direction", "reverse");

        /// <summary>
        /// Defines the direction of the animation.
        /// <para>
        /// Animation progress goes from 0% to 100% then goes back to 0%
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop Alternate { get; } = Create("direction", "alternate");
    }


    /// <summary>
    /// Defines the timing function of the animation.
    /// </summary>
    public abstract class Easing
    {
        /// <summary>
        /// Defines the timing function of the animation.
        /// <para>
        /// Does not apply any easing timing to your animation. Useful for opacity and colors transitions.
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop Linear { get; } = Create("Easing", "linear");
        
        /// <summary>
        /// Defines the number of jumps an animation takes to arrive at its end value.
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop Steps(int steps) => Create("Easing", $"steps({steps})");
        
        /// <summary>
        /// Defines the timing function of the animation.
        /// <para>
        /// Use your own custom cubic Bézier curves.
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop CubicBezier(double x1, double y1, double x2, double y2) =>
            Create("Easing", $"cubicBezier({x1}, {y1}, {x2}, {y2})");
        
        /// <summary>
        /// Defines the timing function of the animation.
        /// <para>
        /// Spring physics based
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop Spring() => Create("Easing", "spring");
        
        /// <summary>
        /// Defines the timing function of the animation.
        /// <para>
        /// Spring physics based
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop Spring(double mass, double stiffness, double damping, double velocity)
            => Create("Easing", $"spring({mass}, {stiffness}, {damping}, {velocity})");
        
        /// <summary>
        /// Defines Elastic easing
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop EaseInElastic(double amplitude, double period)
            => Create("Easing", $"easeInElastic({amplitude}, {period})");
        
        /// <summary>
        /// Defines Elastic easing
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop EaseOutElastic(double amplitude, double period)
            => Create("Easing", $"easeOutElastic({amplitude}, {period})");
        
        /// <summary>
        /// Defines Elastic easing
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop EaseInOutElastic(double amplitude, double period)
            => Create("Easing", $"easeInOutElastic({amplitude}, {period})");
        
        /// <summary>
        /// Defines Elastic easing
        /// </summary>
        /// <returns>Animation Property</returns>
        public static Prop EaseOutInElastic(double amplitude, double period)
            => Create("Easing", $"easeOutInElastic({amplitude}, {period})");

        public static Prop EaseInQuad { get; } = Create("Easing", "easeInQuad");
        public static Prop EaseInCubic { get; } = Create("Easing", "easeInCubic");
        public static Prop EaseInQuart { get; } = Create("Easing", "easeInQuart");
        public static Prop EaseInQuint { get; } = Create("Easing", "easeInQuint");
        public static Prop EaseInSine { get; } = Create("Easing", "easeInSine");
        public static Prop EaseInExpo { get; } = Create("Easing", "easeInExpo");
        public static Prop EaseInCirc { get; } = Create("Easing", "easeInCirc");
        public static Prop EaseInBack { get; } = Create("Easing", "easeInBack");
        public static Prop EaseInBounce { get; } = Create("Easing", "easeInBounce");

        public static Prop EaseOutQuad { get; } = Create("Easing", "easeOutQuad");
        public static Prop EaseOutCubic { get; } = Create("Easing", "easeOutCubic");
        public static Prop EaseOutQuart { get; } = Create("Easing", "easeOutQuart");
        public static Prop EaseOutQuint { get; } = Create("Easing", "easeOutQuint");
        public static Prop EaseOutSine { get; } = Create("Easing", "easeOutSine");
        public static Prop EaseOutExpo { get; } = Create("Easing", "easeOutExpo");
        public static Prop EaseOutCirc { get; } = Create("Easing", "easeOutCirc");
        public static Prop EaseOutBack { get; } = Create("Easing", "easeOutBack");
        public static Prop EaseOutBounce { get; } = Create("Easing", "easeOutBounce");

        public static Prop EaseInOutQuad { get; } = Create("Easing", "easeInOutQuad");
        public static Prop EaseInOutCubic { get; } = Create("Easing", "easeInOutCubic");
        public static Prop EaseInOutQuart { get; } = Create("Easing", "easeInOutQuart");
        public static Prop EaseInOutQuint { get; } = Create("Easing", "easeInOutQuint");
        public static Prop EaseInOutSine { get; } = Create("Easing", "easeInOutSine");
        public static Prop EaseInOutExpo { get; } = Create("Easing", "easeInOutExpo");
        public static Prop EaseInOutCirc { get; } = Create("Easing", "easeInOutCirc");
        public static Prop EaseInOutBack { get; } = Create("Easing", "easeInOutBack");
        public static Prop EaseInOutBounce { get; } = Create("Easing", "easeInOutBounce");

        public static Prop EaseOutInQuad { get; } = Create("Easing", "easeOutInQuad");
        public static Prop EaseOutInCubic { get; } = Create("Easing", "easeOutInCubic");
        public static Prop EaseOutInQuart { get; } = Create("Easing", "easeOutInQuart");
        public static Prop EaseOutInQuint { get; } = Create("Easing", "easeOutInQuint");
        public static Prop EaseOutInSine { get; } = Create("Easing", "easeOutInSine");
        public static Prop EaseOutInExpo { get; } = Create("Easing", "easeOutInExpo");
        public static Prop EaseOutInCirc { get; } = Create("Easing", "easeOutInCirc");
        public static Prop EaseOutInBack { get; } = Create("Easing", "easeOutInBack");
        public static Prop EaseOutInBounce { get; } = Create("Easing", "easeOutInBounce");
    }
}