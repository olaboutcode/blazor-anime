namespace BlazorAnime;

public static class TimelineExtensions
{
    /// <summary>
    /// Add Timeline animation props with relative offSet 
    /// </summary>
    /// <param name="configure"></param>
    public static async Task<Timeline> AddAsync(
        this Task<Timeline> task,
        Func<PropsBuilder, PropsBuilder> configure)
    {
        var timeline = await task;
        await timeline.AddAsync(configure);
        return timeline;
    }
    
    /// <summary>
    /// Add Timeline animation props with relative offSet 
    /// </summary>
    /// <param name="configure"></param>
    /// <param name="offSet"></param>
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
    /// Add Timeline animation props with relative offSet 
    /// </summary>
    /// <param name="configure"></param>
    /// <param name="offSet"></param>
    public static async Task<Timeline> AddAsync(
        this Task<Timeline> task,
        Func<PropsBuilder, PropsBuilder> configure,
        double offSet)
    {
        var timeline = await task;
        await timeline.AddAsync(configure, offSet);
        return timeline;
    }
}