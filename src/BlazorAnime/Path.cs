using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class PathParam(IJSObjectReference paramRef)
{
    internal IJSObjectReference ParamRef { get; } = paramRef;
}

public sealed class Path
{
    internal Path(IJSObjectReference jsRef)
    {
        PathRef = jsRef;
    }

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

    private IJSObjectReference PathRef { get; init; }
}