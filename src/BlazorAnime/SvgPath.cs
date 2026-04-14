using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class SvgPathParam(IJSObjectReference paramRef)
{
    internal IJSObjectReference ParamRef { get; } = paramRef;
}

public sealed class SvgPath
{
    internal SvgPath(IJSObjectReference jsRef)
    {
        PathRef = jsRef;
    }

    /// <summary>
    /// Returns the current value of the SVG property
    /// </summary>
    /// <param name="svgPropertyName"></param>
    /// <returns>PathParam</returns>
    public async Task<SvgPathParam> Get(string svgPropertyName)
    {
        var paramRef = await PathRef.InvokeAsync<IJSObjectReference>("path", svgPropertyName);
        return new SvgPathParam(paramRef);
    }

    private IJSObjectReference PathRef { get; init; }
}