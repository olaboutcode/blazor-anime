using BlazorAnime;
using Microsoft.AspNetCore.Components;

namespace Examples;

public abstract class WaapiDemo : ComponentBase, IAsyncDisposable
{
    [Inject] protected IWaapi Waapi { get; set; } = default!;

    [Inject] protected IAnime Anime { get; set; } = default!;

    [CascadingParameter(Name = "DemoPlaying")]
    public bool? Playing { get; set; }

    protected bool Live => Playing != false;

    private readonly List<WaapiAnimation> _animations = [];
    private readonly List<Animation> _timelines = [];
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
            await DisposeCreated();
        }
    }

    protected async Task<WaapiAnimation> Animate(string selector, Func<WaapiAnimationBuilder, WaapiAnimationBuilder> configure)
    {
        var animation = await Waapi.Animate(selector, configure);
        _animations.Add(animation);
        return animation;
    }

    protected async Task<Timeline> Timeline(Func<PropsBuilder, PropsBuilder> configure)
    {
        var timeline = await Anime.CreateTimeline(configure);
        _timelines.Add(timeline);
        return timeline;
    }

    protected abstract Task Start();

    public async ValueTask DisposeAsync() => await DisposeCreated();

    private async Task DisposeCreated()
    {
        var animations = _animations.ToArray();
        _animations.Clear();
        foreach (var animation in animations)
            await animation.DisposeAsync();

        var timelines = _timelines.ToArray();
        _timelines.Clear();
        foreach (var timeline in timelines)
            await timeline.DisposeAsync();
    }
}
