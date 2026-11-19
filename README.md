# BlazorAnime

Typed Blazor bindings for [anime.js](https://animejs.com/) 4.5.0. The core package embeds the engine and exposes it as `IAnime`. Each extra module is its own package and loads that same engine. Durations are milliseconds. Blazor loads the script from the package.

> [!WARNING]
> The packages are `1.0.0-alpha`. The API can still change.

## Contents

- [Getting started](#getting-started)
  - [Install](#install)
  - [Example](#example)
- [BlazorAnime](#blazoranime)
  - [Service](#service)
  - [Playback](#playback)
  - [Timeline](#timeline)
  - [Parameters](#parameters)
  - [Properties](#properties)
  - [Stagger](#stagger)
  - [Easings](#easings)
  - [SVG](#svg)
- [Modules](#modules)
  - [Animatable](#animatable)
  - [Draggable](#draggable)
  - [Layout](#layout)
  - [Scope](#scope)
  - [Events](#events)
  - [Text](#text)
  - [WAAPI](#waapi)
- [Samples](#samples)
- [How to contribute](#how-to-contribute)

## Getting started

Create the animation in `OnAfterRenderAsync`, after the element exists. `DisposeAsync` pauses playback and releases the JavaScript instance, including callback handles. The namespace is `BlazorAnime`.

### Install

```bash
dotnet add package BlazorAnime --prerelease
```

```csharp
builder.Services.AddBlazorAnime();
```

```razor
@using BlazorAnime
```

WebAssembly hosts keep the framework script in `wwwroot/index.html`. Blazor Web App hosts keep it in `Components/App.razor`.

Install a module only when you use it. Register it beside `AddBlazorAnime()`.

| Package | Register | anime.js |
|---|---|---|
| `BlazorAnime` | `AddBlazorAnime()` | `animate`, `timeline`, `svg`, `easings`, `utils`, `engine` |
| `BlazorAnime.Animatable` | `AddBlazorAnimeAnimatable()` | `createAnimatable` |
| `BlazorAnime.Draggable` | `AddBlazorAnimeDraggable()` | `createDraggable` |
| `BlazorAnime.Layout` | `AddBlazorAnimeLayout()` | `createLayout` |
| `BlazorAnime.Scope` | `AddBlazorAnimeScope()` | `createScope` |
| `BlazorAnime.Events` | `AddBlazorAnimeEvents()` | `onScroll` |
| `BlazorAnime.Text` | `AddBlazorAnimeText()` | `splitText`, `scrambleText` |
| `BlazorAnime.Waapi` | `AddBlazorAnimeWaapi()` | `waapi.animate` |

### Example

```razor
@implements IAsyncDisposable
@inject IAnime Anime

<div @ref="element">Animate me!</div>
<button @onclick="Play">Play</button>

@code {
    private ElementReference element;
    private Animation? _animation;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _animation = await Anime.Animate(props => props
                .Targets(element)
                .TranslateX(250)
                .Rotate(360)
                .Duration(1500)
                .Ease(Easing.InOutQuad)
                .AutoPlay(false));
        }
    }

    private async Task Play()
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

## BlazorAnime

`IAnime` is the core service. A second `Animate` of the same property replaces the running tween (`composition: 'replace'`). `Set` writes the value immediately and leaves other tweens alone (`composition: 'none'`). An omitted ease is `out(2)`.

Playback methods resolve when the call is sent, so a looping animation returns immediately.

### Service

| Member | anime.js | Role |
|---|---|---|
| `Animate` | `animate` | Builds one animation from a `PropsBuilder`. |
| `CreateTimeline` | `createTimeline` | One clock for child animations. Autoplay, loop, and callbacks stay on the timeline. Duration, ease, and every other key are child defaults. |
| `Set` | `utils.set` | Writes values immediately. |
| `Get` | `utils.get` | Reads a computed value. A unit argument returns a number in that unit. |
| `Remove` | `utils.remove` | Drops targets from running animations. |
| `CreateObject` | plain object | JavaScript object you can target. `GetNumber` and `GetString` read it back. |
| `Random` | `utils.random` | Integer in a range. |
| `GetSpeed`, `SetSpeed` | `engine.speed` | Playback speed of the shared engine. |
| `PauseOnDocumentHidden` | `engine.pauseOnDocumentHidden` | Pauses while the tab is hidden. The engine default is `true`. |
| `Version` | version | The embedded engine. `4.5.0`. |

### Playback

`Animation` is the instance `Animate` returns. `Timeline` inherits these members.

| Member | anime.js | Role |
|---|---|---|
| `Play` | `play` | Starts a paused animation. |
| `Pause` | `pause` | Pauses. |
| `Resume` | `resume` | Continues in the current direction. |
| `Restart` | `restart` | Plays again from the initial values. |
| `Reverse` | `reverse` | Plays backward from the current time. |
| `Alternate` | `alternate` | Mirrors the time and flips direction. |
| `Reset` | `reset` | Returns to the initial values and pauses. |
| `Seek` | `seek` | Jumps to a time, in milliseconds. |
| `Progress` | `progress` | Jumps to a value from 0 to 1. |
| `Stretch` | `stretch` | Changes the duration. |
| `Refresh` | `refresh` | Re-reads function values, then refreshes. |
| `Complete` | `complete` | Seeks to the end and leaves the engine. |
| `Cancel` | `cancel` | Cancels. |
| `Revert` | `revert` | Restores the original values. |
| `Finished` | `then` | Completes when playback finishes. An infinite loop never completes. |
| `Remove` | `remove` | Removes targets from this animation. |
| `GetProgress`, `Began`, `Completed`, `Paused`, `Reversed`, `Backwards`, `IsAlternate` | `progress`, `began`, `completed`, `paused`, `reversed`, `backwards`, `alternate` | Current flags. |
| `Id`, `Loop`, `Duration`, `Delay`, `LoopDelay`, `CurrentTime`, `IterationProgress`, `CurrentIteration`, `TargetCount`, `CurrentValues` | the same fields | Current numbers. `Loop` is `-1` when playback repeats forever. |

Callbacks receive an `AnimationState` snapshot with those same fields. The live instance stays in JavaScript.

### Timeline

| Member | anime.js | Role |
|---|---|---|
| `AddAsync` | `add(targets, parameters, position)` | Adds a child. No position appends it. A number is an absolute time. `0` starts with the timeline. |
| `AddAsync` + `string` | `+=500`, `-=200`, `<`, `<<` | Position string, including overlap with the previous child. |
| `OffSet.Before`, `OffSet.After` | `-=`, `+=` | Relative to the end of the previous child. |
| `Sync` | `sync` | Places a WAAPI handle on the timeline. The handle is not a core `Animation`. |

### Parameters

These methods are on `PropsBuilder`. A number is unitless. A string keeps its unit (`"1rem"`, `"1turn"`). `Relative.Add`, `Subtract`, and `Multiply` are `+=`, `-=`, and `*=`. A nested builder sets `to`, `duration`, `delay`, and `ease` on that one property.

`Func<int, int, T>` receives the target index and the target count. `Func<TargetInfo, T>` receives `Index`, `Total`, `Id`, `TagName`, and `Dataset`. Both run once per target when the animation is created.

| Member | anime.js | Role |
|---|---|---|
| `Targets` | `targets` | CSS selector, `ElementReference`, `JsTarget`, `DrawableTarget`, or split-text handles. |
| `Duration` | `duration` | Milliseconds. |
| `Delay` | `delay` | Milliseconds, a stagger, or a per-target function. |
| `LoopDelay` | `loopDelay` | Pause between repeats, in milliseconds. |
| `Loop` | `loop` | `true` repeats forever. A count is extra plays after the first. `0` plays once. |
| `Alternate` | `alternate` | Flips direction on each repeat. |
| `Reversed` | `reversed` | Starts playing backward. |
| `AutoPlay` | `autoplay` | `false` creates the animation paused. A scroll-observer handle links playback and does not start it. |
| `Ease` | `ease` | An `Easing`. Omitted uses `out(2)`. |
| `ModifierRound` | `modifier` | Rounds to this many decimal places. `0` is an integer. |
| `From` | `from` | Start value. |
| `To` | `to` | End value. |
| `FromTo` | `to: [from, to]` | Start and end. |
| `Value` | `value` | The HTML attribute named `value`. |
| `Keyframes` | `keyframes` | A list of steps, or percentage keys such as `"0%"`. |
| `Prop` | any key | A name the typed methods do not cover, including `id`, `composition`, `frameRate`, `playbackRate`, and `playbackEase`. |
| `OnBegin`, `OnBeforeUpdate`, `OnUpdate`, `OnRender`, `OnLoop`, `OnPause`, `OnComplete` | `onBegin`, `onBeforeUpdate`, `onUpdate`, `onRender`, `onLoop`, `onPause`, `onComplete` | `AnimationState` snapshot. |

### Properties

Each setter accepts a number, a unit string, a `Relative`, a stagger, a per-target function, or a nested builder.

| Member | anime.js | Role |
|---|---|---|
| `TranslateX`, `TranslateY`, `TranslateZ` | `translateX`, `translateY`, `translateZ` | Shift on one axis. |
| `Rotate`, `RotateX`, `RotateY`, `RotateZ` | `rotate`, `rotateX`, `rotateY`, `rotateZ` | Turn. Degrees, or a string such as `"1turn"`. |
| `Scale`, `ScaleX`, `ScaleY`, `ScaleZ` | `scale`, `scaleX`, `scaleY`, `scaleZ` | Scale. |
| `SkewX`, `SkewY` | `skewX`, `skewY` | Skew. |
| `Perspective` | `perspective` | Perspective on the transform. |
| `TransformOrigin` | `transformOrigin` | Origin of the transform. |
| `Opacity` | `opacity` | Opacity. |
| `BackgroundColor`, `Color`, `BorderColor`, `OutlineColor`, `Fill` | `backgroundColor`, `color`, `borderColor`, `outlineColor`, `fill` | Color. |
| `Width`, `Height`, `MinWidth`, `MaxWidth`, `MinHeight`, `MaxHeight` | `width`, `height`, `minWidth`, `maxWidth`, `minHeight`, `maxHeight` | Box size. |
| `Top`, `Left`, `Right`, `Bottom` | `top`, `left`, `right`, `bottom` | Offset. |
| `MarginTop`, `MarginLeft`, `MarginRight`, `MarginBottom` | `marginTop`, `marginLeft`, `marginRight`, `marginBottom` | Margin. |
| `PaddingTop`, `PaddingLeft`, `PaddingRight`, `PaddingBottom` | `paddingTop`, `paddingLeft`, `paddingRight`, `paddingBottom` | Padding. |
| `BorderWidth`, `BorderRadius`, `OutlineWidth`, `OutlineOffset` | `borderWidth`, `borderRadius`, `outlineWidth`, `outlineOffset` | Border and outline. |
| `FontSize`, `FontWeight`, `LetterSpacing`, `LineHeight`, `WordSpacing`, `TextIndent` | `fontSize`, `fontWeight`, `letterSpacing`, `lineHeight`, `wordSpacing`, `textIndent` | Text metrics. |
| `BoxShadow`, `TextShadow`, `Filter` | `boxShadow`, `textShadow`, `filter` | Shadow and filter. |
| `ZIndex`, `Order` | `zIndex`, `order` | Stacking and flex order. |
| `FlexGrow`, `FlexShrink`, `FlexBasis`, `Gap`, `RowGap`, `ColumnGap` | `flexGrow`, `flexShrink`, `flexBasis`, `gap`, `rowGap`, `columnGap` | Flex layout. |
| `BackgroundPosition`, `BackgroundSize`, `ClipPath` | `backgroundPosition`, `backgroundSize`, `clipPath` | Background and clip. |
| `X`, `Y`, `Cx`, `Cy`, `R`, `Rx`, `Ry` | `x`, `y`, `cx`, `cy`, `r`, `rx`, `ry` | SVG geometry. |
| `D` | `d` | SVG path data. A `morphTo` function morphs into another shape. |
| `Points` | `points` | Polygon points, or a `morphTo` function. |
| `Stroke`, `StrokeWidth`, `StrokeDashoffset` | `stroke`, `strokeWidth`, `strokeDashoffset` | SVG stroke. |
| `Draw` | `draw` | Line drawing on a `DrawableTarget`. `"0 1"` draws the whole shape. |

### Stagger

`Stagger.Create` is `stagger()`. Pass a delay, a from/to range, or a value plus options. `Delay`, `Duration`, and `LoopDelay` also take `stagger => stagger.Value(...).From(...)`.

| Member | anime.js | Role |
|---|---|---|
| `Start` | `start` | Added to every delay. |
| `Reversed` | `reversed` | Walks the targets backward. |
| `Ease` | `ease` | Distributes the range. |
| `Grid` | `grid` | `[columns, rows]`. |
| `Axis` | `axis` | `StaggerAxis.X` or `StaggerAxis.Y`. |
| `From` | `from` | `StaggerFrom.First`, `Last`, `Center`, or `Index`. |

### Easings

Named curves use the anime.js string: `Easing.InOutQuad` is `inOutQuad`. The set is `In`, `Out`, `InOut`, and `OutIn` combined with `Quad`, `Cubic`, `Quart`, `Quint`, `Sine`, `Expo`, `Circ`, `Back`, and `Bounce`, plus `Linear`.

| Member | anime.js | Role |
|---|---|---|
| `In`, `Out`, `InOut` | `in(power)`, `out(power)`, `inOut(power)` | Power curves. |
| `InElastic`, `OutElastic`, `InOutElastic`, `OutInElastic` | `inElastic`, `outElastic`, `inOutElastic`, `outInElastic` | Amplitude defaults to 1. Period defaults to 0.3. |
| `Spring` | `spring` | Mass, stiffness, damping, and velocity, or bounce plus a duration. |
| `CubicBezier` | `cubicBezier(x1, y1, x2, y2)` | Cubic bezier. |
| `Steps` | `steps(n)` | Discrete steps. |
| `Curve` | sampled function | A .NET function, sampled. The default is 64 samples. |
| `Raw` | the string you pass | An ease string sent as-is. |

### SVG

| Member | anime.js | Role |
|---|---|---|
| `CreateDrawable` | `svg.createDrawable` | Proxies for geometry. Pass the result to `Targets` and animate `Draw`. |
| `CreateMotionPath` | `svg.createMotionPath` | Functions for a path, polygon, or polyline. `offset` is 0–1 along the length. |
| `MotionPath.TranslateX`, `TranslateY`, `Rotate` | `translateX`, `translateY`, `rotate` | Path position and tangent. Pass them to the matching property. |
| `MorphTo` | `svg.morphTo` | A function for `D` or `Points`. Precision `0` copies the shape. The default is `0.33`. |

## Modules

Each package below registers its own service and imports the core engine.

### Animatable

`IAnimatable.Create` is `createAnimatable`. A property is a duration in milliseconds, a `Stagger`, or a settings object. Calling `Set` later animates to a new number. anime.js forces `composition: 'replace'` and `autoplay: false` on every property.

#### Settings

| Member | anime.js | Role |
|---|---|---|
| `Property` | the property name | Duration, or unit, ease, and modifier for one property such as `x` or `backgroundColor`. |
| `Duration` | `duration` | Default duration, in milliseconds. |
| `Ease` | `ease` | Default ease. |
| `Unit` | `unit` | Unit appended to the number, such as `rem`. |
| `ModifierRound` | `modifier` | Rounds to this many decimal places. A property modifier wins. |
| `ModifierSnap` | `modifier` | Snaps to a multiple. |
| `ModifierInvert` | `modifier` | Negates the value. `Get` returns the negated number. |
| `OnBegin` … `OnComplete` | `onBegin` … `onComplete` | `AnimationState` snapshot. |

#### Instance

| Member | anime.js | Role |
|---|---|---|
| `Get` | getter | The current number. |
| `GetValues` | getter | A color or any other list of numbers. |
| `Set` | setter | Animates to a number or a list. An optional duration and ease apply to that call. |
| `Targets` | `targets` | Resolved elements. Kept until `Revert` or disposal. |
| `Animations` | property animations | One `Animation` per property name. |
| `Revert` | `revert` | Restores the original values. |

### Draggable

`IDraggable.Create` is `createDraggable`. The result is a drag instance. An empty builder drags both axes, mapped to `translateX` and `translateY`. Commands resolve when sent. `DisposeAsync` reverts the drag and drops the pointer listeners.

#### Settings

| Member | anime.js | Role |
|---|---|---|
| `X`, `Y` | `x`, `y` | `false` locks that axis. A nested `DraggableAxis` sets map, modifier, composition, and snap. |
| `MapTo` | `mapTo` | CSS property the axis writes, such as `rotate`. |
| `Composition` | `composition` | `DragComposition.Replace`, `None`, or `Blend`. |
| `ModifierRound`, `ModifierSnap`, `ModifierInvert` | `modifier` | Round, snap, or negate. An axis modifier wins on that axis. |
| `Trigger` | `trigger` | Element that receives the pointer. The dragged element is the default. |
| `Container` | `container` | An element, or bounds in pixels: top, right, bottom, left. |
| `ContainerPadding` | `containerPadding` | One padding, or top, right, bottom, left. |
| `ContainerFriction` | `containerFriction` | How far a drag can pass the edge while the pointer is down. 0 to 1. |
| `ReleaseContainerFriction` | `releaseContainerFriction` | How far the release can pass the edge. 0 to 1. |
| `DragSpeed` | `dragSpeed` | Multiplier on pointer movement. |
| `DragThreshold` | `dragThreshold` | Pixels before a press becomes a drag. A pair is mouse and touch. |
| `ScrollSpeed`, `ScrollThreshold` | `scrollSpeed`, `scrollThreshold` | Automatic scrolling near the edge. |
| `MinVelocity`, `MaxVelocity`, `VelocityMultiplier` | `minVelocity`, `maxVelocity`, `velocityMultiplier` | Release velocity. |
| `ReleaseMass`, `ReleaseStiffness`, `ReleaseDamping` | `releaseMass`, `releaseStiffness`, `releaseDamping` | The default release spring. |
| `ReleaseEase` | `releaseEase` | Ease when the pointer lets go. A spring replaces the default spring. |
| `Cursor` | `cursor` | `true` uses grab and grabbing. A pair sets `onHover` and `onGrab`. |
| `Snap` | `snap` | A pixel grid, or a list of points. An axis snap wins on that axis. |
| `OnGrab`, `OnDrag`, `OnRelease`, `OnUpdate`, `OnSettle`, `OnSnap`, `OnResize`, `OnAfterResize` | the same names | A `DragState` snapshot: position, progress, velocity, angle, and grab flags. |

#### Instance

| Member | anime.js | Role |
|---|---|---|
| `Disable`, `Enable` | `disable`, `enable` | Ignore or accept the pointer. |
| `SetX`, `SetY` | `setX`, `setY` | Move an axis. The mute flag skips `onUpdate`. |
| `Stop` | `stop` | Stops the release. |
| `Reset` | `reset` | Stops, clears velocity, and returns both axes to 0. |
| `Refresh` | `refresh` | Re-reads container, snap, and cursor. |
| `Revert` | `revert` | Removes listeners and restores styles. |
| `ScrollInView`, `AnimateInView` | `scrollInView`, `animateInView` | Bring the target into view. A null argument keeps the anime.js default. |
| `X`, `Y`, `ProgressX`, `ProgressY`, `Velocity`, `Angle`, `DestX`, `DestY`, `DeltaX`, `DeltaY` | the same fields | Current numbers. |
| `Enabled`, `Grabbed`, `Dragged`, `Contained` | the same fields | Current flags. |
| `XProp`, `YProp` | the mapped property | Property name each axis writes. |
| `Target`, `Trigger`, `Container`, `ScrollContainer` | `$target`, `$trigger`, `$container`, `$scrollContainer` | Resolved elements. |

### Layout

`ILayout.Create` is `createLayout`. `Record` measures the current DOM. Change the markup, let Blazor render, then call `Animate` from `OnAfterRenderAsync`. `Record`, `Animate`, and `Revert` resolve when the call is sent.

#### Settings

| Member | anime.js | Role |
|---|---|---|
| `Children` | `children` | Elements this layout tracks. Omitted tracks every child of the root. |
| `Duration` | `duration` | Transition length, in milliseconds. |
| `Delay` | `delay` | A number or a `Stagger`. |
| `Ease` | `ease` | Ease of the transition. |
| `Properties` | `properties` | Extra CSS names interpolated when their computed values change. |
| `EnterFrom` | `enterFrom` | State for elements that appear. Replaces the default opacity of 0. |
| `LeaveTo` | `leaveTo` | State for elements that leave. |
| `SwapAt` | `swapAt` | State while elements swap. |
| `Loop`, `Alternate`, `Reversed`, `LoopDelay`, `Autoplay` | `loop`, `alternate`, `reversed`, `loopDelay`, `autoplay` | Playback of the layout timeline. |
| `PlaybackEase` | `playbackEase` | Ease of the timeline, not of each element. |
| `Id` | `id` | Layout id. `Layout.Id` reads it. |
| `OnBegin` … `OnRender` | `onBegin` … `onRender` | `AnimationState` snapshot. |

#### Instance

| Member | anime.js | Role |
|---|---|---|
| `Record` | `record` | Measures the current DOM and keeps it as the previous state. |
| `Animate` | `animate` | Measures again and plays. Settings on this call override `Create` for this run. |
| `Revert` | `revert` | Completes the transition and restores measured styles. |
| `Root` | `root` | The root element. |
| `Entering`, `Leaving`, `Swapping`, `Animating` | the same lists | Elements from the latest `Animate`. |

`EnterFrom`, `LeaveTo`, and `SwapAt` take `Opacity`, `Duration`, `Delay`, `Ease`, or `Property(name, value)`.

### Scope

`IScope.Create` is `createScope`. `Add` enters the scope, awaits the constructor, then leaves. Animations and module instances created during that await register on the scope and pick up its root and defaults. `Refresh` and `Revert` resolve when the rebuild or teardown has finished. Time on the rebuilt instances starts over.

#### Settings

| Member | anime.js | Role |
|---|---|---|
| `Root` | `root` | Selectors resolve inside this element. Omitted uses the document. |
| `MediaQuery` | `mediaQueries` | A name plus a query such as `"(max-width: 200px)"`. |
| `Defaults` | `defaults` | Duration, delay, loop, ease, playback rate, frame rate, and composition for instances created inside the scope. |

`Composition` on the defaults is `none`, `replace`, or `blend`.

#### Instance

| Member | anime.js | Role |
|---|---|---|
| `Add` | `add` | Runs now, and again on `Refresh` and when a media query changes. |
| `AddOnce` | `addOnce` | Runs once. Its instances stay across `Refresh`. |
| `AddMethod` | `add(name, fn)` | A named method that runs inside the scope. |
| `Invoke` | the method | Enters the scope and runs that method. `ScopeCall` carries `ClientX` and `ClientY`. |
| `Refresh` | `refresh` | Reverts repeating instances and runs each `Add` constructor again. |
| `Revert` | `revert` | Reverts every instance and drops the media listeners. |
| `Matches` | `matches` | The latest media-query map. |
| `Root` | `root` | The root element. |
| `ScopeContext.OnCleanup` | cleanup | Runs after repeating instances revert, before the constructor runs again. |
| `ScopeContext.State` | scope data | A string-to-int map. `Get` and `Set` survive `Refresh` and clear on `Revert`. |

### Events

`IEvents.OnScroll` is `onScroll`. Pass `Handle` to `AutoPlay`. anime.js links the animation, timer, or timeline and leaves it paused. `Refresh`, `Revert`, and `Link` resolve when finished.

#### Settings

| Member | anime.js | Role |
|---|---|---|
| `Container` | `container` | The scrolling element. |
| `Target` | `target` | The element whose position drives the observer. |
| `Axis` | `axis` | `"x"` or `"y"`. |
| `Debug` | `debug` | Draws the debug markers. |
| `Repeat` | `repeat` | Runs again each time the target enters. |
| `Enter`, `Leave` | `enter`, `leave` | A position, a number, or a target and container pair. |
| `Sync` | `sync` | `true` seeks from scroll progress. A method string such as `"play pause"` calls those methods on enter and leave. A number eases the link. An `Easing` does the same. |
| `OnEnter`, `OnEnterForward`, `OnEnterBackward`, `OnLeave`, `OnLeaveForward`, `OnLeaveBackward`, `OnUpdate`, `OnSyncComplete`, `OnResize` | the same names | A `ScrollState` snapshot: progress, in view, direction, velocity, and scroll. |

#### Instance

| Member | anime.js | Role |
|---|---|---|
| `Handle` | the observer | Pass this to `AutoPlay`. |
| `Link` | `link` | Attaches an animation that was created without this observer. |
| `Refresh` | `refresh` | Re-reads positions. |
| `Revert` | `revert` | Removes the observer. |
| `Progress`, `IsInView`, `Repeat`, `Horizontal` | the same fields | Current values. |

### Text

`IText` binds `splitText` and `scrambleText`. A split returns element handles for `Targets`. Scramble is the function `InnerHtml` assigns to `innerHTML`. An omitted `Words` value still splits words. `Refresh` and `Revert` resolve when finished. `Refresh` replaces `Lines`, `Words`, and `Chars`. `DisposeAsync` reverts the split.

#### Split

| Member | anime.js | Role |
|---|---|---|
| `Split` | `splitText` | Splits a selector or an element. |
| `Lines`, `Words`, `Chars` | `lines`, `words`, `chars` | `true` or `false`, an HTML template, or class, wrap, and clone. |
| `Class` | `class` | A class name. `false` adds none. |
| `Wrap` | `wrap` | `true`, `false`, or `hidden`, `clip`, `visible`, `scroll`, `auto`. |
| `Clone` | `clone` | `true`, `false`, or `top`, `right`, `bottom`, `left`, `center`. |
| `IncludeSpaces` | `includeSpaces` | Keeps spaces as split elements. |
| `Accessible` | `accessible` | Keeps the original text available to assistive tech. |
| `Debug` | `debug` | Outlines the split elements. |

#### Split result

| Member | anime.js | Role |
|---|---|---|
| `Lines`, `Words`, `Chars` | `lines`, `words`, `chars` | Handles. Pass them to `Targets`. |
| `Refresh` | `refresh` | Splits again and replaces the handles. |
| `Revert` | `revert` | Restores the original markup. |

#### Scramble

| Member | anime.js | Role |
|---|---|---|
| `Scramble` | `scrambleText` | The function `InnerHtml` assigns to `innerHTML`. |
| `Text` | `text` | The string being revealed. |
| `Chars` | `chars` | The glyph set used while scrambling. |
| `From` | `from` | `"left"`, `"center"`, `"right"`, `"random"`, `"auto"`, or an index. |
| `Reversed` | `reversed` | Reveals from the other end. |
| `Cursor` | `cursor` | `true`, a character, or a character code. |
| `Perturbation` | `perturbation` | 0 to 1. |
| `Seed` | `seed` | Stable scramble. |
| `Override` | `override` | `true`, or a string that replaces the glyphs. |
| `RevealRate`, `RevealDelay` | `revealRate`, `revealDelay` | Characters per second, and the wait before the first one. |
| `SettleDuration`, `SettleRate` | `settleDuration`, `settleRate` | How each character settles. |
| `Duration`, `Delay`, `Ease` | `duration`, `delay`, `ease` | Timing of the scramble. |
| `OnChange` | `onChange` | The current string and the progress. |
| `InnerHtml` | `innerHTML` | Puts the scramble function on a text property. |

### WAAPI

`IWaapi.Animate` is `waapi.animate`. The result runs on the browser timeline, at its own `playbackRate`. Pass `Handle` to `Timeline.Sync`. Playback methods resolve when the call is sent. `DisposeAsync` reverts the animation. `ConvertEase` is `waapi.convertEase` and returns a CSS `linear()` string.

#### Settings

| Member | anime.js | Role |
|---|---|---|
| `Animate` | `waapi.animate` | A selector, a list, or an `ElementReference`. |
| `Duration`, `Delay` | `duration`, `delay` | Milliseconds, or a per-target function. `Delay` also takes a `Stagger`. |
| `Loop`, `Alternate`, `Reversed`, `AutoPlay` | `loop`, `alternate`, `reversed`, `autoplay` | Playback. `AutoPlay` also accepts a scroll-observer handle. |
| `Persist` | `persist` | Keeps the final styles. |
| `PlaybackRate` | `playbackRate` | Speed. `Speed` reads it back. |
| `Composition` | `composition` | `replace`, `add`, or `accumulate`. Omitted stays `replace`. |
| `Ease` | `ease` | An `Easing`, converted for the browser. |
| `OnComplete` | `onComplete` | A `WaapiState` snapshot: time, progress, duration, and playback flags. |
| `X`, `Y`, `Translate`, `TranslateX`, `TranslateY`, `Rotate`, `Scale`, `Opacity` | the same names | A number, a unit string, or a `WaapiTween`. |
| `Property` | any CSS name | A value the typed methods do not cover. |
| `To`, `From` | `to`, `from` | Ends and starts of one `WaapiTween`. |

#### Instance

| Member | anime.js | Role |
|---|---|---|
| `Handle` | the animation | The object `Timeline.Sync` accepts. |
| `Play`, `Pause`, `Resume`, `Reverse`, `Alternate`, `Restart`, `Seek`, `Complete`, `Cancel`, `Revert`, `CommitStyles` | the same methods | Playback. `Seek` is milliseconds. |
| `Finished` | `finished` | Completes when playback finishes. An infinite loop never completes. |
| `Duration`, `CurrentTime`, `Progress`, `Speed`, `Paused`, `Completed`, `Reversed`, `Persist`, `Targets` | the same fields | Current values. |
| `SetSpeed` | `playbackRate` | Changes the speed. |

## Samples

| Demo | What it shows |
|---|---|
| [Examples](https://olaboutcode.github.io/blazor-anime/examples/) | The official anime.js documentation examples. [Source](https://github.com/olaboutcode/blazor-anime/tree/master/samples/Examples). |
| [Fun examples](https://olaboutcode.github.io/blazor-anime/fun/) | Custom and fun examples. [Source](https://github.com/olaboutcode/blazor-anime/tree/master/samples/FunExamples). |
| [Page transitions](https://olaboutcode.github.io/blazor-anime/travel/) | Full-page animations example. [Source](https://github.com/olaboutcode/blazor-anime/tree/master/samples/PageTransitions). |

## How to contribute

The solution is `BlazorAnime.slnx` and targets .NET 10 and later. Package versions live in `Directory.Packages.props`.

```bash
dotnet build BlazorAnime.slnx
dotnet test test/BlazorAnime.Tests/BlazorAnime.Tests.csproj
node --test test/interop/*.cjs
dotnet test test/BlazorAnime.UiTests/BlazorAnime.UiTests.csproj
```

The browser tests need Playwright's Chromium driver. The UI test project downloads it on build.

The engine is built under `src/BlazorAnime/anime`. npm stays in that folder.

```bash
npm ci --prefix src/BlazorAnime/anime
npm run build --prefix src/BlazorAnime/anime
```

Run a sample with `dotnet run --project samples/Examples` (`http://localhost:5161`), `samples/FunExamples` (`http://localhost:5181`), or `samples/PageTransitions` (`http://localhost:5175`).

`scripts/publish-pages.sh` builds the sample site locally. `scripts/publish-pages.bat` does the same on Windows. A push to `master` publishes that site.

Open an issue or a pull request at [github.com/olaboutcode/blazor-anime](https://github.com/olaboutcode/blazor-anime).

## License

MIT
