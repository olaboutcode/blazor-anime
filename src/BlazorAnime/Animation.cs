namespace BlazorAnime;

using Microsoft.JSInterop;

public class Animation(IJSObjectReference animationJsRef)
{
    /// <summary>
    /// Plays a paused animation, or starts the animation if the autoplay parameters is set to false.
    /// </summary>
    public async void Play() =>
        await AnimationJsRef.InvokeVoidAsync("play");

    /// <summary>
    /// Pauses a running animation.
    /// </summary>
    public async void Pause() =>
        await AnimationJsRef.InvokeVoidAsync("pause");
    
    public async void Progress(double progress) =>
        await AnimationJsRef.InvokeVoidAsync("progress", progress);

    public async Task<double> GetProgress() =>
        await AnimationJsRef.InvokeAsync<double>("getProgress");

    public async Task<bool> Began() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("hasBegun"));

    public async Task<bool> Completed() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("hasCompleted"));

    public async Task<bool> ChangeBegan() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("changeHasBegun"));

    public async Task<bool> ChangeCompleted() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("changeHasCompleted"));

    public async Task<bool> LoopBegan() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("loopHasBegun"));

    public async Task<bool> Paused() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("isPaused"));

    public async Task<bool> Reversed() =>
        ToBool(await AnimationJsRef.InvokeAsync<int>("isReversed"));

    public async Task<int> Id() =>
        await AnimationJsRef.InvokeAsync<int>("getId");

    public async Task<int> Loop() =>
        await AnimationJsRef.InvokeAsync<int>("getLoop");

    /// <summary>
    /// Restarts an animation from its initial values.
    /// </summary>
    public async void Restart() =>
        await AnimationJsRef.InvokeVoidAsync("restart");

    /// <summary>
    /// Reverses the direction of an animation.
    /// </summary>
    public async void Reverse() =>
        await AnimationJsRef.InvokeVoidAsync("reverse");

    /// <summary>
    /// Jump to a specific time (in milliseconds).
    /// <para>
    /// Can also be used to control an animation while scrolling.
    /// <code>animation.Seek((scrollPercent / 100) * animation.Duration);</code>
    /// </para>
    /// </summary>
    /// <param name="time"></param>
    public async void Seek(double time) =>
        await AnimationJsRef.InvokeVoidAsync("seek", time);

    /// <summary>
    /// Removes targets from a running animation or timeline.
    /// </summary>
    /// <param name="targets"></param>
    public async void Remove(string targets) =>
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
    public async void Set(string targets, Props props) =>
        await AnimationJsRef.InvokeVoidAsync("set", targets, props.ToObject());

    /// <summary>
    /// Returns a random integer within a specific range.
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    public async void Random(double minValue, double maxValue) =>
        await AnimationJsRef.InvokeVoidAsync("random", minValue, maxValue);

    /// <summary>
    /// Plays an animation using an external requestAnimationFrame loop.
    /// </summary>
    /// <param name="time"></param>
    public async void Tick(double time) =>
        await AnimationJsRef.InvokeVoidAsync("tick", time);

    public async Task<double> Duration() =>
        await AnimationJsRef.InvokeAsync<double>("getDuration");

    public async Task<double> Delay() =>
        await AnimationJsRef.InvokeAsync<double>("getDelay");

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