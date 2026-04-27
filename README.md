# Blazor.Anime

A powerful and easy-to-use Blazor wrapper for [anime.js v3](https://animejs.com), the lightweight JavaScript animation library with a simple yet powerful API. Create smooth, performant animations in your Blazor applications with a fully typed C# API.

## Features

- **Full anime.js v3 Support** - Complete wrapper for all anime.js v3 features
- **Strongly Typed** - IntelliSense support with comprehensive extension methods
- **Rich Animation Properties** - Animate transforms, colors, dimensions, typography, and more
- **Timeline Support** - Create complex animation sequences with full control
- **Stagger Animations** - Easily animate multiple elements with delays and offsets
- **SVG Support** - Full anime.js v3 support for SVG morphing, motion paths, and line drawing
- **Function-Based Parameters** - Dynamic animations with callback functions
- **Animation Controls** - Play, pause, seek, and control animations programmatically

## Table Of Content
- [Installation](#installation)
- [Documentation](#documentation)
  - [Targets](#targets)
  - [Properties](#properties)
  - [Property Parameters](#property-parameters)
  - [Animation Parameters](#animation-parameters)
  - [Values](#values)
  - [Keyframes](#keyframes)
  - [Staggering](#staggering)
  - [Timeline](#timeline)
  - [Controls](#controls)
  - [Callbacks](#callbacks)
  - [SVG](#svg)
  - [Easings](#easings)

## Installation

Install Blazor Anime package
```bash
dotnet add package BlazorAnime
```
Register Blazor Anime services in your `program.cs`
```
builder.Services.AddBlazorAnime();
```
In your `_Host.cshtml` (server-side) or in your `index.html` (client-side) add the following lines to the `body` tag **after** the `_framework` reference
```html
<!-- Reference the animeBlazor.js javascript file. -->
<script src="_content/BlazorAnime/blazor.anime.js"></script>
```
Add Blazor Anime reference in your `_Imports.razor`
```
@using BlazorAnime
```

## Usage Example

```csharp
@implements IAsyncDisposable;
@inject IAnime Anime;

<div @ref="element">Animate me!</div>
<button @click="OnPlayButtonClicked">Play Animation</button>

@code {
    ElementReference element;
    private Animation? _animation;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            animation ??= await Anime.Animate(props => props
                .Targets(element)
                .TranslateX(250)
                .Rotate(360)
                .Duration(1500)
                .Easing(Easing.EaseInOutQuad)
                .AutoPlay(false));
        }
    }

    public void OnPlayButtonClicked() => animation?.Play();

    // Dispose animation js references 
    async ValueTask DisposeAsync()
    {
        if (animation is not null)
            await animation.DisposeAsync();
    }
}
```

## Documentation
### Targets
Targets can be any CSS selector. Pseudo elements cannot be selected.
Official docs: https://animejs.com/v3/documentation/#cssSelector
#### CSS selector
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Duration(700)
    .AutoPlay(true));
```
#### Mutliple targets
```csharp
await Anime.Animate(props => props
    .Targets(".element-a", ".element-b")
    .TranslateY(250)
    .Duration(300)
    .AutoPlay(true));
```

### Properties
Any CSS properties can be animated.
Official docs: https://animejs.com/v3/documentation/#cssProperties

#### CSS Properties
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .Left(250)
    .BackgroundColor("#FFF")
    .BorderRadius("0%", "50%")
    .Easing(Easing.EaseInOutQuad)
    .AutoPlay(true));
```
#### CSS Transforms
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Scale(2)
    .Rotate("1turn")
    .AutoPlay(true));
```
#### DOM attributes
```csharp
await Anime.Animate(props => props
    .Targets("input")
    .Value(0, 1000)
    .Round(1)
    .Easing(Easing.EaseInOutExpo)
    .AutoPlay(true));
```
#### SVG attributes
```csharp
await Anime.Animate(props => props
    .Targets(".svg-attributes-demo polygon") // simplified
    .Points("64 128 8.574 96 8.574 32 64 0 119.426 32 119.426 96")
    .Prop("baseFrequency", 0) // custom property
    .Scale(1)
    .Loop(true)
    .Direction(Direction.Alternate)
    .Easing(Easing.EaseInOutExpo)
    .AutoPlay(true));
```

### Property Parameters
How to define property parameters
Official docs: https://animejs.com/v3/documentation/#duration
#### Duration, Delay, EndDelay, Easing, and Round
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Duration(3000) // duration in milliseconds
    .Delay(1000) // delay in milliseconds
    .EndDelay(1000) // Add extra time at the end of the animation
    .Easing(Easing.EaseInOutExpo) // The timing function of the animation
    .Round(10) // Round the animated value to 1 decimal
    .AutoPlay(true));
```
#### Specific Property Parameters
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(props => props
        .Value(250)
        .Duration(800)
    )
    .Rotate(props => props
        .Value(360)
        .Duration(1800)
        .Easing(Easing.EaseInOutExpo)
    )
    .Scale(props => props
        .Value(2)
        .Duration(1600)
        .Delay(800)
        .Easing(Easing.EaseInOutQuart)
    )
    .Delay(250)
    .AutoPlay(true));
```
#### Function Based Parameters
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Direction(Direction.Alternate)
    .Loop(true)
    .Delay(SetDelay)
    .EndDelay(SetEndDelay)
    .AutoPlay(true));
    
// callbacks
[JsInvokable]
public double SetDelay(int i, int l) => i * 100;
[JsInvokable]
public double SetEndDelay(int i, int l) => (l - i) * 100;
```

### Animation Parameters
Control how your animation plays with these parameters.
Official docs: https://animejs.com/v3/documentation/#direction
#### Direction
Set the animation direction.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(100)
    .Direction(Direction.Alternate) // Plays forward, then backward
    .Loop(true)
    .AutoPlay(true));
```
Available directions: `Normal`, `Reverse`, `Alternate`

#### Loop
Make animations repeat.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(100)
    .Loop(true) // Loop infinitely
    .AutoPlay(true));

// Or specify a number
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(100)
    .Loop(3) // Loop 3 times
    .AutoPlay(true));
```

#### Autoplay
Control whether animation plays automatically.
```csharp
var animation = await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .AutoPlay(false)); // Don't play automatically

// Play it later
animation.Play();
```

### Values
Different ways to specify values for your animations.
Official docs: https://animejs.com/v3/documentation/#unitlessValue
#### From → To
Animate from a specific value to another.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(0, 250) // From 0 to 250
    .AutoPlay(true));
```

#### Relative Values
Use `+=` or `-=` for relative values.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX("+=100") // Move 100px from current position
    .AutoPlay(true));
```

#### Specific Property Values
Pass values with specific units.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .Width("100%")
    .TranslateX("250px")
    .Rotate("1turn")
    .AutoPlay(true));
```

#### Unit-less Values
Colors and properties without units.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .BackgroundColor("#FFF", "#000")
    .Opacity(0, 1)
    .AutoPlay(true));
```

### Keyframes
Create complex animations with keyframes.
Official docs: https://animejs.com/v3/documentation/#animationKeyframes
#### Property Keyframes
Animate a single property through multiple values.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX([
        kf => kf.Value(250).Duration(1000),
        kf => kf.Value(0).Duration(500)
    ])
    .Duration(4000)
    .AutoPlay(true));
```

#### Animation Keyframes
Animate multiple properties at specific keyframes.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX([
        kf => kf.Value(250).Duration(1000).Delay(500),
        kf => kf.Value(0).Duration(1000).Delay(500)
    ])
    .TranslateY([
        kf => kf.Value(-40).Duration(500),
        kf => kf.Value(40).Duration(500).Delay(1000),
        kf => kf.Value(0).Duration(500).Delay(1000)
    ])
    .ScaleX([
        kf => kf.Value(4).Duration(100).Delay(500).Easing(Easing.EaseOutExpo),
        kf => kf.Value(1).Duration(900),
        kf => kf.Value(4).Duration(100).Delay(500).Easing(Easing.EaseOutExpo),
        kf => kf.Value(1).Duration(900)
    ])
    .ScaleY([
        kf => kf.Value(1.75, 1).Duration(500),
        kf => kf.Value(2).Duration(50).Delay(1000).Easing(Easing.EaseOutExpo),
        kf => kf.Value(1).Duration(450),
        kf => kf.Value(1.75).Duration(50).Delay(1000).Easing(Easing.EaseOutExpo),
        kf => kf.Value(1).Duration(450)
    ])
    .Easing(Easing.EaseOutElastic(1, .8))
    .Loop(true)
    .AutoPlay(true)
);
```

### Staggering
Animate multiple elements with incremental delays.
Official docs: https://animejs.com/v3/documentation/#staggeringBasics
#### Basic Stagger
Start each animation with a delay.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Delay(stagger => stagger.Value(100)) // Increase delay by 100ms for each element
    .AutoPlay(true));
```

#### Stagger Start Value
Start from a specific delay.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Delay(stagger => stagger
        .Value(100)
        .Start(500) // Start at 500ms
    )
    .AutoPlay(true));
```

#### Stagger From
Control where stagger starts from.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .Scale(2)
    .Delay(stagger => stagger
        .Value(100)
        .From(StaggerFrom.Center) // Start from center
    )
    .AutoPlay(true));
```
Available from values: `First`, `Last`, `Center`, or an index number

#### Stagger Direction
Change stagger direction.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Delay(stagger => stagger
        .Value(100)
        .Direction(true) // Reverse direction
    )
    .AutoPlay(true));
```

#### Stagger Easing
Apply easing to stagger values.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Delay(stagger => stagger
        .Value(100)
        .Easing(Easing.EaseOutQuad)
    )
    .AutoPlay(true));
```

#### Stagger Grid
For elements arranged in a grid.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .Scale(2)
    .Delay(stagger => stagger
        .Value(100)
        .Grid(new[] { 14, 7 }) // 14 columns, 7 rows
        .From(StaggerFrom.Center)
    )
    .AutoPlay(true));
```

#### Stagger Grid Axis
Control grid animation direction.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(270)
    .Delay(stagger => stagger
        .Value(100)
        .Grid(new[] { 14, 7 })
        .Axis(StaggerAxis.X) // Animate along X axis
    )
    .AutoPlay(true));
```

### Timeline
Create complex sequences of animations.
Official docs: https://animejs.com/v3/documentation/#timelineBasics
#### Creating a Timeline
```csharp
var timeline = await Anime.Timeline(props => props
    .Duration(750)
    .Easing(Easing.EaseOutExpo)
    .AutoPlay(false));
```

#### Adding Animations to Timeline
```csharp
var timeline = await Anime.Timeline(props => props
    .Easing(Easing.EaseOutExpo)
    .AutoPlay(false));

await timeline.AddAsync(t => t
    .Targets(".element-1")
    .TranslateX(250));

await timeline.AddAsync(t => t
    .Targets(".element-2")
    .TranslateX(250));

await timeline.AddAsync(t => t
    .Targets(".element-3")
    .TranslateX(250));

timeline.Play();
```

#### Timeline Offsets
Control when each animation starts.
```csharp
var timeline = await Anime.Timeline(props => props
    .AutoPlay(false));

// Default: start after previous animation ends
await timeline.AddAsync(t => t
    .Targets(".el1")
    .TranslateX(250));

// Relative offset: start 1000ms after previous
await timeline.AddAsync(t => t
    .Targets(".el2")
    .TranslateX(250), 
    "+=1000");

// Absolute offset: start at 2000ms
await timeline.AddAsync(t => t
    .Targets(".el3")
    .TranslateX(250), 
    "2000");

timeline.Play();
```

#### Timeline Controls
```csharp
timeline.Play();
timeline.Pause();
timeline.Restart();
timeline.Reverse();
timeline.Seek(1500); // Go to specific time
```

### Controls
Control your animations programmatically.
Official docs: https://animejs.com/v3/documentation/#playPause
#### Play / Pause
```csharp
var animation = await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .AutoPlay(false));

animation.Play();
animation.Pause();
```

#### Restart
Restart animation from the beginning.
```csharp
animation.Restart();
```

#### Reverse
Play animation in reverse.
```csharp
animation.Reverse();
```

#### Seek
Jump to a specific time (in milliseconds).
```csharp
animation.Seek(1500); // Go to 1500ms
```

#### Complete
Jump to the end of the animation.
```csharp
await animation.Complete();
```

### Callbacks
Execute code during animation lifecycle.
Official docs: https://animejs.com/v3/documentation/#update
#### Begin
Callback when animation begins.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Begin(OnBegin)
    .AutoPlay(true));

[JSInvokable]
public void OnBegin(AnimationState state)
{
    Console.WriteLine("Animation started!");
}
```

#### Update
Callback on every frame.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Update(OnUpdate)
    .AutoPlay(true));

[JSInvokable]
public void OnUpdate(AnimationState state)
{
    Console.WriteLine($"Progress: {state.Progress}%");
}
```

#### Complete
Callback when animation completes.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Complete(OnComplete)
    .AutoPlay(true));

[JSInvokable]
public void OnComplete(AnimationState state)
{
    Console.WriteLine("Animation completed!");
}
```

#### Loop Begin
Callback when a loop starts.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Loop(true)
    .LoopBegin(OnLoopBegin)
    .AutoPlay(true));

[JSInvokable]
public void OnLoopBegin(AnimationState state)
{
    Console.WriteLine($"Loop {state.CurrentLoop} started");
}
```

### SVG
Animate SVG elements with special properties.
Official docs: https://animejs.com/v3/documentation/#motionPath
#### Line Drawing
Animate SVG path strokes.
```csharp
await Anime.Animate(props => props
    .Targets(".line-drawing path")
    .StrokeDashoffset(0)
    .Easing(Easing.EaseInOutSine)
    .Duration(1500)
    .Direction(Direction.Alternate)
    .Loop(true)
    .AutoPlay(true));
```

#### SVG Morphing
Morph between different SVG shapes.
```csharp
await Anime.Animate(props => props
    .Targets(".morphing path")
    .D("M10 80 C 40 10, 65 10, 95 80 S 150 150, 180 80") // Target path
    .Duration(2000)
    .Easing(Easing.EaseInOutQuad)
    .AutoPlay(true));
```

#### SVG Motion Path
Animate element along an SVG path.
```csharp
await Anime.Animate(props => props
    .Targets(".motion-path-demo .el")
    .MotionPath(new MotionPathOptions
    {
        Path = ".motion-path-demo path",
        AlignAngle = true // Rotate element based on path direction
    })
    .Duration(2000)
    .Easing(Easing.EaseInOutSine)
    .Loop(true)
    .AutoPlay(true));
```

### Easings
Timing functions control animation acceleration.
Official docs: https://animejs.com/v3/documentation/#penner
#### Built-in Easing Functions
BlazorAnime includes all anime.js easing functions:
- `Linear`
- `EaseInSine`, `EaseOutSine`, `EaseInOutSine`, `EaseOutInSine`
- `EaseInQuad`, `EaseOutQuad`, `EaseInOutQuad`, `EaseOutInQuad`
- `EaseInCubic`, `EaseOutCubic`, `EaseInOutCubic`, `EaseOutInCubic`
- `EaseInQuart`, `EaseOutQuart`, `EaseInOutQuart`, `EaseOutInQuart`
- `EaseInQuint`, `EaseOutQuint`, `EaseInOutQuint`, `EaseOutInQuint`
- `EaseInExpo`, `EaseOutExpo`, `EaseInOutExpo`, `EaseOutInExpo`
- `EaseInCirc`, `EaseOutCirc`, `EaseInOutCirc`, `EaseOutInCirc`
- `EaseInBack`, `EaseOutBack`, `EaseInOutBack`, `EaseOutInBack`
- `EaseInElastic`, `EaseOutElastic`, `EaseInOutElastic`, `EaseOutInElastic`
- `EaseInBounce`, `EaseOutBounce`, `EaseInOutBounce`, `EaseOutInBounce`

#### Cubic Bezier
Define custom cubic-bezier curves.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Easing("cubicBezier(.5, .05, .1, .3)")
    .Duration(1000)
    .AutoPlay(true));
```

#### Spring
Physics-based spring easing.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(250)
    .Easing("spring(1, 80, 10, 0)") // mass, stiffness, damping, velocity
    .AutoPlay(true));
```

#### Elastic
Create elastic easing with custom amplitude and period.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .Scale(2)
    .Easing("easeOutElastic(1, .6)") // amplitude, period
    .Duration(1000)
    .AutoPlay(true));
```

### Helpers
Utility methods for common tasks.
Official docs: https://animejs.com/v3/documentation/#remove
#### Random
Generate random values for animations.
```csharp
// Random number between two values
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateX(Anime.Random(0, 250))
    .AutoPlay(true));
```

#### Stagger Helper
Use the stagger helper for delays and property values.
```csharp
await Anime.Animate(props => props
    .Targets(".element")
    .TranslateY(-30)
    .Delay(stagger => stagger.Value(100))
    .AutoPlay(true));
```

#### Get / Set
Get or set animatable values.
```csharp
// Get current value
var value = await Anime.Get(".element", "translateX");

// Set value
await Anime.Set(".element", "translateX", 250);
```

#### Remove
Remove elements from the DOM.
```csharp
await Anime.Remove(".element");
```
