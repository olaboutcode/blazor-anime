namespace BlazorAnime;

/// <summary>
/// Staggering allows you to animate multiple elements with follow through and overlapping action.
/// </summary>
public class Stagger
{
    /// <summary>
    /// Creates a basic stagger
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Stagger Create(double value) => new(value, []);
    
    /// <summary>
    /// Creates a basic stagger
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Stagger Create(double[] value) => new(value, []);
    
    /// <summary>
    /// Creates a basic stagger
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Stagger Create(string value) => new(value, []);
    
    /// <summary>
    /// Creates a basic stagger
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Stagger Create(string[] value) => new(value, []);
    
    /// <summary>
    /// Creates a stagger with options
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Stagger Create(double value, List<StgOptionProp> options) => 
        new(value, options);
    
    /// <summary>
    /// Creates a stagger with options
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Stagger Create(string value, List<StgOptionProp> options) => 
        new(value, options);

    /// <summary>
    /// Distributes evenly values between two numbers.
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <returns></returns>
    public static Stagger Create(double startValue, double endValue) =>
        new(new[] { startValue, endValue }, []);

    /// <summary>
    /// Distributes evenly values between two numbers with options.
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Stagger Create(double startValue, double endValue, List<StgOptionProp> options) =>
        new(new[] { startValue, endValue }, options);

    public object GetValue() => Value;
    
    public List<StgOptionProp> GetOptions() => Options;

    private object Value { get; }
    
    private List<StgOptionProp> Options { get; }

    private Stagger(object value, List<StgOptionProp> options)
    {
        Value = value;
        Options = options;
    }
}

/// <summary>
/// Defines Stagger options
/// </summary>
public abstract class StaggerProp
{
    /// <summary>
    /// Starts the staggering effect from a specific value.
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public static StgOptionProp Start(double start) => new("Start", start);
    /// <summary>
    /// Starts the staggering effect from a specific value.
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public static StgOptionProp Start(string start) => new("Start", start);

    /// <summary>
    /// Staggering values based a 2D array that allow "ripple" effects.
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static StgOptionProp Grid(int rows, int columns) => new("Grid", new double[] { rows, columns });

    /// <summary>
    /// Stagger values using an easing function.
    /// </summary>
    /// <param name="easing"></param>
    /// <returns></returns>
    public static StgOptionProp Easing(Prop easing) => new("Easing", easing.GetValue());

    /// <summary>
    /// Forces the direction of a grid staggering effect.
    /// </summary>
    public abstract class Axis
    {
        /// <summary>
        /// Follows the x-axis
        /// </summary>
        public static StgOptionProp X { get; } = new("Axis", "x");
        
        /// <summary>
        /// Follows the y-axis
        /// </summary>
        public static StgOptionProp Y { get; } = new("Axis", "y");
    }

    /// <summary>
    /// Changes the order in which the stagger operates.
    /// </summary>
    public abstract class Direction
    {
        /// <summary>
        /// (Default) Normal staggering, from the first element to the last.
        /// </summary>
        public static StgOptionProp Normal { get; } = new("Direction", "normal");
        
        /// <summary>
        /// Reversed staggering, from the last element to the first.
        /// </summary>
        public static StgOptionProp Reverse { get; } = new("Direction", "reverse");
    }

    /// <summary>
    /// Starts the stagger effect from a specific position.
    /// </summary>
    public abstract class From
    {
        /// <summary>
        /// (Default) Start the effect from the first element.
        /// </summary>
        public static StgOptionProp First { get; } = new("From", "first");
        
        /// <summary>
        /// Start the effect from the last element.
        /// </summary>
        public static StgOptionProp Last { get; } = new("From", "last");
        
        /// <summary>
        /// Start the effect from the center.
        /// </summary>
        public static StgOptionProp Center { get; } = new("From", "center");
        
        /// <summary>
        /// Start the effect from the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static StgOptionProp Index(int index) => new("From", index);
    }
}