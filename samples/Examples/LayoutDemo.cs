using BlazorAnime;
using Microsoft.AspNetCore.Components;

namespace Examples;

public abstract class LayoutDemo : ComponentBase, IAsyncDisposable
{
    [Inject] protected ILayout Layouts { get; set; } = default!;

    [CascadingParameter(Name = "DemoPlaying")]
    public bool? Playing { get; set; }

    protected bool Live => Playing != false;

    protected BlazorAnime.Layout? Instance { get; private set; }

    protected async Task<BlazorAnime.Layout> Create(string selector, Func<LayoutBuilder, LayoutBuilder>? configure = null)
    {
        var layout = configure is null
            ? await Layouts.Create(selector)
            : await Layouts.Create(selector, configure);
        _created.Add(layout);
        Instance = layout;
        return layout;
    }

    protected Task Prepare(Action change, Func<LayoutBuilder, LayoutBuilder>? animate = null)
    {
        if (Instance is null)
            return Task.CompletedTask;
        return Prepare(Instance, change, animate);
    }

    protected async Task Prepare(BlazorAnime.Layout layout, Action change, Func<LayoutBuilder, LayoutBuilder>? animate = null)
    {
        if (!Live)
            return;
        await layout.Record();
        change();
        _target = layout;
        _animate = animate;
        _pending = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await OnFirstRender();
            StateHasChanged();
            return;
        }

        var layout = _target ?? Instance;
        if (!_pending || layout is null)
            return;
        _pending = false;
        _target = null;
        var animate = _animate;
        _animate = null;
        if (animate is null)
            await layout.Animate();
        else
            await layout.Animate(animate);
    }

    protected abstract Task OnFirstRender();

    public async ValueTask DisposeAsync()
    {
        foreach (var layout in _created)
            await layout.DisposeAsync();
    }

    private readonly List<BlazorAnime.Layout> _created = [];
    private BlazorAnime.Layout? _target;
    private bool _pending;
    private Func<LayoutBuilder, LayoutBuilder>? _animate;
}
