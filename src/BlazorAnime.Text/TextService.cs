using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class TextService(IJSRuntime jsRuntime) : IText
{
    public Task<TextSplit> Split(string selector) =>
        Split(selector, static builder => builder);

    public Task<TextSplit> Split(string selector, Func<TextSplitBuilder, TextSplitBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        return Create(selector, configure);
    }

    public Task<TextSplit> Split(ElementReference element) =>
        Split(element, static builder => builder);

    public Task<TextSplit> Split(ElementReference element, Func<TextSplitBuilder, TextSplitBuilder> configure) =>
        Create(element, configure);

    public Task<TextScramble> Scramble() => Scramble(static builder => builder);

    public async Task<TextScramble> Scramble(Func<TextScrambleBuilder, TextScrambleBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new TextScrambleBuilder();
        try
        {
            builder = configure(builder) ?? throw new ArgumentNullException(nameof(configure));
            var reference = await Js.InvokeAsync<IJSObjectReference>(IdentifierScramble, builder.Build());
            return new TextScramble(reference, builder.Callbacks);
        }
        catch
        {
            foreach (var callback in builder.Callbacks)
                callback.Dispose();
            throw;
        }
    }

    private async Task<TextSplit> Create(object target, Func<TextSplitBuilder, TextSplitBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(new TextSplitBuilder()) ?? throw new ArgumentNullException(nameof(configure));
        var reference = await Js.InvokeAsync<IJSObjectReference>(IdentifierCreate, target, builder.Build());
        try
        {
            var lines = await reference.InvokeAsync<IJSObjectReference[]>("lines") ?? [];
            var words = await reference.InvokeAsync<IJSObjectReference[]>("words") ?? [];
            var chars = await reference.InvokeAsync<IJSObjectReference[]>("chars") ?? [];
            return new TextSplit(reference, lines, words, chars);
        }
        catch
        {
            try
            {
                await reference.InvokeVoidAsync("revert");
            }
            catch (JSException)
            {
            }

            await reference.DisposeAsync();
            throw;
        }
    }

    private IJSRuntime Js { get; } = jsRuntime;
    private const string IdentifierCreate = "AnimeJsText.create";
    private const string IdentifierScramble = "AnimeJsText.scramble";
}

public interface IText
{
    Task<TextSplit> Split(string selector);
    Task<TextSplit> Split(string selector, Func<TextSplitBuilder, TextSplitBuilder> configure);
    Task<TextSplit> Split(ElementReference element);
    Task<TextSplit> Split(ElementReference element, Func<TextSplitBuilder, TextSplitBuilder> configure);

    /// <summary>Calls <c>scrambleText</c>. The result is a function for <c>innerHTML</c>, not an animation.</summary>
    Task<TextScramble> Scramble();
    Task<TextScramble> Scramble(Func<TextScrambleBuilder, TextScrambleBuilder> configure);
}
