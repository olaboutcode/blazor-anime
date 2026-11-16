using BlazorAnime;
using Microsoft.AspNetCore.Components;

namespace Examples;

public abstract class DraggableDemo : ComponentBase, IAsyncDisposable
{
    [Inject] protected IDraggable Draggables { get; set; } = default!;

    [CascadingParameter(Name = "DemoPlaying")]
    public bool? Playing { get; set; }

    protected bool Live => Playing != false;

    protected async Task<Draggable> Create(string selector, Func<DraggableBuilder, DraggableBuilder>? configure = null)
    {
        var created = configure is null
            ? await Draggables.Create(selector)
            : await Draggables.Create(selector, configure);
        _created.Add(created);
        return created;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var drag in _created)
            await drag.DisposeAsync();
    }

    private readonly List<Draggable> _created = [];
}
