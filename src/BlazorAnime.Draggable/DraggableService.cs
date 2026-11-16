using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class DraggableService(IJSRuntime jsRuntime) : IDraggable
{
    public Task<Draggable> Create(string selector) =>
        Create(selector, static builder => builder);

    public Task<Draggable> Create(string selector, Func<DraggableBuilder, DraggableBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        return CreateCore(selector, configure);
    }

    public Task<Draggable> Create(IReadOnlyList<string> selectors) =>
        Create(selectors, static builder => builder);

    public Task<Draggable> Create(IReadOnlyList<string> selectors, Func<DraggableBuilder, DraggableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(selectors);
        if (selectors.Count == 0)
            throw new ArgumentException("At least one selector is required.", nameof(selectors));
        return CreateCore(selectors.ToArray(), configure);
    }

    public Task<Draggable> Create(ElementReference element) =>
        Create(element, static builder => builder);

    public Task<Draggable> Create(ElementReference element, Func<DraggableBuilder, DraggableBuilder> configure) =>
        CreateCore(element, configure);

    public Task<Draggable> Create(IReadOnlyList<ElementReference> elements) =>
        Create(elements, static builder => builder);

    public Task<Draggable> Create(IReadOnlyList<ElementReference> elements, Func<DraggableBuilder, DraggableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(elements);
        if (elements.Count == 0)
            throw new ArgumentException("At least one element is required.", nameof(elements));
        return CreateCore(elements.ToArray(), configure);
    }

    public Task<Draggable> Create(JsTarget target) =>
        Create(target, static builder => builder);

    public Task<Draggable> Create(JsTarget target, Func<DraggableBuilder, DraggableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(target);
        return CreateCore(ObjectTarget(target.Reference), configure);
    }

    public Task<Draggable> Create(IReadOnlyList<JsTarget> targets) =>
        Create(targets, static builder => builder);

    public Task<Draggable> Create(IReadOnlyList<JsTarget> targets, Func<DraggableBuilder, DraggableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(targets);
        if (targets.Count == 0)
            throw new ArgumentException("At least one target is required.", nameof(targets));
        return CreateCore(ObjectTarget(targets.Select(target => target.Reference).ToArray()), configure);
    }

    private async Task<Draggable> CreateCore(object targets, Func<DraggableBuilder, DraggableBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new DraggableBuilder();
        try
        {
            builder = configure(builder) ?? throw new ArgumentNullException(nameof(configure));
            var reference = await Js.InvokeAsync<IJSObjectReference>(IdentifierCreate, targets, builder.Build());
            return new Draggable(reference, builder.Callbacks);
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
    private const string IdentifierCreate = "AnimeJsDraggable.create";
}

public interface IDraggable
{
    Task<Draggable> Create(string selector);
    Task<Draggable> Create(string selector, Func<DraggableBuilder, DraggableBuilder> configure);
    Task<Draggable> Create(IReadOnlyList<string> selectors);
    Task<Draggable> Create(IReadOnlyList<string> selectors, Func<DraggableBuilder, DraggableBuilder> configure);
    Task<Draggable> Create(ElementReference element);
    Task<Draggable> Create(ElementReference element, Func<DraggableBuilder, DraggableBuilder> configure);
    Task<Draggable> Create(IReadOnlyList<ElementReference> elements);
    Task<Draggable> Create(IReadOnlyList<ElementReference> elements, Func<DraggableBuilder, DraggableBuilder> configure);
    Task<Draggable> Create(JsTarget target);
    Task<Draggable> Create(JsTarget target, Func<DraggableBuilder, DraggableBuilder> configure);
    Task<Draggable> Create(IReadOnlyList<JsTarget> targets);
    Task<Draggable> Create(IReadOnlyList<JsTarget> targets, Func<DraggableBuilder, DraggableBuilder> configure);
}
