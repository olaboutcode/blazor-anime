using Microsoft.JSInterop;

namespace BlazorAnime;

public class Timeline(IJSObjectReference timelineJsRef)
{
    public async void Add(Props props) =>
        await TimelineJsRef.InvokeVoidAsync("add", props.ToObject());

    public async void Add(Props props, double offSet) =>
        await TimelineJsRef.InvokeVoidAsync("add", props.ToObject(), offSet);

    public async void Add(Props props, OffSet offSet) =>
        await TimelineJsRef.InvokeVoidAsync("add", props.ToObject(), offSet.GetValue());

    public async void Play() =>
        await TimelineJsRef.InvokeVoidAsync("play");

    public async void Pause() =>
        await TimelineJsRef.InvokeVoidAsync("pause");

    public async void Restart() =>
        await TimelineJsRef.InvokeVoidAsync("restart");

    public async void Reverse() =>
        await TimelineJsRef.InvokeVoidAsync("reverse");

    public async void Seek(double time) =>
        await TimelineJsRef.InvokeVoidAsync("seek", time);

    public async Task<double> GetProgress() =>
        await TimelineJsRef.InvokeAsync<double>("getProgress");

    public async Task<bool> Began() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("hasBegun"));

    public async Task<bool> Completed() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("hasCompleted"));

    public async Task<bool> ChangeBegan() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("changeHasBegun"));

    public async Task<bool> ChangeCompleted() =>
        ToBool(await TimelineJsRef.InvokeAsync<int>("changeHasCompleted"));

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
    public static OffSet Before(double value) => new(value, "-=");
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