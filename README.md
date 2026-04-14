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
                .Easing(Easing.EaseInOutQuad))
                .AutoPlay(false);
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


