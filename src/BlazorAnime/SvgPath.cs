using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class SvgPathParam(IJSObjectReference paramRef)
{
    internal IJSObjectReference ParamRef { get; } = paramRef;
}

/// <summary>
/// Wrapper around <c>anime.path()</c>. Call <see cref="Get"/> with "x", "y", or "angle"
/// and pass the result to a transform property.
/// </summary>
public sealed class SvgPath : IAsyncDisposable
{
    private readonly List<IJSObjectReference> _params = [];

    internal SvgPath(IJSObjectReference jsRef)
    {
        PathRef = jsRef;
    }

    /// <summary>
    /// Returns the motion-path value for an SVG property: "x", "y", or "angle".
    /// </summary>
    public async Task<SvgPathParam> Get(string svgPropertyName)
    {
        var paramRef = await PathRef.InvokeAsync<IJSObjectReference>("path", svgPropertyName);
        _params.Add(paramRef);
        return new SvgPathParam(paramRef);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var paramRef in _params)
            await paramRef.DisposeAsync();
        await PathRef.DisposeAsync();
    }

    private IJSObjectReference PathRef { get; init; }
}
