using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// Functions from <c>createMotionPath</c> for <c>translateX</c>, <c>translateY</c>, and <c>rotate</c>.
/// Pass them to the matching transform. Offset is chosen when the path is created.
/// </summary>
public sealed class MotionPath : IAsyncDisposable
{
    internal MotionPath(
        IJSObjectReference holder,
        IJSObjectReference translateX,
        IJSObjectReference translateY,
        IJSObjectReference rotate)
    {
        Holder = holder;
        TranslateX = translateX;
        TranslateY = translateY;
        Rotate = rotate;
    }

    public IJSObjectReference TranslateX { get; }

    public IJSObjectReference TranslateY { get; }

    public IJSObjectReference Rotate { get; }

    public async ValueTask DisposeAsync()
    {
        await TranslateX.DisposeAsync();
        await TranslateY.DisposeAsync();
        await Rotate.DisposeAsync();
        await Holder.DisposeAsync();
    }

    private IJSObjectReference Holder { get; }
}
