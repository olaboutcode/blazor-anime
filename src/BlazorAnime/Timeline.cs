using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// Plays child animations on one clock.
/// JavaScript <c>add</c> is <c>add(targets, parameters, position)</c>.
/// </summary>
public sealed class Timeline : Animation
{
    /// <summary>
    /// Add a child at the end of the timeline.
    /// </summary>
    public async Task AddAsync(Func<PropsBuilder, PropsBuilder> configure) =>
        await AddCore(configure, offset: null);

    /// <summary>
    /// Add a child at an absolute time, in milliseconds. <c>0</c> starts with the timeline.
    /// </summary>
    public async Task AddAsync(Func<PropsBuilder, PropsBuilder> configure, double offSet) =>
        await AddCore(configure, offSet);

    /// <summary>
    /// Add a child at a position such as <c>+=500</c>, <c>-=200</c>, <c>&lt;</c>, or <c>&lt;&lt;</c>.
    /// </summary>
    public async Task AddAsync(Func<PropsBuilder, PropsBuilder> configure, string offSet) =>
        await AddCore(configure, offSet);

    /// <summary>
    /// Add a child relative to the end of the previous child.
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
