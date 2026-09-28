namespace BlazorAnime;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public class Animation : IAsyncDisposable
{
    /// <summary>
    /// Plays a paused animation, or starts the animation if autoplay is false.
    /// </summary>
    public async Task Play() =>
        await Js.InvokeVoidAsync("play");

    /// <summary>
    /// Pauses a running animation.
    /// </summary>
    public async Task Pause() =>
        await Js.InvokeVoidAsync("pause");

    /// <summary>
    /// Restarts an animation from its initial values.
    /// </summary>
    public async Task Restart() =>
        await Js.InvokeVoidAsync("restart");

    /// <summary>
    /// Reverses the direction of an animation.
    /// </summary>
    public async Task Reverse() =>
        await Js.InvokeVoidAsync("reverse");

    /// <summary>
    /// Returns the animation to its initial values and pauses it.
    /// </summary>
    public async Task Reset() =>
        await Js.InvokeVoidAsync("reset");

    /// <summary>
    /// Jump to a specific time, in milliseconds.
    /// </summary>
    public async Task Seek(double time) =>
        await Js.InvokeVoidAsync("seek", time);

    /// <summary>
    /// Jump to a progress percentage between 0 and 100.
    /// </summary>
    public async Task Progress(double progress) =>
        await Js.InvokeVoidAsync("setProgress", progress);

    /// <summary>
    /// Seeks to the end of the animation.
    /// </summary>
    public async Task Complete() =>
        await Js.InvokeVoidAsync("finish");

    /// <summary>
    /// Resolves when the animation finishes. An infinite loop never resolves.
    /// </summary>
    public async Task Finished() =>
        await Js.InvokeAsync<object?>("whenFinished");

    /// <summary>
    /// Plays an animation using an external requestAnimationFrame timestamp.
    /// </summary>
    public async Task Tick(double time) =>
        await Js.InvokeVoidAsync("tick", time);

    /// <summary>
    /// Removes targets from this animation.
    /// </summary>
    public async Task Remove(string targets) =>
        await Js.InvokeVoidAsync("remove", targets);

    /// <summary>
    /// Removes an element from this animation.
    /// </summary>
    public async Task Remove(ElementReference target) =>
        await Js.InvokeVoidAsync("remove", target);

    /// <summary>
    /// Returns the original value of an element.
    /// </summary>
    public async Task<string> Get(string targets, string propName, string cssUnit) =>
        await Js.InvokeAsync<string>("get", targets, propName, cssUnit);

    /// <summary>
    /// Returns the original value of an element.
    /// </summary>
    public async Task<string> Get(ElementReference target, string propName) =>
        await Js.InvokeAsync<string>("get", target, propName, null);

    /// <summary>
    /// Immediately sets values on the specified targets.
    /// </summary>
    public async Task Set(string targets, Func<PropsBuilder, PropsBuilder> build) =>
        await Js.InvokeVoidAsync("set", targets, build(new PropsBuilder()).Build().ToObject());

    /// <summary>
    /// Immediately sets values on an element.
    /// </summary>
    public async Task Set(ElementReference target, Func<PropsBuilder, PropsBuilder> build) =>
        await Js.InvokeVoidAsync("set", target, build(new PropsBuilder()).Build().ToObject());

    /// <summary>
    /// Returns a random integer in the inclusive range.
    /// </summary>
    public async Task<int> Random(int minValue, int maxValue) =>
        await Js.InvokeAsync<int>("random", minValue, maxValue);

    public async Task<double> GetProgress() =>
        await Js.InvokeAsync<double>("getProgress");

    public async Task<bool> Began() =>
        ToBool(await Js.InvokeAsync<int>("hasBegun"));

    public async Task<bool> Completed() =>
        ToBool(await Js.InvokeAsync<int>("hasCompleted"));

    public async Task<bool> ChangeBegan() =>
        ToBool(await Js.InvokeAsync<int>("changeHasBegun"));

    public async Task<bool> ChangeCompleted() =>
        ToBool(await Js.InvokeAsync<int>("changeHasCompleted"));

    public async Task<bool> LoopBegan() =>
        ToBool(await Js.InvokeAsync<int>("loopHasBegun"));

    public async Task<bool> Paused() =>
        ToBool(await Js.InvokeAsync<int>("isPaused"));

    public async Task<bool> Reversed() =>
        ToBool(await Js.InvokeAsync<int>("isReversed"));

    public async Task<bool> ReversePlayback() =>
        await Js.InvokeAsync<bool>("isReversePlayback");

    public async Task<int> Id() =>
        await Js.InvokeAsync<int>("getId");

    /// <summary>
    /// Configured loop count. -1 means the animation loops forever.
    /// </summary>
    public async Task<int> Loop() =>
        await Js.InvokeAsync<int>("getLoop");

    /// <summary>
    /// Loops still left to play. -1 means the animation loops forever.
    /// </summary>
    public async Task<int> Remaining() =>
        await Js.InvokeAsync<int>("getRemaining");

    public async Task<double> Duration() =>
        await Js.InvokeAsync<double>("getDuration");

    public async Task<double> Delay() =>
        await Js.InvokeAsync<double>("getDelay");

    public async Task<double> EndDelay() =>
        await Js.InvokeAsync<double>("getEndDelay");

    public async Task<double> CurrentTime() =>
        await Js.InvokeAsync<double>("getCurrentTime");

    public async Task<string> Direction() =>
        await Js.InvokeAsync<string>("getDirection");

    public async Task<int> TargetCount() =>
        await Js.InvokeAsync<int>("getTargetCount");

    /// <summary>
    /// Current interpolated value of each animated property.
    /// </summary>
    public async Task<Dictionary<string, string>> CurrentValues() =>
        await Js.InvokeAsync<Dictionary<string, string>>("getCurrentValues");

    public async ValueTask DisposeAsync()
    {
        try
        {
            await Js.InvokeVoidAsync("pause");
        }
        catch (JSException)
        {
        }
        catch (JSDisconnectedException)
        {
        }
        catch (ObjectDisposedException)
        {
        }

        try
        {
            await Js.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }

        foreach (var handle in _callbacks)
            handle.Dispose();
    }

    internal Animation(IJSObjectReference jsRef, IEnumerable<IDisposable> callbacks)
    {
        Js = jsRef;
        _callbacks = [.. callbacks];
    }

    protected IJSObjectReference Js { get; }
    private readonly List<IDisposable> _callbacks;

    protected void Track(IEnumerable<IDisposable> callbacks) => _callbacks.AddRange(callbacks);

    private static bool ToBool(int val) => val == 1;
}
