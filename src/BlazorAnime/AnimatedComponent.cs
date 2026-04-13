using Microsoft.AspNetCore.Components;

namespace BlazorAnime;

public abstract class AnimatedComponent: ComponentBase, IAsyncDisposable
{
    [Inject]private IAnime Anime { get; set; } = default!;
    private readonly Dictionary<string, Animation> _animations = [];
    private readonly Dictionary<string, Timeline> _timelines = [];
    
    protected IReadOnlyList<Animation> GetAnimations() =>
        [.. _animations.Values];
    protected Animation? GetAnimation(string id) => 
        _animations.TryGetValue(id, out var anim) ? anim : null;
    
    protected IReadOnlyList<Timeline> GetTimelines() =>
        [.._timelines.Values];
    protected Timeline? GetTimeline(string id) => 
        _timelines.TryGetValue(id, out var tl) ? tl : null;

    public async Task<Animation> CreateAnimationAsync(
        string id, 
        Func<PropsBuilder, PropsBuilder> configure)
    {
        var animation = await Anime.Animate(configure);
        _animations.Add(id, animation);
        return animation;
    }

    protected async Task<Timeline> CreateTimelineAsync(
        string id,
        Func<PropsBuilder, PropsBuilder> configure)
    {
        var timeline = await Anime.Timeline(configure);
        _timelines.Add(id, timeline);
        return timeline;
    }

    protected async Task<Animation> CreateAnimationAsync(string id, params Prop[] props)
    {
        var animation = await Anime.Animate(props);
        _animations.Add(id, animation);
        return animation;
    }

    protected async Task<Timeline> CreateTimelineAsync(string id, params Prop[] props)
    {
        var timeline = await Anime.Timeline(props);
        _timelines.Add(id, timeline);
        return timeline;
    }

    public virtual void OnBegin(AnimationState state) => StateHasChanged();
    public virtual void OnUpdate(AnimationState state) => StateHasChanged();
    public virtual void OnComplete(AnimationState state) => StateHasChanged();
    public virtual void OnLoopBegin(AnimationState state) => StateHasChanged();
    public virtual void OnLoopComplete(AnimationState state) => StateHasChanged();
    public virtual void OnChangeBegin(AnimationState state) => StateHasChanged();
    public virtual void OnChangeComplete(AnimationState state) => StateHasChanged();

    public async ValueTask DisposeAsync()
    {
        foreach (var animation in _animations)
        {
            await animation.Value.DisposeAsync();
        }
        foreach (var timeline in _timelines)
        {
            await timeline.Value.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}