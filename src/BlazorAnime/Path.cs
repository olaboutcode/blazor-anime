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
    /// <param name="svgPropertyName"></param>
    /// <returns>PathParam</returns>
    public async Task<PathParam> Get(string svgPropertyName)
    {
        var paramRef = await PathRef.InvokeAsync<IJSObjectReference>("path", svgPropertyName);
        return new PathParam(paramRef);
    }

    private IJSObjectReference PathRef { get; } = jsRef;
}