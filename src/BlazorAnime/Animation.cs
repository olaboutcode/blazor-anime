namespace BlazorAnime;

using Microsoft.JSInterop;

public class Animation(IJSObjectReference animationJsRef)
{
    /// <summary>
    /// Plays a paused animation, or starts the animation if the autoplay parameters is set to false.
    /// </summary>
    public async Task Play() =>
        await AnimationJsRef.InvokeVoidAsync("play");

    /// <summary>
    /// Pauses a running animation.
    /// </summary>
    public async Task Pause() =>
        await AnimationJsRef.InvokeVoidAsync("pause");
    
    /// <summary>
    /// Progresses a running animation.
    /// </summary>
    /// <param name="progress"></param>
    public async Task Progress(double progress) =>
        await AnimationJsRef.InvokeVoidAsync("progress", progress);

    /// <summary>
    /// Returns animation current progress.
    /// </summary>
    /// <returns>double</returns>
    public async Task<double> GetProgress() =>
        await AnimationJsRef.InvokeAsync<double>("getProgress");

    public async Task<bool> Began() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("hasBegun"));

    /// <summary>
    /// Animation has completed
    /// </summary>
    /// <returns>bool</returns>
    public async Task<bool> Completed() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("hasCompleted"));

    /// <summary>
    /// Animation started changing
    /// </summary>
    /// <returns>bool</returns>
    public async Task<bool> ChangeBegan() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("changeHasBegun"));

    /// <summary>
    /// Animation completed changing
    /// </summary>
    /// <returns>bool</returns>
    public async Task<bool> ChangeCompleted() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("changeHasCompleted"));

    /// <summary>
    /// Animation loop started
    /// </summary>
    /// <returns>bool</returns>
    public async Task<bool> LoopBegan() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("loopHasBegun"));

    /// <summary>
    /// Animation paused
    /// </summary>
    /// <returns>bool</returns>
    public async Task<bool> Paused() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("isPaused"));

    /// <summary>
    /// Animation reversed
    /// </summary>
    /// <returns>bool</returns>
    public async Task<bool> Reversed() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("isReversed"));
    
    /// <summary>
    /// Animation identification
    /// </summary>
    /// <returns>int</returns>
    public async Task<int> Id() =>
        await AnimationJsRef.InvokeAsync<int>("getId");

    /// <summary>
    /// Animation started changing
    /// </summary>
    /// <returns>bool</returns>
    public async Task<int> Loop() =>
        await AnimationJsRef.InvokeAsync<int>("getLoop");

    /// <summary>
    /// Restarts an animation from its initial values.
    /// </summary>
    public async Task Restart() =>
        await AnimationJsRef.InvokeVoidAsync("restart");

    /// <summary>
    /// Reverses the direction of an animation.
    /// </summary>
    public async Task Reverse() =>
        await AnimationJsRef.InvokeVoidAsync("reverse");

    /// <summary>
    /// Jump to a specific time (in milliseconds).
    /// <para>
    /// Can also be used to control an animation while scrolling.
    /// <code>animation.Seek((scrollPercent / 100) * animation.Duration);</code>
    /// </para>
    /// </summary>
    /// <param name="time"></param>
    public async Task Seek(double time) =>
        await AnimationJsRef.InvokeVoidAsync("seek", time);

    /// <summary>
    /// Removes targets from a running animation or timeline.
    /// </summary>
    /// <param name="targets"></param>
    public async Task Remove(string targets) =>
        await AnimationJsRef.InvokeVoidAsync("remove", targets);

    /// <summary>
    /// Returns the original value of an element.
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="propName"></param>
    /// <param name="cssUnit"></param>
    /// <returns></returns>
    public async Task<string> Get(string targets, string propName, string cssUnit) =>
        await AnimationJsRef.InvokeAsync<string>("get", targets, propName, cssUnit);

    /// <summary>
    /// Immediately sets values to the specified targets.
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="props"></param>
    public async Task Set(string targets, IEnumerable<Prop> props) =>
        await AnimationJsRef.InvokeVoidAsync("set", targets, props.ToObject());

    /// <summary>
    /// Returns a random integer within a specific range.
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    public async Task Random(double minValue, double maxValue) =>
        await AnimationJsRef.InvokeVoidAsync("random", minValue, maxValue);

    /// <summary>
    /// Plays an animation using an external requestAnimationFrame loop.
    /// </summary>
    /// <param name="time"></param>
    public async Task Tick(double time) =>
        await AnimationJsRef.InvokeVoidAsync("tick", time);

    /// <summary>
    /// Animation duration
    /// </summary>
    /// <returns>double</returns>
    public async Task<double> Duration() =>
        await AnimationJsRef.InvokeAsync<double>("getDuration");

    /// <summary>
    /// Animation delay
    /// </summary>
    /// <returns>double</returns>
    public async Task<double> Delay() =>
        await AnimationJsRef.InvokeAsync<double>("getDelay");

    /// <summary>
    /// Returns string representation of animation direction
    /// </summary>
    /// <returns>string</returns>
    public async Task<string> Direction()
    {
        return await AnimationJsRef.InvokeAsync<string>("getDirection");
    }

    public async ValueTask DisposeAsync()
    {
        await AnimationJsRef.DisposeAsync();
    }

    private IJSObjectReference AnimationJsRef { get; } = animationJsRef;

    private static bool ToBool(int val) => val == 1;
}