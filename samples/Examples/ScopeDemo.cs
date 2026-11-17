using BlazorAnime;
using Microsoft.AspNetCore.Components;

namespace Examples;

public abstract class ScopeDemo : ComponentBase, IAsyncDisposable
{
    [Inject] protected IScope Scopes { get; set; } = default!;

    [Inject] protected IAnime Anime { get; set; } = default!;

    [CascadingParameter(Name = "DemoPlaying")]
    public bool? Playing { get; set; }

    protected bool Live => Playing != false;

    private readonly List<BlazorAnime.Scope> _created = [];
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
            await DisposeScopes();
        }
    }

    protected async Task<BlazorAnime.Scope> Open(Func<ScopeBuilder, ScopeBuilder>? configure = null)
    {
        var scope = configure is null
            ? await Scopes.Create()
            : await Scopes.Create(configure);
        _created.Add(scope);
        return scope;
    }

    protected abstract Task Start();

    public async ValueTask DisposeAsync() => await DisposeScopes();

    private async Task DisposeScopes()
    {
        var scopes = _created.ToArray();
        _created.Clear();
        foreach (var scope in scopes)
            await scope.DisposeAsync();
    }
}
