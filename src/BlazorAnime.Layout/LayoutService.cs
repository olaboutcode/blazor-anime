using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class LayoutService(IJSRuntime jsRuntime) : ILayout
{
    public Task<Layout> Create(string selector) =>
        Create(selector, static builder => builder);

    public Task<Layout> Create(string selector, Func<LayoutBuilder, LayoutBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        return CreateCore(selector, configure);
    }

    public Task<Layout> Create(ElementReference element) =>
        Create(element, static builder => builder);

    public Task<Layout> Create(ElementReference element, Func<LayoutBuilder, LayoutBuilder> configure) =>
        CreateCore(element, configure);

    private async Task<Layout> CreateCore(object root, Func<LayoutBuilder, LayoutBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new LayoutBuilder();
        try
        {
            builder = configure(builder) ?? throw new ArgumentNullException(nameof(configure));
            var reference = await Js.InvokeAsync<IJSObjectReference>(IdentifierCreate, root, builder.Build());
            return new Layout(reference, builder.Callbacks);
        }
        catch
        {
            foreach (var handle in builder.Callbacks)
                handle.Dispose();
            throw;
        }
    }

    private IJSRuntime Js { get; } = jsRuntime;
    private const string IdentifierCreate = "AnimeJsLayout.create";
}

public interface ILayout
{
    Task<Layout> Create(string selector);
    Task<Layout> Create(string selector, Func<LayoutBuilder, LayoutBuilder> configure);
    Task<Layout> Create(ElementReference element);
    Task<Layout> Create(ElementReference element, Func<LayoutBuilder, LayoutBuilder> configure);
}
