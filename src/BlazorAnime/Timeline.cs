using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class Timeline(IJSObjectReference timelineJsRef): IAsyncDisposable
{
    /// <summary>
    /// Add Timeline animation props
    /// </summary>
    /// <param name="props"></param>
    public async Task Add(params Prop[] props) =>
        await TimelineJsRef.InvokeVoidAsync("add", props.ToObject());

    /// <summary>
    /// Starts at `offSet` milliseconds, regardless of the animation position in the timeline
    /// </summary>
    /// <param name="props"></param>
    /// <param name="offSet"></param>
    public async Task Add(Prop[] props, double offSet) =>
        await TimelineJsRef.InvokeVoidAsync("add", props.ToObject(), offSet);

    /// <summary>
    /// Add Timeline animation props with relative offSet 
    /// </summary>
    /// <param name="props"></param>
    /// <param name="offSet"></param>
    public async Task Add(Prop[] props, OffSet offSet) =>
        await TimelineJsRef.InvokeVoidAsync("add", props.ToObject(), offSet.GetValue());

    /// <summary>
    /// Play Timeline animation
    /// </summary>
    public async Task Play() =>
        await TimelineJsRef.InvokeVoidAsync("play");

    /// <summary>
    /// Pause Timeline animation
    /// </summary>
    public async Task Pause() =>
        await TimelineJsRef.InvokeVoidAsync("pause");

    /// <summary>
    /// Restart Timeline animation
    /// </summary>
    public async Task Restart() =>
        await TimelineJsRef.InvokeVoidAsync("restart");

    /// <summary>
    /// Reverse Timeline animation
    /// </summary>
    public async Task Reverse() =>
        await TimelineJsRef.InvokeVoidAsync("reverse");

    /// <summary>
    /// Jump to specific time on the Timeline
    /// </summary>
    /// <param name="time"></param>
    public async Task Seek(double time) =>
        await TimelineJsRef.InvokeVoidAsync("seek", time);

    /// <summary>
    /// Get Timeline progress
    /// </summary>
    /// <returns>double</returns>
    public async Task<double> GetProgress() =>
        await TimelineJsRef.InvokeAsync<double>("getProgress");

    /// <summary>
    /// Timeline started animating
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Began() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("hasBegun"));

    /// <summary>
    /// Timeline completed animating
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Completed() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("hasCompleted"));

    /// <summary>
    /// Timeline animation change started
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ChangeBegan() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("changeHasBegun"));

    /// <summary>
    /// Timeline animation change completed
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ChangeCompleted() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("changeHasCompleted"));

    /// <summary>
    /// Timeline animation loop started
    /// </summary>
    /// <returns></returns>
    public async Task<bool> LoopBegan() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("loopHasBegun"));

    public async ValueTask DisposeAsync()
    {
        await TimelineJsRef.DisposeAsync();
    }

    private IJSObjectReference TimelineJsRef { get; } = timelineJsRef;

    private static bool ToBool(int val) => val == 1;
}

public class OffSet
{
    /// <summary>
    /// Start animation before `value` milliseconds before the previous animation ends
    /// </summary>
    /// <param name="value"></param>
    /// <returns>OffSet</returns>
    public static OffSet Before(double value) => new(value, "-=");
    
    /// <summary>
    /// Start animation after `value` milliseconds after the previous animation ends
    /// </summary>
    /// <param name="value"></param>
    /// <returns>OffSet</returns>
    public static OffSet After(double value) => new(value, "+=");

    private OffSet(double value, string offSet)
    {
        Value = value;
        Offset = offSet;
    }

    private double Value { get; }
    
    private string Offset { get; }
    
    public string GetValue() => $"{Offset}{Value}";
}