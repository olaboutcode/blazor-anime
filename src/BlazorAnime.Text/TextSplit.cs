using Microsoft.JSInterop;

namespace BlazorAnime;

/// <summary>
/// One <c>splitText</c> result. <see cref="Lines"/>, <see cref="Words"/>, and <see cref="Chars"/>
/// are element handles for <c>Targets</c>.
/// </summary>
public sealed class TextSplit : IAsyncDisposable
{
    public IReadOnlyList<IJSObjectReference> Lines { get; private set; }
    public IReadOnlyList<IJSObjectReference> Words { get; private set; }
    public IReadOnlyList<IJSObjectReference> Chars { get; private set; }

    public async Task Refresh()
    {
        await Js.InvokeVoidAsync("refresh");
        var lines = await Read("lines");
        var words = await Read("words");
        var chars = await Read("chars");
        var previous = Lines.Concat(Words).Concat(Chars).ToArray();
        Lines = lines;
        Words = words;
        Chars = chars;
        await DisposeElements(previous);
    }

    public Task Revert() => Js.InvokeVoidAsync("revert").AsTask();

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        _disposed = true;
        try
        {
            await Js.InvokeVoidAsync("revert");
        }
        catch (JSException)
        {
        }
        catch (JSDisconnectedException)
        {
        }
        catch (ObjectDisposedException)
        {
        }

        await DisposeElements(Lines.Concat(Words).Concat(Chars).ToArray());
        Lines = [];
        Words = [];
        Chars = [];
        try
        {
            await Js.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }

    internal TextSplit(
        IJSObjectReference js,
        IReadOnlyList<IJSObjectReference> lines,
        IReadOnlyList<IJSObjectReference> words,
        IReadOnlyList<IJSObjectReference> chars)
    {
        Js = js;
        Lines = lines;
        Words = words;
        Chars = chars;
    }

    private async Task<IReadOnlyList<IJSObjectReference>> Read(string name)
    {
        var elements = await Js.InvokeAsync<IJSObjectReference[]>(name);
        return elements ?? [];
    }

    private static async Task DisposeElements(IReadOnlyList<IJSObjectReference> elements)
    {
        foreach (var element in elements)
        {
            try
            {
                await element.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }

    private readonly IJSObjectReference Js;
    private bool _disposed;
}
