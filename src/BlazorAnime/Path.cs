using Microsoft.JSInterop;

namespace BlazorAnime;

public class PathParm(IJSObjectReference paramRef)
{
    public IJSObjectReference ParamRef { get; } = paramRef;
}

public class Path(IJSObjectReference jsRef)
{
    /// <summary>
    /// Returns the current value of the SVG property
    /// </summary>
    /// <param name="svgPropName"></param>
    /// <returns></returns>
    public async Task<PathParm> Get(string svgPropName)
    {
        var paramRef = await PathRef.InvokeAsync<IJSObjectReference>("path", svgPropName);
        return new PathParm(paramRef);
    }

    private IJSObjectReference PathRef { get; } = jsRef;
}