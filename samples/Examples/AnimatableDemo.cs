using BlazorAnime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Examples;

public class DocsPoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public abstract class AnimatableDemo : ComponentBase, IAsyncDisposable
{
    [Inject] protected IAnimatable Animatables { get; set; } = default!;
    [Inject] protected IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "DemoPlaying")]
    public bool? Playing { get; set; }

    protected bool Live => Playing != false;
    protected bool Reverted { get; private set; }

    protected async Task<Animatable> Create(string selector, Func<AnimatableBuilder, AnimatableBuilder> configure)
    {
        var created = await Animatables.Create(selector, configure);
        _created.Add(created);
        return created;
    }

    protected Task Track(PointerEventArgs pointer, Func<PointerEventArgs, Task> apply)
    {
        if (!Live || Reverted)
            return Task.CompletedTask;
        _pending = pointer;
        _apply = apply;
        return Drain();
    }

    protected async Task<DocsPoint> Pointer(ElementReference stage, PointerEventArgs pointer) =>
        await Js.InvokeAsync<DocsPoint>("docsPointer", stage, pointer.ClientX, pointer.ClientY);

    protected async Task<double> Angle(ElementReference element, PointerEventArgs pointer) =>
        await Js.InvokeAsync<double>("docsAngle", element, pointer.ClientX, pointer.ClientY);

    protected static double Unwrap(double current, ref double angle, ref double last)
    {
        var diff = current - last;
        if (diff > Math.PI)
            diff -= 2 * Math.PI;
        else if (diff < -Math.PI)
            diff += 2 * Math.PI;
        angle += diff;
        last = current;
        return angle;
    }

    protected static double Map(double value, double inMin, double inMax, double outMin, double outMax)
    {
        if (Math.Abs(inMax - inMin) < 0.0001)
            return outMin;
        return outMin + (outMax - outMin) * ((value - inMin) / (inMax - inMin));
    }

    protected async Task Revert(Animatable? animatable)
    {
        if (Reverted || animatable is null)
            return;
        Reverted = true;
        await animatable.Revert();
    }

    public async ValueTask DisposeAsync()
    {
        Reverted = true;
        foreach (var animatable in _created)
            await animatable.DisposeAsync();
    }

    private async Task Drain()
    {
        if (_draining)
            return;
        _draining = true;
        try
        {
            while (_pending is not null && Live && !Reverted && _apply is not null)
            {
                var next = _pending;
                _pending = null;
                await _apply(next);
            }
        }
        finally
        {
            _draining = false;
        }
    }

    private readonly List<Animatable> _created = [];
    private PointerEventArgs? _pending;
    private Func<PointerEventArgs, Task>? _apply;
    private bool _draining;
}
