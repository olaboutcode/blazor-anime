using BlazorAnime;
using Microsoft.AspNetCore.Components;

namespace Examples;

public abstract class EventsDemo : ComponentBase, IAsyncDisposable
{
    [Inject] protected IEvents Events { get; set; } = default!;

    [Inject] protected IAnime Anime { get; set; } = default!;

    [CascadingParameter(Name = "DemoPlaying")]
    public bool? Playing { get; set; }

    protected bool Live => Playing != false;

    private readonly List<ScrollObserver> _created = [];
    private readonly List<Animation> _animations = [];
    private bool _started;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Live && !_started)
        {
            _started = true;
            await Start();
            return;
        }

        if (!Live && _started)
        {
            _started = false;
            await DisposeObservers();
        }
    }

    protected async Task<ScrollObserver> Open(Func<ScrollObserverBuilder, ScrollObserverBuilder>? configure = null)
    {
        var observer = configure is null
            ? await Events.OnScroll()
            : await Events.OnScroll(configure);
        _created.Add(observer);
        return observer;
    }

    protected async Task<Animation> Animate(Func<PropsBuilder, PropsBuilder> configure)
    {
        var animation = await Anime.Animate(configure);
        _animations.Add(animation);
        return animation;
    }

    protected abstract Task Start();

    public async ValueTask DisposeAsync() => await DisposeObservers();

    private async Task DisposeObservers()
    {
        var observers = _created.ToArray();
        _created.Clear();
        foreach (var observer in observers)
            await observer.DisposeAsync();

        var animations = _animations.ToArray();
        _animations.Clear();
        foreach (var animation in animations)
            await animation.DisposeAsync();
    }
}
