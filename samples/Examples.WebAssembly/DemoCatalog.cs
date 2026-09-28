namespace Examples.WebAssembly;

public sealed record DemoEntry(
    string Slug,
    string Title,
    string Summary,
    string Details,
    string Code,
    string Source);

public sealed record DemoSection(string Title, IReadOnlyList<DemoEntry> Entries);

public static class DemoCatalog
{
    public static IReadOnlyList<DemoSection> Sections { get; } =
    [
        new("Animation",
        [
            Entry("relative-values", "Relative values",
                "Move, resize, and rotate from the value the element already has.",
                "Relative.Multiply, Relative.Subtract, and Relative.Add read the current computed value when the animation is created. A unit string such as \"2turn\" is passed through to anime.js.",
                """
                await Anime.Animate(props => props
                    .Targets(".demo-relative-values .el")
                    .TranslateX(property => property.Value(Relative.Multiply(2.5)).Duration(1000))
                    .Width(property => property.Value(Relative.Subtract(20)).Duration(1800))
                    .Rotate(property => property.Value(Relative.Add("2turn")).Duration(1800))
                    .Direction(Direction.Alternate)
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/RelativeValues.razor"),
            Entry("function-based-values", "Function based values",
                "Each target gets its own value from its index or its data attributes.",
                "TargetInfo is a snapshot taken when the animation is built. data-x becomes Dataset[\"x\"]. Index callbacks receive the target index and the target count. The functions are not called again on later frames.",
                """
                await Anime.Animate(props => props
                    .Targets(".function-based-values-demo .el")
                    .TranslateX(target => double.Parse(target.Dataset.GetValueOrDefault("x", "0")))
                    .TranslateY((index, count) => 50 + (-50 * index))
                    .Scale((index, count) => (count - index) + .25)
                    .Direction(Direction.Alternate)
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/FunctionBasedValues.razor"),
            Entry("function-based-parameters", "Function based parameters",
                "Delay can be a function of the target index.",
                "Lambdas and ordinary methods both work. The function runs once per target while the animation is created, then anime.js uses the returned number.",
                """
                await Anime.Animate(props => props
                    .Targets(".function-based-parameters-demo .el")
                    .TranslateX(180)
                    .Delay((index, total) => index * 120)
                    .Duration(750)
                    .Direction(Direction.Alternate)
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/FunctionBasedParameters.razor"),
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
                    .Easing(Easing.EaseOutElastic(1, .8))
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/Keyframes.razor"),
            Entry("property-keyframes", "Property keyframes",
                "Keyframes can live on a single property.",
                "The array form is the same one used for animation keyframes. Duration on the animation is the full run; each step can set its own duration too.",
                """
                await Anime.Animate(props => props
                    .Targets(".property-keyframes-demo .el")
                    .TranslateX([
                        step => step.Value(250).Duration(1000),
                        step => step.Value(0).Duration(500)
                    ])
                    .Duration(4000)
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/PropertyKeyframes.razor"),
            Entry("callbacks", "Callbacks",
                "Begin, update, and complete receive a snapshot of the animation.",
                "The callback is an Action of AnimationState. It can be a lambda. The snapshot includes progress, current time, and how many loops remain. It is not the live anime.js instance.",
                """
                await Anime.Animate(props => props
                    .Targets(".animation-callbacks-demo .el")
                    .TranslateX(250)
                    .Begin(state => Log("began", state))
                    .Update(state => Log("update", state))
                    .Complete(state => Log("complete", state))
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/AnimationCallbacks.razor"),
            Entry("controls", "Controls",
                "Play, pause, seek, and restart the same animation.",
                "AutoPlay(false) leaves the animation paused. Seek takes milliseconds. Progress takes a percentage from 0 to 100. Finished completes when playback ends, and does not complete while Loop(true) is set.",
                """
                var animation = await Anime.Animate(props => props
                    .Targets(".animation-controls-demo .el")
                    .Prop("translateX", 250)
                    .AutoPlay(false));

                await animation.Play();
                await animation.Seek(400);
                await animation.Restart();
                """,
                "Components/OfficialDocsExamples/AnimationControls.razor"),
            Entry("easings", "Easings",
                "Named curves, elastic, spring, and a sampled C# function.",
                "EaseInOutExpo is a property. EaseOutElastic is a method because amplitude and period are optional. Easing.Curve samples a C# function from 0 to 1 and the browser interpolates that table.",
                """
                await Anime.Animate(props => props
                    .Targets(".animation-easings-demo .el")
                    .TranslateX(220)
                    .Duration(900)
                    .Easing(Easing.EaseInOutExpo)
                    .Direction(Direction.Alternate)
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/AnimationEasings.razor"),
            Entry("helpers", "Helpers",
                "Random, get, and set talk to the engine without building a timeline.",
                "Random is asynchronous, so read it before you build an animation. Get uses computed style. Remove takes targets out of running animations and leaves the elements in the document.",
                """
                var distance = await Anime.Random(0, 250);
                var current = await Anime.Get(".el", "translateX");
                await Anime.Set(".el", "translateX", distance);
                await Anime.Remove(".el");
                """,
                "Components/OfficialDocsExamples/AnimationHelpers.razor")
        ]),
        new("Timeline",
        [
            Entry("timeline-basics", "Basics",
                "A timeline plays child animations with shared defaults.",
                "Duration and easing set on the timeline apply to each child that does not set its own. AddAsync appends a child. The timeline is itself an animation, so play, pause, and dispose are the same methods.",
                """
                var timeline = await Anime.Timeline(props => props
                    .Duration(750)
                    .Easing(Easing.EaseOutExpo));

                await timeline.AddAsync(child => child
                    .Targets(".offsets-demo .el.square")
                    .TranslateX(250));
                """,
                "Components/OfficialDocsExamples/TimelineBasics.razor"),
            Entry("timeline-offsets", "Offsets",
                "Place children at a time, or relative to the previous child.",
                "A number is an absolute time in milliseconds. OffSet.Before and OffSet.After shift from the end of the previous child. The string \"+=500\" is the same idea as OffSet.After(500).",
                """
                await timeline.AddAsync(child => child
                    .Targets(".offsets-demo .el.square")
                    .TranslateX(250));
                await timeline.AddAsync(child => child
                    .Targets(".offsets-demo .el.circle")
                    .TranslateX(250), OffSet.Before(600));
                await timeline.AddAsync(child => child
                    .Targets(".offsets-demo .el.triangle")
                    .TranslateX(250), 400);
                """,
                "Components/OfficialDocsExamples/TimelineOffset.razor"),
            Entry("timeline-inheritance", "Inheritance",
                "Children inherit duration and easing from the timeline.",
                "Set the shared timing on the timeline. A child can still override a single property, such as its own easing or duration, without repeating the rest.",
                """
                var timeline = await Anime.Timeline(props => props
                    .Duration(750)
                    .Easing(Easing.EaseOutExpo));

                await timeline.AddAsync(child => child
                    .Targets(".offsets-demo .el.square")
                    .TranslateX(250));
                """,
                "Components/OfficialDocsExamples/TimelineInheritance.razor"),
            Entry("timeline-controls", "Controls",
                "A timeline pauses, seeks, and restarts as one animation.",
                "Call Play, Pause, Seek, or Restart on the timeline. Children added with AddAsync stay on that same instance.",
                """
                var timeline = await Anime.Timeline(props => props
                    .Duration(750)
                    .Easing(Easing.EaseOutExpo)
                    .AutoPlay(false));

                await timeline.AddAsync(child => child
                    .Targets(".offsets-demo .el.square")
                    .TranslateX(250));
                await timeline.Play();
                """,
                "Components/OfficialDocsExamples/TimelineControls.razor")
        ]),
        new("Stagger",
        [
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
                "Components/OfficialDocsExamples/StaggerFrom.razor"),
            Entry("stagger-direction", "Direction",
                "Reverse the order the stagger walks.",
                "Direction on the stagger options is independent of the animation direction. Reverse starts at the last target.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Direction(Direction.Reverse)))
                """,
                "Components/OfficialDocsExamples/StaggerDirection.razor"),
            Entry("stagger-easing", "Easing",
                "Ease the stagger itself, not only the motion.",
                "The easing on a stagger changes the gap between targets. Easing.Curve works here too, and the curve is passed through without the extra wrapper an animation easing needs.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Easing(Easing.EaseOutQuad)))
                """,
                "Components/OfficialDocsExamples/StaggerEasing.razor"),
            Entry("stagger-grid", "Grid",
                "Stagger across columns and rows.",
                "Grid takes columns first, then rows. The targets are laid out in that order, left to right.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Grid(14, 5)
                    .From(StaggerPosition.Center)))
                """,
                "Components/OfficialDocsExamples/StaggerGrid.razor"),
            Entry("stagger-axis", "Axis",
                "Limit a grid stagger to one axis.",
                "Axis is StaggerAxis.X or StaggerAxis.Y. Combined with From, the wave starts on a side or at the center and travels along that axis.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Grid(14, 5)
                    .From(StaggerPosition.Center)
                    .Axis(StaggerAxis.X)))
                """,
                "Components/OfficialDocsExamples/StaggerAxisDemo.razor")
        ]),
        new("SVG",
        [
            Entry("svg-line-drawing", "Line drawing",
                "Draw strokes by animating the dash offset back to zero.",
                "StrokeDashoffset(0) is the usual line-drawing target. Delay by index so each path starts after the one before it. SetDashoffset can measure a path and set its dash array to that length.",
                """
                await Anime.Animate(props => props
                    .Targets(".line-drawing-demo .lines path")
                    .StrokeDashoffset(0)
                    .Easing(Easing.EaseInOutSine)
                    .Duration(1500)
                    .Delay((index, total) => index * 250)
                    .Direction(Direction.Alternate)
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/SvgLineDrawing.razor"),
            Entry("svg-morphing", "Morphing",
                "Interpolate one path d attribute into another.",
                "The two path strings need a compatible command structure. Anime.js interpolates the numbers inside the d attribute.",
                """
                await Anime.Animate(props => props
                    .Targets(".morphing-demo .polymorph")
                    .D(nextPath)
                    .Duration(2000)
                    .Easing(Easing.EaseInOutQuad)
                    .Direction(Direction.Alternate)
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/SvgMorphing.razor"),
            Entry("svg-motion-path", "Motion path",
                "Move an element along an SVG path.",
                "GetSvgPath reads the path element. The x, y, and angle properties are values anime.js samples along that path. Dispose the SvgPath with the animation.",
                """
                var path = await Anime.GetSvgPath(".motion-path-demo path");
                await Anime.Animate(async props => props
                    .Targets(".motion-path-demo .el")
                    .TranslateX(await path.Get("x"))
                    .TranslateY(await path.Get("y"))
                    .Rotate(await path.Get("angle"))
                    .Duration(2000)
                    .Easing(Easing.Linear)
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/SvgMotionPath.razor")
        ]),
        new("Examples",
        [
            Entry("animated-sphere", "Animated sphere",
                "A wireframe sphere that draws itself, shifts its gradient, and breathes.",
                "Twenty-one SVG rings share one intro, then a long-running driver seeks a paused animation on each ring. The gradient on the stroke moves on its own timeline.",
                """
                await Anime.Animate(props => props
                    .Targets(".animated-sphere-demo .sphere path")
                    .StrokeDashoffset(0)
                    .Duration(3900)
                    .Delay(Stagger.Create(190))
                    .Direction(Direction.Reverse)
                    .Easing(Easing.EaseInOutCirc));
                """,
                "Components/AdditionalFunExamples/AnimatedSphere.razor"),
            Entry("easter-icons", "Easter icons",
                "Five icons play in sequence, then each one can be replayed.",
                "A timeline draws the egg, chick, basket, brush, and rabbit. Complete callbacks restart the shiver loops, and the buttons start a single icon again.",
                """
                await timeline.AddAsync(props => props
                    .Targets(".easter-demo .gift-icon__egg")
                    .ScaleY([
                        frame => frame.Value(0.9).Duration(170),
                        frame => frame.Value(1.1).Duration(170),
                        frame => frame.Value(1).Duration(170)
                    ]));
                """,
                "Components/AdditionalFunExamples/AnimatedEasterIcons.razor"),
            Entry("pedaling-bicycle", "Pedaling bicycle",
                "The wheels, pedals, and road loop together.",
                "One timeline rotates both tyres, turns the pedal, and slides the road marks. Transform boxes are set to fill-box so the SVG parts spin around themselves.",
                """
                await timeline.AddAsync(props => props
                    .Targets(".bicycle-demo #tyre1, .bicycle-demo #tyre2")
                    .Rotate(360)
                    .Duration(1000)
                    .Easing(Easing.Linear)
                    .Loop(true));
                """,
                "Components/AdditionalFunExamples/PedalingBicycle.razor"),
            Entry("error-404", "404 page",
                "The digits, astronaut, and hair of a lost page keep moving.",
                "Each layer is its own looping animation: the zero swings, the astronaut floats, and the hair uses a short keyframe sequence. Razor writes the keyframe name as @@keyframes.",
                """
                await Anime.Animate(props => props
                    .Targets(".error-404-demo #zero")
                    .Rotate([
                        frame => frame.Value(-8).Duration(300),
                        frame => frame.Value(8).Duration(600),
                        frame => frame.Value(0).Duration(300)
                    ])
                    .Loop(true));
                """,
                "Components/AdditionalFunExamples/Error404Page.razor")
        ])
    ];

    public static IReadOnlyList<DemoEntry> All { get; } = Sections.SelectMany(section => section.Entries).ToList();

    public static DemoEntry Default => All[0];

    public static DemoEntry? Find(string? slug) =>
        All.FirstOrDefault(entry => string.Equals(entry.Slug, slug, StringComparison.OrdinalIgnoreCase));

    private static DemoEntry Entry(string slug, string title, string summary, string details, string code, string source) =>
        new(slug, title, summary, details, code.Trim(), source);
}
