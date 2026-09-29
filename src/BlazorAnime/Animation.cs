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
    /// Plays the animation backward from its current time.
    /// </summary>
    public async Task Reverse() =>
        await Js.InvokeVoidAsync("reverse");

    /// <summary>Continues playback in the current direction.</summary>
    public async Task Resume() =>
        await Js.InvokeVoidAsync("resume");

    /// <summary>Mirrors the current time and flips <see cref="Reversed"/>.</summary>
    public async Task Alternate() =>
        await Js.InvokeVoidAsync("alternate");

    public async Task Cancel() =>
        await Js.InvokeVoidAsync("cancel");

    public async Task Revert() =>
        await Js.InvokeVoidAsync("revert");

    public async Task Stretch(double duration) =>
        await Js.InvokeVoidAsync("stretch", duration);

    /// <summary>
    /// Re-reads function values, then refreshes the animation from those values.
    /// </summary>
    public async Task Refresh() =>
        await Js.InvokeVoidAsync("refresh");

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
    /// Jump to a progress value between 0 and 1.
    /// </summary>
    public async Task Progress(double progress) =>
        await Js.InvokeVoidAsync("setProgress", progress);

    /// <summary>
    /// Seeks to the end and removes the instance from the engine.
    /// </summary>
    public async Task Complete() =>
        await Js.InvokeVoidAsync("complete");

    /// <summary>
    /// Resolves when the animation finishes. An infinite loop never resolves.
    /// </summary>
    public async Task Finished() =>
        await Js.InvokeAsync<object?>("whenFinished");

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

    public async Task<double> GetProgress() =>
        await Js.InvokeAsync<double>("getProgress");

    public async Task<bool> Began() =>
        ToBool(await Js.InvokeAsync<int>("hasBegun"));

    public async Task<bool> Completed() =>
        ToBool(await Js.InvokeAsync<int>("hasCompleted"));

    public async Task<bool> Paused() =>
        ToBool(await Js.InvokeAsync<int>("isPaused"));

    public async Task<bool> Reversed() =>
        ToBool(await Js.InvokeAsync<int>("isReversed"));

    public async Task<bool> Backwards() =>
        ToBool(await Js.InvokeAsync<int>("isBackwards"));

    public async Task<bool> IsAlternate() =>
        ToBool(await Js.InvokeAsync<int>("isAlternate"));

    public async Task<string> Id() =>
        await Js.InvokeAsync<string>("getId");

    /// <summary>
    /// Configured loop count. -1 means the animation loops forever.
    /// </summary>
    public async Task<int> Loop() =>
        await Js.InvokeAsync<int>("getLoop");

    public async Task<double> Duration() =>
        await Js.InvokeAsync<double>("getDuration");

    public async Task<double> Delay() =>
        await Js.InvokeAsync<double>("getDelay");

    public async Task<double> LoopDelay() =>
        await Js.InvokeAsync<double>("getLoopDelay");

    public async Task<double> CurrentTime() =>
        await Js.InvokeAsync<double>("getCurrentTime");

    public async Task<double> IterationProgress() =>
        await Js.InvokeAsync<double>("getIterationProgress");

    public async Task<int> CurrentIteration() =>
        await Js.InvokeAsync<int>("getCurrentIteration");

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
