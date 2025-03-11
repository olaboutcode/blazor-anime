namespace BlazorAnime;

using Microsoft.JSInterop;

public abstract class Prop
{
    /// <summary>
    /// Can be any CSS selectors
    /// <para>
    /// Pseudo elements can't be targeted using JavaScript.
    /// </para>
    /// </summary>
    /// <param name="el"></param>
    /// <returns>Animation Property</returns>
    public static AProp Targets(string el) => Create("targets", el);
    
    /// <summary>
    /// Can be any CSS selectors
    /// <para>
    /// Pseudo elements can't be targeted using JavaScript.
    /// </para>
    /// </summary>
    /// <param name="els"></param>
    /// <returns>Animation Property</returns>
    public static AProp Targets(string[] els) => Create("targets", els);
    
    /// <summary>
    /// Can be any custom objects, DOM Nodes, or NodeList
    /// <para>
    /// Custom objects with at least one property containing a numerical value.
    /// </para>
    /// </summary>
    /// <param name="els"></param>
    /// <returns>Animation Property</returns>
    public static AProp Targets(object[] els) => Create("targets", els);

    /// <summary>
    /// Defines the duration in milliseconds of the animation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Duration(double value) => Create("duration", value);
    
    /// <summary>
    /// Defines the duration in milliseconds of the animation.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static AProp Duration(double from, double to) => Create("duration", from, to);
    
    /// <summary>
    /// Defines the duration in milliseconds of the animation.
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static AProp Duration(IEnumerable<AProp> props) => Create("duration", props);

    /// <summary>
    /// Defines the delay in milliseconds of the animation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Delay(double value) => Create("delay", value);
    
    /// <summary>
    /// Defines the delay in milliseconds of the animation with a custom callback
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp Delay(object caller, string callbackName, Func<double> callback) =>
        Create("delay", caller, callbackName, callback);
    
    /// <summary>
    /// Defines the delay in milliseconds of the animation with a custom callback
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp Delay(object caller, string callbackName, Func<int, double> callback) =>
        Create("delay", caller, callbackName, callback);
    
    /// <summary>
    /// Defines the delay in milliseconds of the animation with a custom callback
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp Delay(object caller, string callbackName, Func<int, int, double> callback) =>
        Create("delay", caller, callbackName, callback);
    
    /// <summary>
    /// Defines the staggered delay in milliseconds of the animation
    /// </summary>
    /// <param name="stagger"></param>
    /// <returns>Animation Property</returns>
    public static AProp Delay(Stagger stagger) =>
        new StgProp("delay", stagger.GetValue(), stagger.GetOptions().ToObject());

    /// <summary>
    /// Adds some extra time in milliseconds at the end of the animation
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp EndDelay(double value) => Create("endDelay", value);
    
    /// <summary>
    /// Adds some extra time in milliseconds at the end of the animation with a custom callback
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp EndDelay(object caller, string callbackName, Func<double> callback) =>
        Create("delay", caller, callbackName, callback);
    
    /// <summary>
    /// Adds some extra time in milliseconds at the end of the animation with a custom callback
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp EndDelay(object caller, string callbackName, Func<int, double> callback) =>
        Create("delay", caller, callbackName, callback);
    
    /// <summary>
    /// Adds some extra time in milliseconds at the end of the animation with a custom callback
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp EndDelay(object caller, string callbackName, Func<int, int, double> callback) =>
        Create("delay", caller, callbackName, callback);
    
    /// <summary>
    /// Adds staggered extra time in milliseconds at the end of the animation
    /// </summary>
    /// <param name="stagger"></param>
    /// <returns>Animation Property</returns>
    public static AProp EndDelay(Stagger stagger) =>
        new StgProp("delay", stagger.GetValue(), stagger.GetOptions().ToObject());

    /// <summary>
    /// Rounds up the value to x decimals.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Round(int value) => Create("round", value);

    /// <summary>
    /// Defines the number of iterations of an animation.
    /// <para>
    /// True - Loop indefinitely
    /// </para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Loop(bool value) => Create("loop", value);
    
    /// <summary>
    /// Defines the number of iterations of an animation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Loop(int value) => Create("loop", value);

    /// <summary>
    /// Autoplay animation
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp AutoPlay(bool value) => Create("autoplay", value);

    /// <summary>
    /// Define an animation prop value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(double value) => Create("value", value);
    
    /// <summary>
    /// Define an animation prop value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(bool value) => Create("value", value);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(string value) => Create("value", value);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(Color value) => Create("value", value);
    
    /// <summary>
    /// Define an animation prop value
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(IEnumerable<AProp> props) => Create("value", props);
    
    /// <summary>
    /// Define an animation prop value
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(double from, double to) => Create("value", from, to);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(string from, string to) => Create("value", from, to);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(Color from, Color to) => Create("value", from, to);
    
    /// <summary>
    /// Define an animation prop value
    /// <para>
    /// If the original value has a unit, it will be automatically added to the animated value.
    /// </para>
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static AProp Value(List<IEnumerable<AProp>> props) => Create("value", props);

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
    public static AProp Keyframes(List<IEnumerable<AProp>> props) => Create("keyframes", props);

    /// <summary>
    /// Callback triggered on every frame as soon as the animation starts playing.
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp Update(object caller, string callbackName, Action callback) =>
        Create("update", caller, callbackName, callback);

    /// <summary>
    /// Callback is triggered once, when the animation starts playing.
    /// <para>
    /// Begin() callbacks are called if the animation's duration is 0.
    /// </para>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp Begin(object caller, string callbackName, Action callback) =>
        Create("begin", caller, callbackName, callback);
    
    /// <summary>
    /// Callback is triggered once, when the animation is completed.
    /// <para>
    /// Complete() callbacks are called if the animation's duration is 0.
    /// </para>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp Complete(object caller, string callbackName, Action callback) =>
        Create("complete", caller, callbackName, callback);

    /// <summary>
    /// LoopBegin() callback is triggered once everytime a loop begin.
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp LoopBegin(object caller, string callbackName, Action callback) =>
        Create("loopBegin", caller, callbackName, callback);
    
    /// <summary>
    /// LoopComplete() callback is triggered once everytime a loop is completed.
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp LoopComplete(object caller, string callbackName, Action callback) =>
        Create("loopComplete", caller, callbackName, callback);

    /// <summary>
    /// Callback triggered on every frames in between the animation's delay and endDelay.
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp Change(object caller, string callbackName, Action callback) =>
        Create("change", caller, callbackName, callback);
    
    /// <summary>
    /// changeBegin() callback is triggered everytime the animation starts changing.
    /// <para>
    /// Animation direction will affect the order in which changeBegin() is triggered.
    /// </para>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp ChangeBegin(object caller, string callbackName, Action callback) =>
        Create("changeBegin", caller, callbackName, callback);
    
    /// <summary>
    /// changeComplete() callback is triggered everytime the animation stops changing.
    /// <para>
    /// Animation direction will affect the order in which changeComplete() is triggered.
    /// </para>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="callback"></param>
    /// <returns>Animation Property</returns>
    public static AProp ChangeComplete(object caller, string callbackName, Action callback) =>
        Create("changeComplete", caller, callbackName, callback);

    /// <summary>
    /// Creates path drawing animation using the 'stroke-dashoffset' property.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Animation Property</returns>
    public static AProp StrokeDashoffset(int value) => Create("strokeDashoffset", value);

    /// <summary>
    /// Creates transition between two svg shapes.
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation Property</returns>
    public static AProp Points(List<IEnumerable<AProp>> props) => Create("points", props);

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
    public static AProp Create(string name, bool value) => new GenProp<bool>(name, value);
    
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
    public static AProp Create(string name, int value) => new GenProp<int>(name, value);
    
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
    public static AProp Create(string name, double value) => new GenProp<double>(name, value);
    
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
    public static AProp Create(string name, string value) => new GenProp<string>(name, value);
    
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
    public static AProp Create(string name, object value) => new GenProp<object>(name, value);
    
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
    public static AProp Create(string name, Color value) => new GenProp<string>(name, value.GetValue());
    
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
    public static AProp Create(string name, string[] value) => new GenProp<string[]>(name, value);
    
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
    public static AProp Create(string name, double[] value) => new GenProp<double[]>(name, value);
    
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
    public static AProp Create(string name, object[] value) => new GenProp<object[]>(name, value);
    
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
    public static AProp Create(string name, IEnumerable<AProp> props) => new GenProp<object>(name, props.ToObject());
    
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
    public static AProp Create(string name, double from, double to) => new GenProp<object>(name, new[] { from, to });
    
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
    public static AProp Create(string name, string from, string to) => new GenProp<string[]>(name, [from, to]);
    
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
    public static AProp Create(string name, Color from, Color to) =>
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
    public static AProp Create(string name, List<IEnumerable<AProp>> props) =>
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
    public static AProp Create(string name, Stagger stagger) =>
        new StgProp(name, stagger.GetValue(), stagger.GetOptions().ToObject());
    
    /// <summary>
    /// Animate any CSS property with a custom callback
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="propName"></param>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="_"></param>
    /// <returns>Animation Property</returns>
    public static AProp Create(string propName, object caller, string callbackName, Action _) =>
        new CallbackProp(propName, callbackName, 0, DotNetObjectReference.Create(caller));
    
    /// <summary>
    /// Animate any CSS property with a custom callback
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="propName"></param>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="_"></param>
    /// <returns>Animation Property</returns>
    public static AProp Create(string propName, object caller, string callbackName, Func<double> _) =>
        new CallbackProp(propName, callbackName, 2, DotNetObjectReference.Create(caller));
    
    /// <summary>
    /// Animate any CSS property with a custom callback
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="propName"></param>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="_"></param>
    /// <returns>Animation Property</returns>
    public static AProp Create(string propName, object caller, string callbackName, Func<int, double> _) =>
        new CallbackProp(propName, callbackName, 2, DotNetObjectReference.Create(caller));
    
    /// <summary>
    /// Animate any CSS property with a custom callback
    /// <para>
    /// Most CSS properties will cause layout changes or repaint, and will result in choppy animation.
    /// <br/>Prioritize opacity and CSS transforms as much as possible.
    /// </para>
    /// </summary>
    /// <param name="propName"></param>
    /// <param name="caller"></param>
    /// <param name="callbackName"></param>
    /// <param name="_"></param>
    /// <returns>Animation Property</returns>
    public static AProp Create(string propName, object caller, string callbackName, Func<int, int, double> _) =>
        new CallbackProp(propName, callbackName, 3, DotNetObjectReference.Create(caller));
    
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
    public static AProp Create(string name, PathParm value) =>new SvgProp(name, value.ParamRef);

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
        public static AProp Add(double value) => Create("value", $"+={value}");
        
        /// <summary>
        /// Adds to the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static AProp Add(string value) => Create("value", $"+={value}");

        /// <summary>
        /// Subtracts from the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static AProp Subtract(double value) => Create("value", $"-={value}");
        
        /// <summary>
        /// Subtracts from the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static AProp Subtract(string value) => Create("value", $"-={value}");

        /// <summary>
        /// Multiplies the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static AProp Multiply(double value) => Create("value", $"*={value}");
        
        /// <summary>
        /// Multiplies the original value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Animation Property</returns>
        public static AProp Multiply(string value) => Create("value", $"*={value}");
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
        public static AProp Normal { get; } = Create("direction", "normal");

        /// <summary>
        /// Defines the direction of the animation.
        /// <para>
        /// Animation progress goes from 100% to 0%
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp Reverse { get; } = Create("direction", "reverse");

        /// <summary>
        /// Defines the direction of the animation.
        /// <para>
        /// Animation progress goes from 0% to 100% then goes back to 0%
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp Alternate { get; } = Create("direction", "alternate");
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
        public static AProp Linear { get; } = Create("Easing", "linear");
        
        /// <summary>
        /// Defines the number of jumps an animation takes to arrive at its end value.
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp Steps(int steps) => Create("Easing", $"steps({steps})");
        
        /// <summary>
        /// Defines the timing function of the animation.
        /// <para>
        /// Use your own custom cubic Bézier curves.
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp CubicBezier(double x1, double y1, double x2, double y2) =>
            Create("Easing", $"cubicBezier({x1}, {y1}, {x2}, {y2})");
        
        /// <summary>
        /// Defines the timing function of the animation.
        /// <para>
        /// Spring physics based
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp Spring() => Create("Easing", "spring");
        
        /// <summary>
        /// Defines the timing function of the animation.
        /// <para>
        /// Spring physics based
        /// </para>
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp Spring(double mass, double stiffness, double damping, double velocity)
            => Create("Easing", $"spring({mass}, {stiffness}, {damping}, {velocity})");
        
        /// <summary>
        /// Defines Elastic easing
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp EaseInElastic(double amplitude, double period)
            => Create("Easing", $"easeInElastic({amplitude}, {period})");
        
        /// <summary>
        /// Defines Elastic easing
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp EaseOutElastic(double amplitude, double period)
            => Create("Easing", $"easeOutElastic({amplitude}, {period})");
        
        /// <summary>
        /// Defines Elastic easing
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp EaseInOutElastic(double amplitude, double period)
            => Create("Easing", $"easeInOutElastic({amplitude}, {period})");
        
        /// <summary>
        /// Defines Elastic easing
        /// </summary>
        /// <returns>Animation Property</returns>
        public static AProp EaseOutInElastic(double amplitude, double period)
            => Create("Easing", $"easeOutInElastic({amplitude}, {period})");

        public static AProp EaseInQuad { get; } = Create("Easing", "easeInQuad");
        public static AProp EaseInCubic { get; } = Create("Easing", "easeInCubic");
        public static AProp EaseInQuart { get; } = Create("Easing", "easeInQuart");
        public static AProp EaseInQuint { get; } = Create("Easing", "easeInQuint");
        public static AProp EaseInSine { get; } = Create("Easing", "easeInSine");
        public static AProp EaseInExpo { get; } = Create("Easing", "easeInExpo");
        public static AProp EaseInCirc { get; } = Create("Easing", "easeInCirc");
        public static AProp EaseInBack { get; } = Create("Easing", "easeInBack");
        public static AProp EaseInBounce { get; } = Create("Easing", "easeInBounce");

        public static AProp EaseOutQuad { get; } = Create("Easing", "easeOutQuad");
        public static AProp EaseOutCubic { get; } = Create("Easing", "easeOutCubic");
        public static AProp EaseOutQuart { get; } = Create("Easing", "easeOutQuart");
        public static AProp EaseOutQuint { get; } = Create("Easing", "easeOutQuint");
        public static AProp EaseOutSine { get; } = Create("Easing", "easeOutSine");
        public static AProp EaseOutExpo { get; } = Create("Easing", "easeOutExpo");
        public static AProp EaseOutCirc { get; } = Create("Easing", "easeOutCirc");
        public static AProp EaseOutBack { get; } = Create("Easing", "easeOutBack");
        public static AProp EaseOutBounce { get; } = Create("Easing", "easeOutBounce");

        public static AProp EaseInOutQuad { get; } = Create("Easing", "easeInOutQuad");
        public static AProp EaseInOutCubic { get; } = Create("Easing", "easeInOutCubic");
        public static AProp EaseInOutQuart { get; } = Create("Easing", "easeInOutQuart");
        public static AProp EaseInOutQuint { get; } = Create("Easing", "easeInOutQuint");
        public static AProp EaseInOutSine { get; } = Create("Easing", "easeInOutSine");
        public static AProp EaseInOutExpo { get; } = Create("Easing", "easeInOutExpo");
        public static AProp EaseInOutCirc { get; } = Create("Easing", "easeInOutCirc");
        public static AProp EaseInOutBack { get; } = Create("Easing", "easeInOutBack");
        public static AProp EaseInOutBounce { get; } = Create("Easing", "easeInOutBounce");

        public static AProp EaseOutInQuad { get; } = Create("Easing", "easeOutInQuad");
        public static AProp EaseOutInCubic { get; } = Create("Easing", "easeOutInCubic");
        public static AProp EaseOutInQuart { get; } = Create("Easing", "easeOutInQuart");
        public static AProp EaseOutInQuint { get; } = Create("Easing", "easeOutInQuint");
        public static AProp EaseOutInSine { get; } = Create("Easing", "easeOutInSine");
        public static AProp EaseOutInExpo { get; } = Create("Easing", "easeOutInExpo");
        public static AProp EaseOutInCirc { get; } = Create("Easing", "easeOutInCirc");
        public static AProp EaseOutInBack { get; } = Create("Easing", "easeOutInBack");
        public static AProp EaseOutInBounce { get; } = Create("Easing", "easeOutInBounce");
    }
}