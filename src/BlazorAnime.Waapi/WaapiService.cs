using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class WaapiService(IJSRuntime jsRuntime) : IWaapi
{
    public Task<WaapiAnimation> Animate(string selector) =>
        Animate(selector, static builder => builder);

    public Task<WaapiAnimation> Animate(string selector, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        return Create(selector, configure);
    }

    public Task<WaapiAnimation> Animate(IReadOnlyList<string> selectors, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(selectors);
        if (selectors.Count == 0)
            throw new ArgumentException("At least one selector is required.", nameof(selectors));
        return Create(selectors.ToArray(), configure);
    }

    public Task<WaapiAnimation> Animate(ElementReference element) =>
        Animate(element, static builder => builder);

    public Task<WaapiAnimation> Animate(ElementReference element, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure) =>
        Create(element, configure);

    public Task<WaapiAnimation> Animate(IReadOnlyList<ElementReference> elements, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(elements);
        if (elements.Count == 0)
            throw new ArgumentException("At least one element is required.", nameof(elements));
        return Create(elements.ToArray(), configure);
    }

    public Task<string> ConvertEase(Easing easing)
    {
        ArgumentNullException.ThrowIfNull(easing);
        return Js.InvokeAsync<string>(IdentifierConvert, WaapiAnimationBuilder.EaseMarker(easing)).AsTask();
    }

    private async Task<WaapiAnimation> Create(object targets, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new WaapiAnimationBuilder();
        try
        {
            builder = configure(builder) ?? throw new ArgumentNullException(nameof(configure));
            var reference = await Js.InvokeAsync<IJSObjectReference>(IdentifierCreate, targets, builder.Build());
            var handle = await reference.InvokeAsync<IJSObjectReference>("handle");
            return new WaapiAnimation(reference, handle, builder.Callbacks);
        }
        catch
        {
            foreach (var callback in builder.Callbacks)
                callback.Dispose();
            throw;
        }
    }

    private IJSRuntime Js { get; } = jsRuntime;
    private const string IdentifierCreate = "AnimeJsWaapi.create";
    private const string IdentifierConvert = "AnimeJsWaapi.convertEase";
}

public interface IWaapi
{
    Task<WaapiAnimation> Animate(string selector);
    Task<WaapiAnimation> Animate(string selector, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure);
    Task<WaapiAnimation> Animate(IReadOnlyList<string> selectors, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure);
    Task<WaapiAnimation> Animate(ElementReference element);
    Task<WaapiAnimation> Animate(ElementReference element, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure);
    Task<WaapiAnimation> Animate(IReadOnlyList<ElementReference> elements, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure);

    /// <summary>Calls <c>waapi.convertEase</c> and returns the CSS <c>linear()</c> string. It does not create an animation.</summary>
    Task<string> ConvertEase(Easing easing);
}
