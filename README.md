# Blazor.Anime

Typed Blazor bindings for [anime.js](https://animejs.com/) 4.5.0. The package embeds that engine and exposes it through `IAnime`, so animations stay in C# while playback stays in the browser. Register the service with `AddBlazorAnime()`. Blazor loads the script. Durations are milliseconds.

This guide matches the [anime.js v4 documentation](https://animejs.com/documentation/). The [v3 to v4 migration notes](https://github.com/juliangarnier/anime/wiki/Migrating-from-v3-to-v4) describe the engine this package binds.

## Features

- **anime.js 4.5.0** — targets, properties, keyframes, stagger, timelines, SVG, and easings
- **Strongly typed** — IntelliSense for transforms, colors, dimensions, and SVG attributes
- **Function parameters** — per-target values from an index or a `TargetInfo` snapshot, including lambdas and ordinary methods
- **Playback** — play, pause, resume, seek, restart, reverse, alternate, and a task that completes when the animation finishes
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
- [Animatable](#animatable)
- [Samples](#samples)
- [Building](#building)

## Installation

```bash
dotnet add package BlazorAnime
```

Register the service. That call does not inject a script tag. Blazor imports `BlazorAnime.lib.module.js` from the package when the app starts.

```csharp
builder.Services.AddBlazorAnime();
```

WebAssembly hosts keep the framework script in `wwwroot/index.html`. Blazor Web App hosts keep it in `Components/App.razor`. Do not add an anime.js script beside either one.

Add the namespace in `_Imports.razor`:

```razor
@using BlazorAnime
```

Animatable is a second package. It registers `IAnimatable` and loads `BlazorAnime.Animatable.lib.module.js`. That file imports the core module, so the page still has one engine.

```bash
dotnet add package BlazorAnime.Animatable
```

```csharp
builder.Services.AddBlazorAnime();
builder.Services.AddBlazorAnimeAnimatable();
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
                .Ease(Easing.InOutQuad)
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

A second `Animate` of the same property on the same target cancels the tween already running. That is anime.js `composition: 'replace'`, the default. `Set` does not cancel, because `utils.set` forces `composition: 'none'`.

CSS properties:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .Left(250)
    .BackgroundColor("#FFF")
    .BorderRadius("0%", "50%")
    .Ease(Easing.InOutQuad)
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

DOM attributes. `Value` is the HTML attribute named `value`. Tween destinations use `To`, not `Value`.

```csharp
await Anime.Animate(props => props
    .Targets("input")
    .Value(0, 1000)
    .ModifierRound(0)
    .Ease(Easing.InOutExpo)
    .AutoPlay(true));
```

`ModifierRound(0)` rounds to an integer. `ModifierRound(1)` keeps one decimal place. The argument is the number of decimal places.

SVG attributes. `points` is set through a property parameter because `Points` takes a builder:

```csharp
await Anime.Animate(props => props
    .Targets(".svg-attributes-demo polygon")
    .Points(shape => shape.To("64 128 8.574 96 8.574 32 64 0 119.426 32 119.426 96"))
    .Prop("baseFrequency", 0)
    .Scale(1)
    .Loop(true)
    .Alternate(true)
    .Ease(Easing.InOutExpo)
    .AutoPlay(true));
```

### Property parameters

Duration, delay, loop delay, ease, and `ModifierRound` can apply to the whole animation or to one property. `Duration`, `Delay`, and `LoopDelay` are milliseconds.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Duration(3000)
    .Delay(1000)
    .LoopDelay(1000)
    .Ease(Easing.InOutExpo)
    .ModifierRound(1)
    .AutoPlay(true));
```

Per-property timing. `To` is the destination:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(property => property
        .To(250)
        .Duration(800))
    .Rotate(property => property
        .To(360)
        .Duration(1800)
        .Ease(Easing.InOutExpo))
    .Scale(property => property
        .To(2)
        .Duration(1600)
        .Delay(800)
        .Ease(Easing.InOutQuart))
    .Delay(250)
    .AutoPlay(true));
```

Function parameters receive the target index and the target count. Lambdas and ordinary methods both work, and the function runs once per target when the animation is created.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Alternate(true)
    .Loop(true)
    .Delay((index, total) => index * 100)
    .LoopDelay((index, total) => (total - index) * 100)
    .AutoPlay(true));
```

The same shape as a method:

```csharp
public double SetDelay(int index, int total) => index * 100;
```

### Animation parameters

`Alternate(true)` flips direction on each repeat. `Reversed(true)` plays backward from the start.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(100)
    .Alternate(true)
    .Loop(true)
    .AutoPlay(true));
```

`Loop(true)` repeats forever. A number is a repeat count: `Loop(2)` plays once, then repeats twice more. Omit `Loop`, or pass `0`, for a single play. On the snapshot, `Loop` is the value stored at build, and it is `-1` when the animation loops forever.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(100)
    .Loop(2)
    .AutoPlay(true));
```

`AutoPlay(false)` creates the animation in a paused state. Call `Play` when you want it to start. An infinite loop never finishes, so `Finished()` does not complete for `Loop(true)`.

If you omit `Ease`, anime.js uses `out(2)`.

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

Function-based values use `(index, total) => ...` or a `TargetInfo`. `TargetInfo` is a snapshot taken when the animation is built: `Index`, `Total`, `Id`, `TagName`, and `Dataset`. A `data-x` attribute is `Dataset["x"]`. The function is not called again on later frames. `Refresh()` reads function values again.

```csharp
await Anime.Animate(props => props
    .Targets(".el")
    .TranslateX((index, total) => index * 40)
    .TranslateY(target => double.Parse(target.Dataset.GetValueOrDefault("x", "0")))
    .AutoPlay(true));
```

### Keyframes

Pass each step as an array of builders. A comma-separated list of lambdas binds to the `TargetInfo` overload instead of a keyframe sequence. Each step uses `To`. A step can set its own duration, delay, and ease.

Animation keyframes:

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX([
        step => step.To(250).Duration(1000).Delay(500),
        step => step.To(0).Duration(1000).Delay(500)
    ])
    .TranslateY([
        step => step.To(-40).Duration(500),
        step => step.To(40).Duration(500).Delay(1000),
        step => step.To(0).Duration(500).Delay(1000)
    ])
    .ScaleX([
        step => step.To(4).Duration(100).Delay(500).Ease(Easing.OutExpo),
        step => step.To(1).Duration(900),
        step => step.To(4).Duration(100).Delay(500).Ease(Easing.OutExpo),
        step => step.To(1).Duration(900)
    ])
    .ScaleY([
        step => step.FromTo(1.75, 1).Duration(500),
        step => step.To(2).Duration(50).Delay(1000).Ease(Easing.OutExpo),
        step => step.To(1).Duration(450),
        step => step.To(1.75).Duration(50).Delay(1000).Ease(Easing.OutExpo),
        step => step.To(1).Duration(450)
    ])
    .Ease(Easing.OutElastic(1, .8))
    .Loop(true)
    .AutoPlay(true));
```

Property keyframes are the same array on a single property. Ease on the animation covers steps that leave ease unset.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX([
        step => step.To(250).Duration(1000),
        step => step.To(0).Duration(500)
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

`From` is `StaggerPosition.First`, `StaggerPosition.Last`, `StaggerPosition.Center`, or an index:

```csharp
.Delay(stagger => stagger.Value(100).From(StaggerPosition.Center))
.Delay(stagger => stagger.Value(100).From(3))
```

`Reversed(true)` starts at the last target. `Ease` changes the gap between targets:

```csharp
.Delay(stagger => stagger
    .Value(100)
    .Reversed(true)
    .Ease(Easing.OutQuad))
```

Grid is `[columns, rows]`. `Axis` is `StaggerAxis.X` or `StaggerAxis.Y`.

```csharp
.Delay(stagger => stagger
    .Value(100)
    .Grid(14, 7)
    .From(StaggerPosition.Center)
    .Axis(StaggerAxis.X))
```

`Stagger.Create` builds the same value when you want to pass it to a property directly:

```csharp
.TranslateX(Stagger.Create(10, 40, options => options.From(StaggerPosition.Center)))
.Delay(Stagger.Create(100, options => options.Reversed(true).Ease(Easing.OutQuad)))
```

### Timeline

`CreateTimeline` plays child animations on one clock. Duration and ease set on the timeline become defaults for each child that does not set its own. `AutoPlay(false)` is playback on the timeline, not a default copied onto each child.

```csharp
var timeline = await Anime.CreateTimeline(props => props
    .Duration(750)
    .Ease(Easing.OutExpo)
    .AutoPlay(false));

await timeline.AddAsync(child => child
    .Targets(".element-1")
    .TranslateX(250));

await timeline.AddAsync(child => child
    .Targets(".element-2")
    .TranslateX(250));

await timeline.Play();
```

A position places the child. A number is an absolute time in milliseconds, and `0` starts with the timeline. `"<"` starts with the previous child. `"<<"` starts with the child before that. `"+=500"` and `"-=200"` shift from the end of the previous child. `OffSet.After(500)` is the same as `"+=500"`. `OffSet.Before(200)` is `"-=200"`.

```csharp
await timeline.AddAsync(child => child
    .Targets(".el1")
    .TranslateX(250));

await timeline.AddAsync(child => child
    .Targets(".el2")
    .TranslateX(250), "+=1000");

await timeline.AddAsync(child => child
    .Targets(".el3")
    .TranslateX(250), 0);

await timeline.AddAsync(child => child
    .Targets(".el4")
    .TranslateX(250), "<");

await timeline.AddAsync(child => child
    .Targets(".el5")
    .TranslateX(250), OffSet.Before(200));
```

A `Timeline` is an `Animation`, so play, pause, seek, restart, reverse, and dispose are the same methods.

### Controls

Every control method is asynchronous. `Seek` takes milliseconds. `Progress` takes a value from 0 to 1. `Resume` continues in the current direction. `Alternate` mirrors the current time and flips direction. `Complete` seeks to the end and removes the animation from the engine. `Finished` completes when playback ends. It does not complete while `Loop(true)` is set.

```csharp
var animation = await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .AutoPlay(false));

await animation.Play();
await animation.Pause();
await animation.Resume();
await animation.Restart();
await animation.Reverse();
await animation.Alternate();
await animation.Reset();
await animation.Seek(1500);
await animation.Progress(0.5);
await animation.Complete();
await animation.Finished();
```

`animation.Remove(".element")` drops that target from this instance. `Anime.Remove` drops it from running animations.

### Callbacks

`OnBegin`, `OnUpdate`, `OnLoop`, and `OnComplete` take `Action<AnimationState>`. Lambdas and ordinary methods both work. The argument is a snapshot, not the live instance, and the callback returns `void`.

`OnBegin` runs after the delay. `OnUpdate` runs each frame and reports `Progress` from 0 to 1. `OnLoop` runs when a repeat begins. `OnComplete` runs after the repeats finish.

```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Delay(500)
    .Alternate(true)
    .OnBegin(state => Console.WriteLine($"Started at {state.CurrentTime}ms"))
    .OnUpdate(state => Console.WriteLine($"Progress: {state.Progress:0.00}"))
    .OnLoop(state => Console.WriteLine($"Repeat {state.CurrentIteration}"))
    .OnComplete(state => Console.WriteLine("Animation completed"))
    .Loop(2)
    .AutoPlay(true));
```

`OnRender`, `OnPause`, and `OnBeforeUpdate` use the same snapshot.

`AnimationState` also carries `Began`, `Completed`, `Paused`, `Reversed`, `Backwards`, `Alternate`, `Duration`, `Delay`, `LoopDelay`, `CurrentTime`, `IterationProgress`, `CurrentIteration`, and `Loop`. `Loop` is `-1` when the animation loops forever. `Alternate` on the snapshot is the flag stored at build.

### SVG

Line drawing. `CreateDrawable` wraps the geometry, and `Draw` animates the stroke. `Draw("0 0", "0 1", "1 1")` draws the stroke on, then carries the dash off the end.

```csharp
var lines = await Anime.CreateDrawable(".line-drawing path");
await Anime.Animate(props => props
    .Targets(lines)
    .Draw("0 0", "0 1", "1 1")
    .Ease(Easing.InOutQuad)
    .Duration(2000)
    .Delay(Stagger.Create(100))
    .Loop(true)
    .AutoPlay(true));
```

Dispose the `DrawableTarget` with the animation.

Morphing. `MorphTo` returns the function anime.js builds for another shape. Pass that reference to `D` or `Points`. Do not pass a string array to `D`. Precision defaults to `0.33`. `0` copies the target shape. A path string can still be assigned with `D(string)` when you are not morphing toward an element.

```csharp
var morph = await Anime.MorphTo(".target-shape");
await Anime.Animate(props => props
    .Targets(".morphing path")
    .D(morph)
    .Duration(2000)
    .Ease(Easing.InOutQuad)
    .AutoPlay(true));
```

Motion path. `CreateMotionPath` accepts a selector or an `ElementReference`. Offset is 0 to 1 along the path, and the default `0` starts at the beginning. The three fields are functions. Pass them to the matching transforms.

```csharp
var motion = await Anime.CreateMotionPath(".motion-path path");
await Anime.Animate(props => props
    .Targets(".motion-path .el")
    .TranslateX(motion.TranslateX)
    .TranslateY(motion.TranslateY)
    .Rotate(motion.Rotate)
    .Duration(2000)
    .Ease(Easing.Linear)
    .Loop(true)
    .AutoPlay(true));
```

Dispose the `MotionPath` with the animation.

### Easings

Named curves drop the `ease` prefix. They are properties on `Easing`:

- `Linear`
- `InSine`, `OutSine`, `InOutSine`, `OutInSine`
- `InQuad`, `OutQuad`, `InOutQuad`, `OutInQuad`
- `InCubic`, `OutCubic`, `InOutCubic`, `OutInCubic`
- `InQuart`, `OutQuart`, `InOutQuart`, `OutInQuart`
- `InQuint`, `OutQuint`, `InOutQuint`, `OutInQuint`
- `InExpo`, `OutExpo`, `InOutExpo`, `OutInExpo`
- `InCirc`, `OutCirc`, `InOutCirc`, `OutInCirc`
- `InBack`, `OutBack`, `InOutBack`, `OutInBack`
- `InBounce`, `OutBounce`, `InOutBounce`, `OutInBounce`

`In(power)`, `Out(power)`, and `InOut(power)` build a curve from a number. `out(2)` is the ease anime.js uses when a call omits one.

Elastic easing is a method. Amplitude defaults to `1` and period defaults to `0.3`.

```csharp
.Ease(Easing.OutElastic(1, .6))
```

`InElastic`, `InOutElastic`, and `OutInElastic` take the same arguments.

`Spring()` uses the anime.js defaults: mass `1`, stiffness `100`, damping `10`, and velocity `0`. The four-argument method sets those values. `SpringBounce` takes a bounce from -1 to 1 and a duration in milliseconds.

```csharp
.Ease(Easing.Spring())
.Ease(Easing.Spring(1, 80, 10, 0))
.Ease(Easing.SpringBounce(0.5, 600))
```

Cubic Bézier, steps, and a raw expression the v4 parser accepts:

```csharp
.Ease(Easing.CubicBezier(.5, .05, .1, .3))
.Ease(Easing.Steps(5))
.Ease(Easing.Raw("inElastic(1, .5)"))
```

A C# function can be the ease. `Easing.Curve` samples it from 0 to 1 (64 samples by default, at least 2) and the browser interpolates that table on each frame. The same call works on a stagger.

```csharp
.Ease(Easing.Curve(t => t * t))
.Delay(stagger => stagger.Value(100).Ease(Easing.Curve(t => t * t, samples: 32)))
```

### Helpers

`Random`, `Get`, `Set`, and `Remove` are `utils`. Speed and `PauseOnDocumentHidden` are the engine. `Version()` is the pinned anime.js build, `4.5.0`.

`Random` is asynchronous. Read it before you build the animation:

```csharp
var distance = await Anime.Random(0, 250);
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(distance)
    .AutoPlay(true));
```

`Get` reads computed style. Without a unit the result is a string, usually in pixels. The unit overload converts it and returns a number. `Set` writes immediately and does not cancel other tweens.

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

Engine helpers:

```csharp
await Anime.SetSpeed(1.5);
var speed = await Anime.GetSpeed();
var version = await Anime.Version();
await Anime.PauseOnDocumentHidden(false);
var pauses = await Anime.GetPauseOnDocumentHidden();
```

`PauseOnDocumentHidden(true)` is the anime.js default: playback pauses while the tab is hidden.

## Animatable

`BlazorAnime.Animatable` binds `createAnimatable`. A property is a number duration in milliseconds, or a settings object. Calling it later animates to a new number. `Get` reads one number. `GetValues` reads a color or any other list of numbers. `Revert` restores the original values. `DisposeAsync` pauses and releases the instance.

```razor
@implements IAsyncDisposable
@inject IAnimatable Animatable

<div class="square"></div>

@code {
    private Animatable? _square;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _square = await Animatable.Create(".square", props => props
                .Property("x", 500)
                .Property("y", property => property.Duration(500).Ease(Easing.Out(4)))
                .Ease(Easing.Out(3)));
        }
    }

    private async Task Follow(double x, double y)
    {
        if (_square is null)
            return;
        await _square.Set("x", x);
        await _square.Set("y", y);
    }

    public async ValueTask DisposeAsync()
    {
        if (_square is not null)
            await _square.DisposeAsync();
    }
}
```

Property names are the anime.js names: `x`, `translateX`, `backgroundColor`. A setter accepts a `double` or a list of doubles. Pass a duration and an `Easing` when that one call should override the settings:

```csharp
await _square.Set("y", 50, durationMilliseconds: 500, ease: Easing.Out(2));
await _square.Set("backgroundColor", new[] { 164d, 255, 79 });
```

## Samples

Pushes to `master` publish both samples to [olaboutcode.github.io/blazor-anime](https://olaboutcode.github.io/blazor-anime/). In the repository settings, set GitHub Pages to deploy from GitHub Actions. `scripts/publish-pages.sh` builds that same site locally, and `scripts/publish-pages.bat` does it on Windows.

`samples/Examples` walks through the anime.js documentation demos for the modules this package implements: animation, timeline, SVG, utilities, and easings. The http profile listens on `http://localhost:5161`.

```bash
dotnet run --project samples/Examples
```

`samples/FunExamples` holds the longer demos: a wireframe sphere, Easter icons, a pedaling bicycle, and a 404 page. Routes are `/`, `/animated-sphere`, `/easter-icons`, `/pedaling-bicycle`, and `/error-404`. The http profile listens on `http://localhost:5181`.

```bash
dotnet run --project samples/FunExamples
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
node --test test/interop/*.cjs
dotnet test test/BlazorAnime.UiTests/BlazorAnime.UiTests.csproj
```

The browser tests need Playwright's Chromium driver, which the UI test project downloads on build.
