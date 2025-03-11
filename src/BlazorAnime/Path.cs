using Microsoft.JSInterop;

namespace BlazorAnime;

public class PathParam(IJSObjectReference paramRef)
{
    public IJSObjectReference ParamRef { get; } = paramRef;
}

public class Path(IJSObjectReference jsRef)
{
    /// <summary>
    /// Returns the current value of the SVG property
    /// </summary>
    /// <param name="svgPropName"></param>
    /// <returns>PathParam</returns>
    public async Task<PathParam> Get(string svgPropName)
    {
        var paramRef = await PathRef.InvokeAsync<IJSObjectReference>("path", svgPropName);
        return new PathParam(paramRef);
    }

    private IJSObjectReference PathRef { get; } = jsRef;
}