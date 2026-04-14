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
### Values
### Keyframes
### Staggering
### Timeline
### Controls
### Callbacks
### SVG
### Easings
### Helpers


