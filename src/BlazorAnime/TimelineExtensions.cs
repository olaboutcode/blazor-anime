namespace BlazorAnime;

public static class TimelineExtensions
{
    /// <summary>
    /// Add a child at the end of the timeline.
    /// </summary>
    public static async Task<Timeline> AddAsync(
        this Task<Timeline> task,
        Func<PropsBuilder, PropsBuilder> configure)
    {
        var timeline = await task;
        await timeline.AddAsync(configure);
        return timeline;
    }
    
    /// <summary>
    /// Add a child relative to the end of the previous child.
    /// </summary>
    public static async Task<Timeline> AddAsync(
        this Task<Timeline> task,
        Func<PropsBuilder, PropsBuilder> configure,
        OffSet offSet)
    {
        var timeline = await task;
        await timeline.AddAsync(configure, offSet);
        return timeline;
    }

    /// <summary>
    /// Add a child at an absolute time, in milliseconds.
    /// </summary>
    public static async Task<Timeline> AddAsync(
        this Task<Timeline> task,
        Func<PropsBuilder, PropsBuilder> configure,
        double offSet)
    {
        var timeline = await task;
        await timeline.AddAsync(configure, offSet);
        return timeline;
    }

    /// <summary>
    /// Add a child at a position such as "+=500", "&lt;", or "&lt;&lt;".
    /// </summary>
    public static async Task<Timeline> AddAsync(
        this Task<Timeline> task,
        Func<PropsBuilder, PropsBuilder> configure,
        string offSet)
    {
        var timeline = await task;
        await timeline.AddAsync(configure, offSet);
        return timeline;
    }
}