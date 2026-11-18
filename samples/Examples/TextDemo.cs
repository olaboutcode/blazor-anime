using BlazorAnime;
using Microsoft.AspNetCore.Components;

namespace Examples;

public abstract class TextDemo : ComponentBase, IAsyncDisposable
{
    [Inject] protected IText Text { get; set; } = default!;

    [Inject] protected IAnime Anime { get; set; } = default!;

    [CascadingParameter(Name = "DemoPlaying")]
    public bool? Playing { get; set; }

    protected bool Live => Playing != false;

    private readonly List<TextSplit> _splits = [];
    private readonly List<TextScramble> _scrambles = [];
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
            await DisposeCreated();
        }
    }

    protected async Task<TextSplit> Split(string selector, Func<TextSplitBuilder, TextSplitBuilder> configure)
    {
        var split = await Text.Split(selector, configure);
        _splits.Add(split);
        return split;
    }

    protected async Task<TextScramble> Scramble(Func<TextScrambleBuilder, TextScrambleBuilder> configure)
    {
        var scramble = await Text.Scramble(configure);
        _scrambles.Add(scramble);
        return scramble;
    }

    protected async Task<Animation> Animate(Func<PropsBuilder, PropsBuilder> configure)
    {
        var animation = await Anime.Animate(configure);
        _animations.Add(animation);
        return animation;
    }

    protected abstract Task Start();

    public async ValueTask DisposeAsync() => await DisposeCreated();

    private async Task DisposeCreated()
    {
        var animations = _animations.ToArray();
        _animations.Clear();
        foreach (var animation in animations)
            await animation.DisposeAsync();

        var scrambles = _scrambles.ToArray();
        _scrambles.Clear();
        foreach (var scramble in scrambles)
            await scramble.DisposeAsync();

        var splits = _splits.ToArray();
        _splits.Clear();
        foreach (var split in splits)
            await split.DisposeAsync();
    }
}
