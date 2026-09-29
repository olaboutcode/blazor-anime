using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class AnimatableService(IJSRuntime jsRuntime) : IAnimatable
{
    public Task<Animatable> Create(string selector, Func<AnimatableBuilder, AnimatableBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        return CreateCore(selector, configure);
    }

    public Task<Animatable> Create(IReadOnlyList<string> selectors, Func<AnimatableBuilder, AnimatableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(selectors);
        if (selectors.Count == 0)
            throw new ArgumentException("At least one selector is required.", nameof(selectors));
        return CreateCore(selectors.ToArray(), configure);
    }

    public Task<Animatable> Create(ElementReference element, Func<AnimatableBuilder, AnimatableBuilder> configure) =>
        CreateCore(element, configure);

    public Task<Animatable> Create(IReadOnlyList<ElementReference> elements, Func<AnimatableBuilder, AnimatableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(elements);
        if (elements.Count == 0)
            throw new ArgumentException("At least one element is required.", nameof(elements));
        return CreateCore(elements.ToArray(), configure);
    }

    public Task<Animatable> Create(JsTarget target, Func<AnimatableBuilder, AnimatableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(target);
        return CreateCore(ObjectTarget(target.Reference), configure);
    }

    public Task<Animatable> Create(IReadOnlyList<JsTarget> targets, Func<AnimatableBuilder, AnimatableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(targets);
        if (targets.Count == 0)
            throw new ArgumentException("At least one target is required.", nameof(targets));
        return CreateCore(ObjectTarget(targets.Select(target => target.Reference).ToArray()), configure);
    }

    private async Task<Animatable> CreateCore(object targets, Func<AnimatableBuilder, AnimatableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new AnimatableBuilder();
        try
        {
            builder = configure(builder) ?? throw new ArgumentNullException(nameof(configure));
            if (!builder.HasProperties)
                throw new ArgumentException("An animatable needs at least one property.", nameof(configure));

            var reference = await Js.InvokeAsync<IJSObjectReference>(IdentifierCreate, targets, builder.Build());
            return new Animatable(reference, builder.Callbacks);
        }
        catch
        {
            foreach (var handle in builder.Callbacks)
                handle.Dispose();
            throw;
        }
    }

    private static Dictionary<string, object> ObjectTarget(object reference) => new()
    {
        ["propType"] = "objectTarget",
        ["value"] = reference
    };

    private IJSRuntime Js { get; } = jsRuntime;
    private const string IdentifierCreate = "AnimeJsAnimatable.create";
}

public interface IAnimatable
{
    Task<Animatable> Create(string selector, Func<AnimatableBuilder, AnimatableBuilder> configure);
    Task<Animatable> Create(IReadOnlyList<string> selectors, Func<AnimatableBuilder, AnimatableBuilder> configure);
    Task<Animatable> Create(ElementReference element, Func<AnimatableBuilder, AnimatableBuilder> configure);
    Task<Animatable> Create(IReadOnlyList<ElementReference> elements, Func<AnimatableBuilder, AnimatableBuilder> configure);
    Task<Animatable> Create(JsTarget target, Func<AnimatableBuilder, AnimatableBuilder> configure);
    Task<Animatable> Create(IReadOnlyList<JsTarget> targets, Func<AnimatableBuilder, AnimatableBuilder> configure);
}
