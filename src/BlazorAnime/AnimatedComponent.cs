using Microsoft.AspNetCore.Components;

namespace BlazorAnime;

public abstract class AnimatedComponent: ComponentBase, IAsyncDisposable
{
    [Inject]private IAnime Anime { get; set; } = default!;
    private readonly Dictionary<string, Animation> _animations = [];
    
    protected IReadOnlyList<Animation> GetAnimations() =>
        [.. _animations.Values];
    protected Animation? GetAnimation(string id) => 
        _animations.TryGetValue(id, out var anim) ? anim : null;

    protected async Task<Animation> CreateAnimationAsync(string id, params Prop[] props)
    {
         var animation = await Anime.Animate(props);
         _animations.Add(id, animation);
         return animation;
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
        GC.SuppressFinalize(this);
    }
}