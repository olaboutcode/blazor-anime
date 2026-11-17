using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class EventsService(IJSRuntime jsRuntime) : IEvents
{
    public Task<ScrollObserver> OnScroll() => OnScroll(static builder => builder);

    public async Task<ScrollObserver> OnScroll(Func<ScrollObserverBuilder, ScrollObserverBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(new ScrollObserverBuilder()) ?? throw new ArgumentNullException(nameof(configure));
        var reference = await Js.InvokeAsync<IJSObjectReference>(
            IdentifierCreate,
            builder.HasContainer ? builder.ContainerValue : null,
            builder.HasTarget ? builder.TargetValue : null,
            builder.Settings);
        var handle = await reference.InvokeAsync<IJSObjectReference>("handle");
        return new ScrollObserver(reference, handle, builder.Callbacks);
    }

    private IJSRuntime Js { get; } = jsRuntime;
    private const string IdentifierCreate = "AnimeJsEvents.create";
}

public interface IEvents
{
    Task<ScrollObserver> OnScroll();
    Task<ScrollObserver> OnScroll(Func<ScrollObserverBuilder, ScrollObserverBuilder> configure);
}
