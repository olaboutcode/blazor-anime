using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class Timeline : Animation
{
    /// <summary>
    /// Add an animation that starts when the previous child ends.
    /// </summary>
    public async Task AddAsync(Func<PropsBuilder, PropsBuilder> configure) =>
        await AddCore(configure, offset: null);

    /// <summary>
    /// Add an animation at an absolute time, in milliseconds.
    /// </summary>
    public async Task AddAsync(Func<PropsBuilder, PropsBuilder> configure, double offSet) =>
        await AddCore(configure, offSet);

    /// <summary>
    /// Add an animation at a relative or absolute offset such as "+=500" or "-=200".
    /// </summary>
    public async Task AddAsync(Func<PropsBuilder, PropsBuilder> configure, string offSet) =>
        await AddCore(configure, offSet);

    /// <summary>
    /// Add an animation at a relative offset.
    /// </summary>
    public async Task AddAsync(Func<PropsBuilder, PropsBuilder> configure, OffSet offSet) =>
        await AddCore(configure, offSet.GetValue());

    internal Timeline(IJSObjectReference jsRef, IEnumerable<IDisposable> callbacks)
        : base(jsRef, callbacks)
    {
    }

    private async Task AddCore(Func<PropsBuilder, PropsBuilder> configure, object? offset)
    {
        var builder = configure(new PropsBuilder());
        Track(builder.CallbackHandles);
        var props = builder.Build().ToObject();
        if (offset is null)
            await Js.InvokeVoidAsync("add", props);
        else
            await Js.InvokeVoidAsync("add", props, offset);
    }
}

public sealed class OffSet
{
    /// <summary>
    /// Start this animation <paramref name="value"/> milliseconds before the previous one ends.
    /// </summary>
    public static OffSet Before(double value) => new(value, "-=");

    /// <summary>
    /// Start this animation <paramref name="value"/> milliseconds after the previous one ends.
    /// </summary>
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
