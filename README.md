# Blazor.Anime

Typed Blazor bindings for [anime.js](https://animejs.com/) v3. The package embeds anime.js 3.2.2 and exposes it through `IAnime`, so animations stay in C# while playback stays in the browser. The live anime.js documentation describes v4, which is a different API. This guide matches the v3 engine shipped in the package.

## Features

- **anime.js v3 API** — targets, properties, keyframes, stagger, timelines, SVG, and easings
- **Strongly typed** — IntelliSense for transforms, colors, dimensions, and SVG attributes
- **Function parameters** — per-target values from an index or a `TargetInfo` snapshot, including lambdas and ordinary methods
- **Playback** — play, pause, seek, progress, restart, and a task that completes when the animation finishes
- **Object targets** — animate a plain JavaScript object and read the values back

## Contents

- [Installation](#installation)
- [Usage example](#usage-example)
- [Documentation](#documentation)
  - [Targets](#targets)
  - [Properties](#properties)
  - [Property parameters](#property-parameters)
  - [Animation parameters](#animation-parameters)
  - [Values](#values)
  - [Keyframes](#keyframes)
  - [Staggering](#staggering)
  - [Timeline](#timeline)
  - [Controls](#controls)
  - [Callbacks](#callbacks)
  - [SVG](#svg)
  - [Easings](#easings)
  - [Helpers](#helpers)
- [Samples](#samples)
- [Building](#building)

## Installation

```bash
dotnet add package BlazorAnime
```

Register the service:

```csharp
builder.Services.AddBlazorAnime();
```

Load the interop script **after** the Blazor framework script.

WebAssembly, in `wwwroot/index.html`:

```html
<script src="_framework/blazor.webassembly.js"></script>
<script src="_content/BlazorAnime/blazor.anime.interop.js"></script>
```

Blazor Web App, in `Components/App.razor` (or the host page):

```html
<script src="_framework/blazor.web.js"></script>
<script src="_content/BlazorAnime/blazor.anime.interop.js"></script>
```

Add the namespace in `_Imports.razor`:

```razor
@using BlazorAnime
```

## Usage example

```razor
@implements IAsyncDisposable
@inject IAnime Anime

<div @ref="element">Animate me!</div>
<button @onclick="OnPlayButtonClicked">Play Animation</button>

@code {
    private ElementReference element;
    private Animation? _animation;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _animation ??= await Anime.Animate(props => props
                .Targets(element)
                .TranslateX(250)
                .Rotate(360)
                .Duration(1500)
                .Easing(Easing.EaseInOutQuad)
                .AutoPlay(false));
        }
    }

    private async Task OnPlayButtonClicked()
    {
        if (_animation is not null)
            await _animation.Play();
    }

    public async ValueTask DisposeAsync()
    {
        if (_animation is not null)
            await _animation.DisposeAsync();
    }
}
```

Create the animation in `OnAfterRenderAsync`, after the element exists. `DisposeAsync` pauses playback and releases the JavaScript instance, including callback handles.

## Documentation

Build animations after the first render, once the target elements exist.

### Targets

A target is a CSS selector, an `ElementReference`, or a `JsTarget`. Pseudo-elements cannot be selected.

CSS selector:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Duration(700)
    .AutoPlay(true));
```

Several selectors:

```csharp
await Anime.Animate(props => props
    .Targets(".element-a", ".element-b")
    .TranslateY(250)
    .Duration(300)
    .AutoPlay(true));
```

An element reference, from `@ref`:

```csharp
await Anime.Animate(props => props
    .Targets(element)
    .TranslateX(250)
    .Duration(700));
```

A plain JavaScript object. Property names are sent as-is, and you can read them back while the animation runs:

```csharp
var target = await Anime.CreateObject(new Dictionary<string, object>
{
    ["progress"] = 0d
});

await Anime.Animate(props => props
    .Targets(target)
    .Prop("progress", 1)
    .Duration(1000)
    .AutoPlay(true));

var progress = await target.GetNumber("progress");
```

`GetString` reads a string property. Dispose the `JsTarget` when you are done with it.

### Properties

Any CSS property, DOM attribute, or SVG attribute can be animated. Typed methods cover the common names. Anything else goes through `Prop`.

CSS properties:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .Left(250)
    .BackgroundColor("#FFF")
    .BorderRadius("0%", "50%")
    .Easing(Easing.EaseInOutQuad)
    .AutoPlay(true));
```

CSS transforms:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Scale(2)
    .Rotate("1turn")
    .AutoPlay(true));
```

DOM attributes:

```csharp
await Anime.Animate(props => props
    .Targets("input")
    .Value(0, 1000)
    .Round(1)
    .Easing(Easing.EaseInOutExpo)
    .AutoPlay(true));
```

SVG attributes. `points` is set through a property parameter because `Points` takes a builder:

```csharp
await Anime.Animate(props => props
    .Targets(".svg-attributes-demo polygon")
    .Points(shape => shape.Value("64 128 8.574 96 8.574 32 64 0 119.426 32 119.426 96"))
    .Prop("baseFrequency", 0)
    .Scale(1)
    .Loop(true)
    .Direction(Direction.Alternate)
    .Easing(Easing.EaseInOutExpo)
    .AutoPlay(true));
```

### Property parameters

Duration, delay, end delay, easing, and round can apply to the whole animation or to one property.

Duration, delay, and easing:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Duration(3000)
    .Delay(1000)
    .EndDelay(1000)
    .Easing(Easing.EaseInOutExpo)
    .Round(10) // 1 rounds to an integer, 10 to one decimal, 100 to two
    .AutoPlay(true));
```

Per-property timing:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(property => property
        .Value(250)
        .Duration(800))
    .Rotate(property => property
        .Value(360)
        .Duration(1800)
        .Easing(Easing.EaseInOutExpo))
    .Scale(property => property
        .Value(2)
        .Duration(1600)
        .Delay(800)
        .Easing(Easing.EaseInOutQuart))
    .Delay(250)
    .AutoPlay(true));
```

Function parameters receive the target index and the target count. Lambdas and ordinary methods both work, and the function runs once per target when the animation is created.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Direction(Direction.Alternate)
    .Loop(true)
    .Delay((index, total) => index * 100)
    .EndDelay((index, total) => (total - index) * 100)
    .AutoPlay(true));
```

The same shape as a method:

```csharp
public double SetDelay(int index, int total) => index * 100;
```

### Animation parameters

Direction is `Direction.Normal`, `Direction.Reverse`, or `Direction.Alternate`.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(100)
    .Direction(Direction.Alternate)
    .Loop(true)
    .AutoPlay(true));
```

`Loop(true)` repeats forever. `Loop(3)` plays the animation three times.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(100)
    .Loop(3)
    .AutoPlay(true));
```

`AutoPlay(false)` creates the animation in a paused state. Call `Play` when you want it to start. An infinite loop never finishes, so `Finished()` does not complete for `Loop(true)`.

### Values

Unitless values and explicit units:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(0, 250)
    .Width("100%")
    .Rotate("1turn")
    .BackgroundColor("#FFF", "#000")
    .Opacity(0, 1)
    .AutoPlay(true));
```

Relative values. A unit string works on transforms. `Relative` is the typed form and also accepts units.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX("+=100")
    .Rotate(Relative.Add("1turn"))
    .AutoPlay(true));
```

`Relative.Add`, `Relative.Subtract`, and `Relative.Multiply` take a number or a string.

Function-based values use `(index, total) => ...` or a `TargetInfo`. `TargetInfo` is a snapshot taken when the animation is built: `Index`, `Total`, `Id`, `TagName`, and `Dataset`. A `data-x` attribute is `Dataset["x"]`. The function is not called again on later frames.

```csharp
await Anime.Animate(props => props
    .Targets(".el")
    .TranslateX((index, total) => index * 40)
    .TranslateY(target => double.Parse(target.Dataset.GetValueOrDefault("x", "0")))
    .AutoPlay(true));
```

### Keyframes

Pass each step as an array of builders. A comma-separated list of lambdas binds to the `TargetInfo` overload instead of a keyframe sequence.

Animation keyframes:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX([
        step => step.Value(250).Duration(1000).Delay(500),
        step => step.Value(0).Duration(1000).Delay(500)
    ])
    .TranslateY([
        step => step.Value(-40).Duration(500),
        step => step.Value(40).Duration(500).Delay(1000),
        step => step.Value(0).Duration(500).Delay(1000)
    ])
    .ScaleX([
        step => step.Value(4).Duration(100).Delay(500).Easing(Easing.EaseOutExpo),
        step => step.Value(1).Duration(900),
        step => step.Value(4).Duration(100).Delay(500).Easing(Easing.EaseOutExpo),
        step => step.Value(1).Duration(900)
    ])
    .ScaleY([
        step => step.Value(1.75, 1).Duration(500),
        step => step.Value(2).Duration(50).Delay(1000).Easing(Easing.EaseOutExpo),
        step => step.Value(1).Duration(450),
        step => step.Value(1.75).Duration(50).Delay(1000).Easing(Easing.EaseOutExpo),
        step => step.Value(1).Duration(450)
    ])
    .Easing(Easing.EaseOutElastic(1, .8))
    .Loop(true)
    .AutoPlay(true));
```

Property keyframes are the same array on a single property:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX([
        step => step.Value(250).Duration(1000),
        step => step.Value(0).Duration(500)
    ])
    .Duration(4000)
    .AutoPlay(true));
```

### Staggering

`Delay(stagger => ...)` builds an anime.js stagger.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Delay(stagger => stagger.Value(100))
    .AutoPlay(true));
```

Start offset:

```csharp
.Delay(stagger => stagger.Value(100).Start(500))
```

`From` is `StaggerFrom.First`, `StaggerFrom.Last`, `StaggerFrom.Center`, or an index:

```csharp
.Delay(stagger => stagger.Value(100).From(StaggerFrom.Center))
.Delay(stagger => stagger.Value(100).From(3))
```

Direction and easing:

```csharp
.Delay(stagger => stagger
    .Value(100)
    .Direction(Direction.Reverse)
    .Easing(Easing.EaseOutQuad))
```

Grid is `[columns, rows]`. `Axis` is `StaggerAxis.X` or `StaggerAxis.Y`.

```csharp
.Delay(stagger => stagger
    .Value(100)
    .Grid(14, 7)
    .From(StaggerFrom.Center)
    .Axis(StaggerAxis.X))
```

`Stagger.Create` builds the same value when you want to pass it to a property directly, for example `.TranslateX(Stagger.Create(10, 40, options => options.From(StaggerFrom.Center)))`.

### Timeline

Defaults on the timeline apply to each child that does not set its own.

```csharp
var timeline = await Anime.Timeline(props => props
    .Duration(750)
    .Easing(Easing.EaseOutExpo)
    .AutoPlay(false));

await timeline.AddAsync(child => child
    .Targets(".element-1")
    .TranslateX(250));

await timeline.AddAsync(child => child
    .Targets(".element-2")
    .TranslateX(250));

await timeline.Play();
```

Offsets are a number of milliseconds, a relative string, or `OffSet`:

```csharp
await timeline.AddAsync(child => child
    .Targets(".el1")
    .TranslateX(250));

await timeline.AddAsync(child => child
    .Targets(".el2")
    .TranslateX(250), "+=1000");

await timeline.AddAsync(child => child
    .Targets(".el3")
    .TranslateX(250), 2000);

await timeline.AddAsync(child => child
    .Targets(".el4")
    .TranslateX(250), OffSet.Before(200));
```

`OffSet.After(500)` is the same as `"+=500"`. A `Timeline` is an `Animation`, so play, pause, seek, restart, reverse, and dispose are the same methods.

### Controls

Every control method is asynchronous.

```csharp
var animation = await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .AutoPlay(false));

await animation.Play();
await animation.Pause();
await animation.Restart();
await animation.Reverse();
await animation.Reset();
await animation.Seek(1500);
await animation.Progress(50);
await animation.Complete();
await animation.Finished();
```

`Seek` and `Progress` take milliseconds and a percentage from 0 to 100. `Complete` jumps to the end. `Finished` completes when playback ends. It does not complete while `Loop(true)` is set.

`animation.Remove(".element")` drops that target from this instance. `Anime.Remove` drops it from every running instance.

### Callbacks

Begin, update, complete, loop, and change callbacks take `Action<AnimationState>`. Lambdas and ordinary methods both work. The argument is a snapshot of the instance at that frame, and the callback returns `void`.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Begin(state => Console.WriteLine($"Started at {state.CurrentTime}ms"))
    .Update(state => Console.WriteLine($"Progress: {state.Progress}%"))
    .Complete(state => Console.WriteLine("Animation completed"))
    .Loop(true)
    .LoopBegin(state => Console.WriteLine($"{state.Remaining} loops left"))
    .AutoPlay(true));
```

The same hooks exist for `LoopComplete`, `Change`, `ChangeBegin`, and `ChangeComplete`.

`AnimationState` also carries `Began`, `Completed`, `Paused`, `Reversed`, `Duration`, `Delay`, `EndDelay`, `Direction`, `Loop`, and `Remaining`. `Loop` and `Remaining` are `-1` when the animation loops forever.

### SVG

Line drawing:

```csharp
await Anime.Animate(props => props
    .Targets(".line-drawing path")
    .StrokeDashoffset(0)
    .Easing(Easing.EaseInOutSine)
    .Duration(1500)
    .Delay((index, total) => index * 250)
    .Direction(Direction.Alternate)
    .Loop(true)
    .AutoPlay(true));
```

`Anime.SetDashoffset` sets `stroke-dasharray` to the path length and returns that length:

```csharp
var length = await Anime.SetDashoffset(".line-drawing path");
```

Morphing:

```csharp
await Anime.Animate(props => props
    .Targets(".morphing path")
    .D("M10 80 C 40 10, 65 10, 95 80 S 150 150, 180 80")
    .Duration(2000)
    .Easing(Easing.EaseInOutQuad)
    .AutoPlay(true));
```

Motion path. `GetSvgPath` accepts a selector or an `ElementReference`. The optional percent argument is how much of the path to travel, from 0 to 100.

```csharp
var path = await Anime.GetSvgPath(".motion-path path");
await Anime.Animate(async props => props
    .Targets(".motion-path .el")
    .TranslateX(await path.Get("x"))
    .TranslateY(await path.Get("y"))
    .Rotate(await path.Get("angle"))
    .Duration(2000)
    .Easing(Easing.Linear)
    .Loop(true)
    .AutoPlay(true));
```

Dispose the `SvgPath` with the animation. `"x"`, `"y"`, and `"angle"` are the path properties anime.js exposes.

### Easings

Named curves are properties on `Easing`:

- `Linear`
- `EaseInSine`, `EaseOutSine`, `EaseInOutSine`, `EaseOutInSine`
- `EaseInQuad`, `EaseOutQuad`, `EaseInOutQuad`, `EaseOutInQuad`
- `EaseInCubic`, `EaseOutCubic`, `EaseInOutCubic`, `EaseOutInCubic`
- `EaseInQuart`, `EaseOutQuart`, `EaseInOutQuart`, `EaseOutInQuart`
- `EaseInQuint`, `EaseOutQuint`, `EaseInOutQuint`, `EaseOutInQuint`
- `EaseInExpo`, `EaseOutExpo`, `EaseInOutExpo`, `EaseOutInExpo`
- `EaseInCirc`, `EaseOutCirc`, `EaseInOutCirc`, `EaseOutInCirc`
- `EaseInBack`, `EaseOutBack`, `EaseInOutBack`, `EaseOutInBack`
- `EaseInBounce`, `EaseOutBounce`, `EaseInOutBounce`, `EaseOutInBounce`

Elastic easing is a method, because amplitude and period are optional. The defaults are `1` and `0.5`.

```csharp
.Easing(Easing.EaseOutElastic(1, .6))
```

`EaseInElastic`, `EaseInOutElastic`, and `EaseOutInElastic` take the same arguments.

Cubic Bézier, spring, and steps:

```csharp
.Easing(Easing.CubicBezier(.5, .05, .1, .3))
.Easing(Easing.Spring(1, 80, 10, 0))
.Easing(Easing.Steps(5))
```

`Easing.Spring()` with no arguments is anime.js `spring`. `Easing.Raw("easeInElastic(1, .5)")` passes an expression through unchanged.

A C# function can be the easing. `Easing.Curve` samples it from 0 to 1 (64 samples by default, at least 2) and the browser interpolates that table on each frame. The same call works on a stagger.

```csharp
.Easing(Easing.Curve(t => t * t))
.Delay(stagger => stagger.Value(100).Easing(Easing.Curve(t => t * t, samples: 32)))
```

### Helpers

`Random` is asynchronous. Read it before you build the animation:

```csharp
var distance = await Anime.Random(0, 250);
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(distance)
    .AutoPlay(true));
```

`Get` uses computed style, so the result is usually in pixels unless you pass a unit.

```csharp
var value = await Anime.Get(".element", "translateX");
var percent = await Anime.Get(element, "width", "%");
await Anime.Set(".element", "translateX", 250);
await Anime.Set(element, props => props.TranslateX(250).Rotate(45));
```

`Remove` takes targets out of running animations. It leaves the elements in the document.

```csharp
await Anime.Remove(".element");
```

Other engine helpers:

```csharp
await Anime.SetSpeed(1.5);
var speed = await Anime.GetSpeed();
var version = await Anime.Version();
var converted = await Anime.ConvertPx(element, "16px", "rem");
await Anime.SuspendWhenDocumentHidden(false);
var running = await Anime.RunningLength();
```

`Version` is the embedded anime.js version. `SuspendWhenDocumentHidden(true)` is the anime.js default: playback pauses while the tab is hidden.

## Samples

Pushes to `master` publish both samples to [olaboutcode.github.io/blazor-anime](https://olaboutcode.github.io/blazor-anime/). In the repository settings, set GitHub Pages to deploy from GitHub Actions. `scripts/publish-pages.sh` builds that same site locally, and `scripts/publish-pages.bat` does it on Windows.

`samples/Examples.WebAssembly` walks through the anime.js docs and a few longer demos (a wireframe sphere, Easter icons, a bicycle, and a 404 page). The http profile listens on `http://localhost:5161`.

```bash
dotnet run --project samples/Examples.WebAssembly
```

`samples/PageTransitions` is a Blazor WebAssembly recreation of the CSS-Tricks travel-app page transitions. Routes are `/`, `/place`, and `/group`. The http profile listens on `http://localhost:5175`.

```bash
dotnet run --project samples/PageTransitions
```

## Building

The solution file is `BlazorAnime.slnx`. Package versions are declared once in `Directory.Packages.props`.

```bash
dotnet build BlazorAnime.slnx
dotnet test test/BlazorAnime.Tests/BlazorAnime.Tests.csproj
node --test test/interop/anime-interop.test.cjs
dotnet test test/BlazorAnime.UiTests/BlazorAnime.UiTests.csproj
```

The browser tests need Playwright's Chromium driver, which the UI test project downloads on build.
