namespace Examples;

public sealed record DemoEntry(
    string Slug,
    string Title,
    string Summary,
    string Details,
    string Code,
    string Source);

public sealed record DemoGroup(string? Title, IReadOnlyList<DemoEntry> Entries);

public sealed record DemoSection(string Id, string Title, IReadOnlyList<DemoGroup> Groups)
{
    public IEnumerable<DemoEntry> Entries => Groups.SelectMany(group => group.Entries);
}

public static class DemoCatalog
{
    public static IReadOnlyList<DemoSection> Sections { get; } =
    [
        new("animation", "Animation",
        [
            Group("Tween values",
            Entry("relative-values", "Relative values",
                "Move, resize, and rotate from the value the element already has.",
                "Relative.Multiply, Relative.Subtract, and Relative.Add read the current computed value when the animation is created. A unit string such as \"2turn\" is passed through to anime.js.",
                """
                await Anime.Animate(props => props
                    .Targets(".demo-relative-values .el")
                    .TranslateX(property => property.To(Relative.Multiply(2.5)).Duration(1000))
                    .Width(property => property.To(Relative.Subtract(20)).Duration(1800))
                    .Rotate(property => property.To(Relative.Add("2turn")).Duration(1800))
                    .Alternate(true)
                    .Loop(true));
                """,
                "Components/RelativeValues.razor"),
            Entry("function-based-values", "Function based values",
                "Each target gets its own value from its index or its data attributes.",
                "TargetInfo is a snapshot taken when the animation is built. data-x becomes Dataset[\"x\"]. Index callbacks receive the target index and the target count. The functions are not called again on later frames.",
                """
                await Anime.Animate(props => props
                    .Targets(".function-based-values-demo .el")
                    .TranslateX(target => double.Parse(target.Dataset.GetValueOrDefault("x", "0")))
                    .TranslateY((index, count) => 50 + (-50 * index))
                    .Scale((index, count) => (count - index) + .25)
                    .Alternate(true)
                    .Loop(true));
                """,
                "Components/FunctionBasedValues.razor")),
            Leaves(
            Entry("function-based-parameters", "Function based parameters",
                "Delay can be a function of the target index.",
                "Lambdas and ordinary methods both work. The function runs once per target while the animation is created, then anime.js uses the returned number.",
                """
                await Anime.Animate(props => props
                    .Targets(".function-based-parameters-demo .el")
                    .TranslateX(180)
                    .Delay((index, total) => index * 120)
                    .Duration(750)
                    .Alternate(true)
                    .Loop(true));
                """,
                "Components/FunctionBasedParameters.razor")),
            Group("Keyframes",
            Entry("keyframes", "Keyframes",
                "One property walks through several steps.",
                "Pass the steps as an array of builders. A comma-separated list of lambdas binds to the target-info overload instead of a keyframe sequence.",
                """
                await Anime.Animate(props => props
                    .Targets(".animation-keyframes-demo .el")
                    .Keyframes([
                        step => step.TranslateY(-40),
                        step => step.TranslateX(250),
                        step => step.TranslateY(40),
                        step => step.TranslateX(0),
                        step => step.TranslateY(0)
                    ])
                    .Duration(4000)
                    .Ease(Easing.OutElastic(1, .8))
                    .Loop(true));
                """,
                "Components/Keyframes.razor"),
            Entry("property-keyframes", "Property keyframes",
                "Keyframes can live on a single property.",
                "Each step uses To. A step can set its own duration, delay, and ease. Ease on the animation covers steps that leave ease unset.",
                """
                await Anime.Animate(props => props
                    .Targets(".property-keyframes-demo .el")
                    .TranslateX([
                        step => step.To(250).Duration(1000).Delay(500),
                        step => step.To(0).Duration(1000).Delay(500)
                    ])
                    .ScaleX([
                        step => step.To(4).Duration(100).Delay(500).Ease(Easing.OutExpo),
                        step => step.To(1).Duration(900)
                    ])
                    .Ease(Easing.OutElastic(1, .8))
                    .Loop(true));
                """,
                "Components/PropertyKeyframes.razor")),
            Leaves(
            Entry("callbacks", "Callbacks",
                "Begin, update, loop, and complete receive a snapshot.",
                "OnBegin runs after the delay. OnUpdate reports progress from 0 to 1. OnLoop runs when a repeat begins. Loop(2) is two extra repeats, and Alternate(true) flips direction between them. OnComplete runs after those repeats finish.",
                """
                await Anime.Animate(props => props
                    .Targets(".animation-callbacks-demo .el")
                    .TranslateX(270)
                    .Delay((index, total) => index * 500)
                    .Duration(500)
                    .Alternate(true)
                    .OnBegin(state => Show("Began"))
                    .OnUpdate(state => Show(state.Progress))
                    .OnLoop(state => Show("Looped"))
                    .OnComplete(state => Show("Complete"))
                    .Loop(2));
                """,
                "Components/AnimationCallbacks.razor"),
            Entry("controls", "Controls",
                "Play, pause, resume, seek, restart, reverse, alternate, and complete.",
                "AutoPlay(false) leaves the animation paused. Seek takes milliseconds. Progress is 0 to 1. Resume continues in the current direction. Alternate mirrors the current time and flips direction. Complete seeks to the end and removes the animation from the engine. Finished does not complete while Loop(true) is set.",
                """
                var animation = await Anime.Animate(props => props
                    .Targets(".animation-controls-demo .el")
                    .Prop("translateX", 250)
                    .AutoPlay(false));

                await animation.Play();
                await animation.Pause();
                await animation.Resume();
                await animation.Seek(400);
                await animation.Reverse();
                await animation.Alternate();
                await animation.Restart();
                await animation.Complete();
                """,
                "Components/AnimationControls.razor"))
        ]),
        new("timeline", "Timeline",
        [
            Leaves(
            Entry("timeline-basics", "Basics",
                "A timeline plays child animations with shared defaults.",
                "Duration and ease set on the timeline become defaults for each child that does not set its own. AddAsync appends a child. The timeline is itself an animation, so play, pause, and dispose are the same methods.",
                """
                var timeline = await Anime.CreateTimeline(props => props
                    .Duration(750)
                    .Ease(Easing.OutExpo));

                await timeline.AddAsync(child => child
                    .Targets(".timeline-basics-demo .el.square")
                    .TranslateX(250));
                """,
                "Components/TimelineBasics.razor"),
            Entry("timeline-offsets", "Offsets",
                "Place children at a time, or relative to the previous child.",
                "A number is an absolute time in milliseconds, and 0 starts with the timeline. OffSet.Before and OffSet.After shift from the end of the previous child. \"+=500\" matches OffSet.After(500). \"<\" starts with the previous child, and \"<<\" starts with the child before that.",
                """
                await timeline.AddAsync(child => child
                    .Targets(".timeline-offset-demo .el.square")
                    .TranslateX(250));
                await timeline.AddAsync(child => child
                    .Targets(".timeline-offset-demo .el.circle")
                    .TranslateX(250), OffSet.Before(600));
                await timeline.AddAsync(child => child
                    .Targets(".timeline-offset-demo .el.triangle")
                    .TranslateX(250), 400);
                """,
                "Components/TimelineOffset.razor"),
            Entry("timeline-inheritance", "Inheritance",
                "Children inherit duration and ease, and one child can override them.",
                "The square inherits 750ms and OutExpo. The circle starts at the same time, position 0, and overrides the ease with Linear.",
                """
                var timeline = await Anime.CreateTimeline(props => props
                    .Duration(750)
                    .Ease(Easing.OutExpo));

                await timeline.AddAsync(child => child
                    .Targets(".timeline-inheritance-demo .el.square")
                    .TranslateX(250));
                await timeline.AddAsync(child => child
                    .Targets(".timeline-inheritance-demo .el.circle")
                    .TranslateX(250)
                    .Ease(Easing.Linear), 0);
                """,
                "Components/TimelineInheritance.razor"),
            Entry("timeline-controls", "Controls",
                "A timeline pauses, seeks, and restarts as one animation.",
                "Call Play, Pause, Seek, or Restart on the timeline. AutoPlay(false) is playback on the timeline, not a default copied onto each child.",
                """
                var timeline = await Anime.CreateTimeline(props => props
                    .Duration(750)
                    .Ease(Easing.OutExpo)
                    .AutoPlay(false));

                await timeline.AddAsync(child => child
                    .Targets(".timeline-controls-demo .el.square")
                    .TranslateX(250));
                await timeline.Play();
                """,
                "Components/TimelineControls.razor"))
        ]),
        new("animatable", "Animatable",
        [
            Leaves(
            Entry("animatable", "Animatable",
                "Update x and y as the cursor moves, without creating a new animation each time.",
                "Property(\"x\", 500) sets that property's duration to 500ms. Ease applies to every property. Set starts a transition toward the cursor. The value is clamped to the demo.",
                """
                var square = await Animatable.Create(".square", props => props
                    .Property("x", 500)
                    .Property("y", 500)
                    .Ease(Easing.Out(3)));

                await square.Set("x", x);
                await square.Set("y", y);
                """,
                "Components/AnimatableFollow.razor")),
            Group("Settings",
            Entry("animatable-unit", "unit",
                "Rotate a clock by passing radians.",
                "Unit(\"rad\") is the CSS unit for that property. Duration(400) applies only to rotate. The angle is unwrapped so the hand does not take the long way around.",
                """
                var clock = await Animatable.Create(".clock", props => props
                    .Property("rotate", property => property.Unit("rad").Duration(400)));

                await clock.Set("rotate", angle);
                """,
                "Components/AnimatableUnit.razor"),
            Entry("animatable-duration", "duration",
                "Give x no transition and stagger the y duration from the center.",
                "A number on a property is its duration in milliseconds. 0 jumps to the new value. The y stagger starts at 200ms in the center and adds 200ms toward the ends.",
                """
                var circles = await Animatable.Create(".circle", props => props
                    .Property("x", 0)
                    .Property("y", Stagger.Create(200, stagger => stagger
                        .From(StaggerPosition.Center)
                        .Start(200)))
                    .Ease(Easing.Out(4)));

                await circles.Set("x", x);
                await circles.Set("y", y);
                """,
                "Components/AnimatableDuration.razor"),
            Entry("animatable-ease", "ease",
                "One clock eases in a straight line, the other overshoots.",
                "Ease on the animatable is the default for every property. Linear keeps a constant rate. OutElastic matches outElastic. Each hand tracks the cursor from its own center.",
                """
                var linear = await Animatable.Create(".clock-1", props => props
                    .Property("rotate", property => property.Unit("rad"))
                    .Ease(Easing.Linear));
                var elastic = await Animatable.Create(".clock-2", props => props
                    .Property("rotate", property => property.Unit("rad"))
                    .Ease(Easing.OutElastic()));
                """,
                "Components/AnimatableEase.razor"),
            Entry("animatable-modifier", "modifier",
                "Snap one hand to steps of π/10 and mirror the other.",
                "Duration 0 writes the number immediately. ModifierSnap rounds the angle to the nearest π/10 inside anime.js. ModifierInvert negates it. Get returns that modified number. Targets is the clock element the pointer is measured against.",
                """
                var snapped = await Animatable.Create(".clock-1", props => props
                    .Property("rotate", property => property.Unit("rad"))
                    .ModifierSnap(Math.PI / 10)
                    .Duration(0));
                var inverted = await Animatable.Create(".clock-2", props => props
                    .Property("rotate", property => property.Unit("rad"))
                    .ModifierInvert()
                    .Duration(0));

                var clock = (await snapped.Targets())[0];
                var angle = Math.Atan2(y, x) + Math.PI / 2;
                await snapped.Set("rotate", angle);
                await inverted.Set("rotate", angle);
                """,
                "Components/AnimatableModifier.razor")),
            Group("Methods",
            Entry("animatable-getters", "Getters",
                "Read x and y while the circle follows the cursor.",
                "Get returns the current number. OnRender runs while the transition is moving, and that is when the readout is refreshed. A color or any other list uses GetValues.",
                """
                var circle = await Animatable.Create(".circle", props => props
                    .Property("x", 500)
                    .Property("y", 500)
                    .Ease(Easing.Out(2))
                    .OnRender(_ => _ = Show()));

                async Task Show()
                {
                    var x = await circle.Get("x");
                    var y = await circle.Get("y");
                }
                """,
                "Components/AnimatableGetters.razor"),
            Entry("animatable-setters", "Setters",
                "Override the duration and ease for one call, and pass a color as numbers.",
                "The first Set calls install 500ms out(2) on x, 500ms out(3) on y, and 250ms on the color. Later calls keep those settings. Red and blue follow the horizontal position. Green stays 255. The fourth number is the alpha, so the circle stays visible.",
                """
                await circle.Set("x", 0, 500, Easing.Out(2));
                await circle.Set("y", 0, 500, Easing.Out(3));
                await circle.Set("backgroundColor", new[] { 164d, 255, 79, 1 }, 250);

                await circle.Set("x", x);
                await circle.Set("y", y);
                await circle.Set("backgroundColor", new[] { red, 255d, blue, 1 });
                """,
                "Components/AnimatableSetters.razor"),
            Entry("animatable-revert", "revert()",
                "Put the circles back and stop following the cursor.",
                "Revert restores the original values and removes the property methods. The button calls it once. DisposeAsync pauses instead, and leaves the values where they are.",
                """
                var circles = await Animatable.Create(".circle", props => props
                    .Property("x", Stagger.Create(50, stagger => stagger
                        .From(StaggerPosition.Center)
                        .Start(100)))
                    .Property("y", Stagger.Create(200, stagger => stagger
                        .From(StaggerPosition.Center)
                        .Start(200)))
                    .Ease(Easing.Out(4)));

                await circles.Revert();
                """,
                "Components/AnimatableRevert.razor"))
        ]),
        new("draggable", "Draggable",
        [
            Leaves(
            Entry("draggable", "Draggable",
                "Drag a square with the pointer.",
                "Create with no settings enables both axes. The stage clips the square. Dispose reverts the drag so the pointer listeners do not stay.",
                """
                var square = await Draggable.Create(".square");
                """,
                "Components/DraggableBasic.razor")),
            Group("Settings",
            Entry("draggable-axes", "Axes",
                "Lock one square to vertical and the other to horizontal.",
                "X(false) disables the horizontal axis. Y(false) disables the vertical axis. An omitted axis stays enabled.",
                """
                var vertical = await Draggable.Create(".vertical", props => props.X(false));
                var horizontal = await Draggable.Create(".horizontal", props => props.Y(false));
                """,
                "Components/DraggableAxes.razor"),
            Entry("draggable-container", "container",
                "Keep the square inside the stage.",
                "Container is the element the drag cannot leave. ContainerPadding insets that box by 16 pixels on every side.",
                """
                var square = await Draggable.Create(".square", props => props
                    .Container(".stage")
                    .ContainerPadding(16));
                """,
                "Components/DraggableContainer.razor"),
            Entry("draggable-snap", "snap",
                "Move the square in steps of 40 pixels.",
                "Snap is the grid, in pixels. A list of numbers is a set of points instead. ModifierSnap is a different setting: it rounds the tween value.",
                """
                var square = await Draggable.Create(".square", props => props.Snap(40));
                """,
                "Components/DraggableSnap.razor"),
            Entry("draggable-release", "releaseEase",
                "Let go, and the square springs to a stop.",
                "ReleaseEase replaces the default release curve. A spring also replaces the built-in release spring. ReleaseMass, ReleaseStiffness, and ReleaseDamping tune that spring when the ease is not one.",
                """
                var square = await Draggable.Create(".square", props => props
                    .ReleaseEase(Easing.Spring()));
                """,
                "Components/DraggableRelease.razor")),
            Group("Methods",
            Entry("draggable-callbacks", "Callbacks",
                "Read the position when the pointer grabs, releases, and settles.",
                "Each callback receives a snapshot. x and y are the position. grabbed, dragged, and enabled are the flags at that moment. The call does not run inside the pointer event.",
                """
                var square = await Draggable.Create(".square", props => props
                    .OnGrab(state => Show("Grab", state))
                    .OnRelease(state => Show("Release", state))
                    .OnSettle(state => Show("Settle", state)));
                """,
                "Components/DraggableCallbacks.razor"),
            Entry("draggable-methods", "Methods",
                "Disable, enable, reset, or revert the drag.",
                "Disable ignores the pointer. Enable restores it. Reset returns both axes to 0. Revert removes the listeners and restores the styles. Dispose calls Revert.",
                """
                await square.Disable();
                await square.Enable();
                await square.Reset();
                await square.Revert();
                """,
                "Components/DraggableMethods.razor"),
            Entry("draggable-properties", "Properties",
                "Read x, y, and velocity while the square moves.",
                "OnUpdate runs when the position changes. The snapshot velocity is the release speed. X, Y, and Velocity read the same values back from the instance.",
                """
                var square = await Draggable.Create(".square", props => props
                    .OnUpdate(state =>
                    {
                        var x = state.X;
                        var y = state.Y;
                        var velocity = state.Velocity;
                    }));
                """,
                "Components/DraggableProperties.razor"))
        ]),
        new("layout", "Layout",
        [
            Leaves(
            Entry("layout", "Root",
                "Animate one root, and leave the other where it is.",
                "Each root is its own layout, so a class change on A does not measure B. Record, render the new class, then Animate. That is the Blazor form of update(), whose callback has to change the DOM before it returns.",
                """
                var layoutA = await Layouts.Create(".root-a");
                var layoutB = await Layouts.Create(".root-b");

                await layoutA.Record();
                _rowA = !_rowA;
                await layoutA.Animate();
                """,
                "Components/LayoutRoot.razor")),
            Group("Changes",
            Entry("layout-display", "display",
                "Cycle flex, grid, and display: none.",
                "The root class walks flex-row, grid-1, flex-col, none, grid-2, and flex-row-reverse. none hides every item. leaveTo scales those items to 0, staggered by 75ms.",
                """
                var layout = await Layouts.Create(".container", props => props
                    .LeaveTo(state => state
                        .Property("transform", "scale(0)")
                        .Delay(Stagger.Create(75))));
                """,
                "Components/LayoutDisplay.razor"),
            Entry("layout-stagger", "stagger",
                "Stagger the row change from the first or last item.",
                "Ease is outExpo. The delay is stagger(50). from is last when the root is already a row, and first otherwise. The value is chosen before the class toggles, matching the docs call.",
                """
                var layout = await Layouts.Create(".container", props => props
                    .Ease(Easing.OutExpo));

                await layout.Animate(props => props.Delay(stagger =>
                    stagger.Value(50).From(fromEnd ? BlazorAnime.StaggerFrom.Last : BlazorAnime.StaggerFrom.First)));
                """,
                "Components/LayoutStagger.razor"),
            Entry("layout-order", "DOM order",
                "Shuffle the items.",
                "The same elements change order. @key keeps each letter on its element so anime.js can follow it across the render.",
                """
                await layout.Record();
                Shuffle(_items);
                await layout.Animate();
                """,
                "Components/LayoutOrder.razor")),
            Group("Enter and leave",
            Entry("layout-enter", "enterFrom",
                "Add items into an empty root.",
                "enterFrom starts at translateY(100px) scale(.25) and opacity 0, over 350ms with out(3). The rest of the layout uses 250ms and outQuad. The button stops at 15 items.",
                """
                var layout = await Layouts.Create(".container", props => props
                    .Duration(250)
                    .Ease(Easing.OutQuad)
                    .EnterFrom(state => state
                        .Property("transform", "translateY(100px) scale(.25)")
                        .Opacity(0)
                        .Duration(350)
                        .Ease(Easing.Out(3))));
                """,
                "Components/LayoutEnter.razor"),
            Entry("layout-leave", "leaveTo",
                "Hide the first item, then remove it.",
                "leaveTo moves the item to translateY(-100px) scale(.25) and opacity 0. The item stays in the DOM with display: none during the transition. OnComplete removes it, which is the Blazor form of layout.leaving.",
                """
                var layout = await Layouts.Create(".container", props => props
                    .Duration(250)
                    .Ease(Easing.OutQuad)
                    .LeaveTo(state => state
                        .Property("transform", "translateY(-100px) scale(.25)")
                        .Opacity(0)
                        .Duration(350)
                        .Ease(Easing.Out(3))));
                """,
                "Components/LayoutLeave.razor"),
            Entry("layout-swap", "swap parent",
                "Move one item between two parents.",
                "The layout root contains both parents, and every descendant is tracked. The item element itself is appended to the other parent. The next parent is raised.",
                """
                var layout = await Layouts.Create(".layout");
                """,
                "Components/LayoutSwap.razor"),
            Entry("layout-modal", "modal",
                "Open a card into a dialog.",
                "The dialog is the root. The cards sit outside it and share data-layout-id with the clone placed inside the dialog. Children names those elements, and --overlay-alpha fades the backdrop. Open uses the card's duration.",
                """
                var layout = await Layouts.Create(".dialog", props => props
                    .Children([".item", "h2", "h3", "p"])
                    .Properties("--overlay-alpha"));

                await layout.Animate(props => props.Duration(duration));
                """,
                "Components/LayoutModal.razor"))
        ]),
        new("scope", "Scope",
        [
            Leaves(
            Entry("scope", "Media queries",
                "One square reacts to the viewport and to reduced motion.",
                "isSmall is (max-width: 200px). reduceMotion is (prefers-reduced-motion). Add runs again when either match changes. The constructor reads Matches and creates the animation while the scope is entered.",
                """
                var scope = await Scopes.Create(props => props
                    .MediaQuery("isSmall", "(max-width: 200px)")
                    .MediaQuery("reduceMotion", "(prefers-reduced-motion)"));

                await scope.Add(async ctx =>
                {
                    var small = ctx.Matches.GetValueOrDefault("isSmall");
                    var reduce = ctx.Matches.GetValueOrDefault("reduceMotion");
                    await Anime.Animate(props =>
                    {
                        props.Targets(".square");
                        if (small)
                            props.TranslateX(0).TranslateY("-1.25rem", "1.25rem");
                        else
                            props.TranslateX("-1.25rem", "1.25rem").TranslateY(0);
                        return props.Loop(true).Alternate(true)
                            .Duration(reduce ? 0 : small ? 750 : 1250);
                    });
                });
                """,
                "Components/ScopeMedia.razor")),
            Group("Parameters",
            Entry("scope-root", "root",
                "Only the square inside the root moves.",
                "Root limits every selector used while the scope is entered. The squares outside that row stay put.",
                """
                var scope = await Scopes.Create(props => props.Root(root));
                await scope.Add(async _ =>
                {
                    await Anime.Animate(props => props
                        .Targets(".square")
                        .TranslateX("4rem")
                        .Loop(true)
                        .Alternate(true));
                });
                """,
                "Components/ScopeRoot.razor"),
            Entry("scope-defaults", "defaults",
                "Three rows share a motion and keep their own ease.",
                "Each row is its own scope. The default ease is out(1), out(2), or out(3). The animation does not set an ease, so it uses the scope default.",
                """
                var scope = await Scopes.Create(props => props
                    .Root(row)
                    .Defaults(defaults => defaults.Ease(Easing.Out(1))));
                """,
                "Components/ScopeDefaults.razor"),
            Entry("scope-constructor", "constructor",
                "A narrow viewport spins the square. A wide one drags it.",
                "Add is the constructor. It runs again when isSmall changes. The previous draggable is reverted with the scope, and OnCleanup releases the .NET handle. The Scope package does not reference Draggable.",
                """
                await scope.Add(async ctx =>
                {
                    ctx.OnCleanup(ReleaseDrag);
                    if (ctx.Matches.GetValueOrDefault("isSmall"))
                        await Anime.Animate(props => props
                            .Targets(".square")
                            .Rotate(360)
                            .Loop(true));
                    else
                        _drag = await Draggables.Create(".square", props => props.Container(stage));
                });
                """,
                "Components/ScopeConstructor.razor")),
            Group("Methods",
            Entry("scope-method", "method",
                "A click moves the square from inside the scope.",
                "AddMethod registers onClick. Invoke enters the scope before the method runs, so the animation it creates is registered. ClientX and ClientY are the click position. There is no DOM event.",
                """
                await scope.AddMethod("onClick", async call =>
                {
                    var small = (await scope.Matches()).GetValueOrDefault("isSmall");
                    await Anime.Animate(props =>
                    {
                        props.Targets(".square");
                        return small
                            ? props.Rotate("+=360").Duration(750)
                            : props.TranslateX($"{call.ClientX}px").TranslateY($"{call.ClientY}px").Duration(400);
                    });
                });

                await scope.Invoke("onClick", new ScopeCall(x, y));
                """,
                "Components/ScopeMethod.razor"),
            Entry("scope-refresh", "refresh",
                "Refresh rebuilds one row and leaves the other.",
                "State keeps an integer across refresh. Each click advances the stagger origin and runs the constructor again. The other row is a separate scope.",
                """
                await scope.Add(async ctx =>
                {
                    var index = await ctx.State.Get("i");
                    await ctx.State.Set("i", index + 1);
                    await Anime.Animate(props => props
                        .Targets(".circle")
                        .Scale(0.5, 1)
                        .Loop(true)
                        .Delay(stagger => stagger.Value(40).From(index)));
                });

                await scope.Refresh();
                """,
                "Components/ScopeRefresh.razor"),
            Entry("scope-revert", "revert",
                "Revert stops the square and restores it.",
                "Revert drops every instance created while the scope was entered, then clears the media listeners and the state map.",
                """
                await scope.Revert();
                """,
                "Components/ScopeRevert.razor"))
        ]),
        new("events", "Events",
        [
            Leaves(
            Entry("events", "onScroll",
                "Scroll plays the square.",
                "AutoPlay takes the observer handle. anime.js links the animation and does not start it. The scroll container is the card.",
                """
                var scroll = await Events.OnScroll(props => props.Container(container));
                await Anime.Animate(props => props
                    .Targets(".square")
                    .TranslateX("4rem")
                    .Rotate("1turn")
                    .Duration(2000)
                    .Alternate(true)
                    .Loop(true)
                    .AutoPlay(scroll.Handle));
                """,
                "Components/EventsAutoplay.razor"),
            Entry("events-sync", "Sync",
                "Enter and leave call playback methods.",
                "sync is a list of method names. Two names bind enter and leave. Four names bind the forward and backward crossings.",
                """
                var scroll = await Events.OnScroll(props => props
                    .Container(container)
                    .Enter("bottom top")
                    .Leave("top bottom")
                    .Sync("resume pause reverse reset"));
                """,
                "Components/EventsSync.razor"),
            Entry("events-progress", "Progress",
                "Scroll position is the animation progress.",
                "Sync(true) seeks the linked animation. Progress runs from 0 to 1 across the threshold.",
                """
                .Sync(true)
                """,
                "Components/EventsProgress.razor"),
            Entry("events-thresholds", "Thresholds",
                "Enter and leave report a snapshot.",
                "OnEnter and OnLeave receive ScrollState. Progress is 0 to 1. The callback does not get the live observer.",
                """
                .Enter("bottom top")
                .Leave("top bottom")
                .OnEnter(state => { })
                .OnLeave(state => { })
                """,
                "Components/EventsThresholds.razor"),
            Entry("events-revert", "Revert",
                "Revert stops the scroll link.",
                "Revert resolves when the observer is removed. It does not return the observer.",
                """
                await scroll.Revert();
                """,
                "Components/EventsRevert.razor"))
        ]),
        new("svg", "SVG",
        [
            Leaves(
            Entry("svg-line-drawing", "Line drawing",
                "Draw strokes with a drawable proxy.",
                "CreateDrawable wraps the paths. Draw walks from \"0 0\" to \"0 1\" to \"1 1\": so the stroke draws on and then the dash leaves the end. The delay is a 100ms stagger.",
                """
                var lines = await Anime.CreateDrawable(".line-drawing-demo .lines path");
                await Anime.Animate(props => props
                    .Targets(lines)
                    .Draw("0 0", "0 1", "1 1")
                    .Ease(Easing.InOutQuad)
                    .Duration(2000)
                    .Delay(Stagger.Create(100))
                    .Loop(true));
                """,
                "Components/SvgLineDrawing.razor"),
            Entry("svg-morphing", "Morphing",
                "Tween a polygon through four point lists.",
                "Each frame is a points string. The first frame moves from the current shape to the next one. Later frames use To. MorphTo returns a function for D or Points when the target is another element.",
                """
                .Points(
                    frame => frame.FromTo(
                        "70 24 119.574 60.369 100.145 117.631 50.855 101.631 3.426 54.369",
                        "70 41 118.574 59.369 111.145 132.631 60.855 84.631 20.426 60.369"),
                    frame => frame.To("70 6 119.574 60.369 100.145 117.631 39.855 117.631 55.426 68.369"),
                    frame => frame.To("70 57 136.574 54.369 89.145 100.631 28.855 132.631 38.426 64.369"),
                    frame => frame.To("70 24 119.574 60.369 100.145 117.631 50.855 101.631 3.426 54.369"))
                """,
                "Components/SvgMorphing.razor"),
            Entry("svg-motion-path", "Motion path",
                "Move an element along an SVG path.",
                "CreateMotionPath returns translateX, translateY, and rotate as functions. Offset is 0 to 1, and 0 starts at the beginning of the path. Dispose the MotionPath with the animation.",
                """
                var motion = await Anime.CreateMotionPath(".motion-path-demo path");
                await Anime.Animate(props => props
                    .Targets(".motion-path-demo .el")
                    .TranslateX(motion.TranslateX)
                    .TranslateY(motion.TranslateY)
                    .Rotate(motion.Rotate)
                    .Duration(2000)
                    .Ease(Easing.Linear)
                    .Loop(true));
                """,
                "Components/SvgMotionPath.razor"))
        ]),
        new("utilities", "Utilities",
        [
            Group("Stagger",
            Entry("stagger-from", "From",
                "Start the stagger at the first item, the last, the center, or an index.",
                "Stagger.Create builds the delay. From accepts StaggerPosition or an integer index.",
                """
                await Anime.Animate(props => props
                    .Targets(".staggering-from-demo .el")
                    .TranslateX(270)
                    .Delay(Stagger.Create(100, stagger => stagger.From(StaggerPosition.Center)))
                    .Duration(1000)
                    .Loop(true));
                """,
                "Components/StaggerFrom.razor"),
            Entry("stagger-direction", "Reversed",
                "Start the stagger at the last target.",
                "Reversed(true) walks from the last target back to the first.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Reversed(true)))
                """,
                "Components/StaggerDirection.razor"),
            Entry("stagger-easing", "Ease",
                "Ease the gaps between staggered targets.",
                "Ease on the stagger changes the gap between targets.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Ease(Easing.OutQuad)))
                """,
                "Components/StaggerEasing.razor"),
            Entry("stagger-grid", "Grid",
                "Stagger across columns and rows.",
                "Grid takes columns first, then rows. The targets are laid out in that order, left to right. Loop(3) is three extra repeats of the scale.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Grid(14, 5)
                    .From(StaggerPosition.Center)))
                """,
                "Components/StaggerGrid.razor"),
            Entry("stagger-axis", "Axis",
                "Limit a grid stagger to one axis.",
                "Axis is StaggerAxis.X or StaggerAxis.Y. Combined with From, the wave starts at the center and travels along that axis. Loop(3) is three extra repeats.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Grid(14, 5)
                    .From(StaggerPosition.Center)
                    .Axis(StaggerAxis.X)))
                """,
                "Components/StaggerAxisDemo.razor")),
            Leaves(
            Entry("helpers", "Utils",
                "Random, get, set, and remove are utils. Speed is the engine.",
                "Get reads computed style, and the unit overload converts it. Set writes immediately and does not cancel other tweens. Version is the pinned anime.js build. PauseOnDocumentHidden is engine.pauseOnDocumentHidden.",
                """
                var distance = await Anime.Random(0, 250);
                await Anime.Set(".helpers-demo .el", "translateX", distance);
                var current = await Anime.Get(".helpers-demo .el", "translateX");
                await Anime.Remove(".helpers-demo .el");
                var version = await Anime.Version();
                await Anime.SetSpeed(1);
                """,
                "Components/AnimationHelpers.razor"))
        ]),
        new("easings", "Easings",
        [
            Leaves(
            Entry("easings", "Easings",
                "A named ease, a spring, and a sampled C# curve.",
                "InOutExpo is a built-in name. Spring() uses the default spring. Curve samples a C# function from 0 to 1, and the browser interpolates that table.",
                """
                await Anime.Animate(props => props
                    .Targets(".animation-easings-demo .expo")
                    .TranslateX(220)
                    .Ease(Easing.InOutExpo)
                    .Loop(true));
                await Anime.Animate(props => props
                    .Targets(".animation-easings-demo .spring")
                    .TranslateX(220)
                    .Ease(Easing.Spring())
                    .Loop(true));
                await Anime.Animate(props => props
                    .Targets(".animation-easings-demo .curve")
                    .TranslateX(220)
                    .Ease(Easing.Curve(t => t * t))
                    .Loop(true));
                """,
                "Components/AnimationEasings.razor"))
        ])
    ];

    public static IReadOnlyList<DemoEntry> All { get; } = Sections.SelectMany(section => section.Entries).ToList();

    public static DemoEntry Default => All[0];

    public static DemoEntry? Find(string? slug) =>
        All.FirstOrDefault(entry => string.Equals(entry.Slug, slug, StringComparison.OrdinalIgnoreCase));

    private static DemoEntry Entry(string slug, string title, string summary, string details, string code, string source) =>
        new(slug, title, summary, details, code.Trim(), source);

    private static DemoGroup Group(string title, params DemoEntry[] entries) => new(title, entries);

    private static DemoGroup Leaves(params DemoEntry[] entries) => new(null, entries);
}
