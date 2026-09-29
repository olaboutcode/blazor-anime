namespace Examples;

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
                    .TranslateX(property => property.To(Relative.Multiply(2.5)).Duration(1000))
                    .Width(property => property.To(Relative.Subtract(20)).Duration(1800))
                    .Rotate(property => property.To(Relative.Add("2turn")).Duration(1800))
                    .Alternate(true)
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
                    .Alternate(true)
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
                    .Alternate(true)
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
                    .Ease(Easing.OutElastic(1, .8))
                    .Loop(true));
                """,
                "Components/OfficialDocsExamples/Keyframes.razor"),
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
                "Components/OfficialDocsExamples/PropertyKeyframes.razor"),
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
                "Components/OfficialDocsExamples/AnimationCallbacks.razor"),
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
                "Components/OfficialDocsExamples/AnimationControls.razor"),
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
                "Components/OfficialDocsExamples/AnimationEasings.razor"),
            Entry("helpers", "Helpers",
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
                "Components/OfficialDocsExamples/AnimationHelpers.razor")
        ]),
        new("Timeline",
        [
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
                "Components/OfficialDocsExamples/TimelineBasics.razor"),
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
                "Components/OfficialDocsExamples/TimelineOffset.razor"),
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
                "Components/OfficialDocsExamples/TimelineInheritance.razor"),
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
            Entry("stagger-direction", "Reversed",
                "Start the stagger at the last target.",
                "Reversed(true) walks from the last target back to the first.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Reversed(true)))
                """,
                "Components/OfficialDocsExamples/StaggerDirection.razor"),
            Entry("stagger-easing", "Ease",
                "Ease the gaps between staggered targets.",
                "Ease on the stagger changes the gap between targets.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Ease(Easing.OutQuad)))
                """,
                "Components/OfficialDocsExamples/StaggerEasing.razor"),
            Entry("stagger-grid", "Grid",
                "Stagger across columns and rows.",
                "Grid takes columns first, then rows. The targets are laid out in that order, left to right. Loop(3) is three extra repeats of the scale.",
                """
                .Delay(Stagger.Create(100, stagger => stagger
                    .Grid(14, 5)
                    .From(StaggerPosition.Center)))
                """,
                "Components/OfficialDocsExamples/StaggerGrid.razor"),
            Entry("stagger-axis", "Axis",
                "Limit a grid stagger to one axis.",
                "Axis is StaggerAxis.X or StaggerAxis.Y. Combined with From, the wave starts at the center and travels along that axis. Loop(3) is three extra repeats.",
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
                "Components/OfficialDocsExamples/SvgLineDrawing.razor"),
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
                "Components/OfficialDocsExamples/SvgMorphing.razor"),
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
                "Components/OfficialDocsExamples/SvgMotionPath.razor")
        ]),
        new("Examples",
        [
            Entry("animated-sphere", "Animated sphere",
                "A wireframe sphere that draws itself, shifts its gradient, and breathes.",
                "Twenty-one SVG rings share one intro, then a long-running driver seeks a paused animation on each ring. The gradient on the stroke moves on its own timeline.",
                """
                var rings = await Anime.CreateDrawable(".animated-sphere-demo .sphere path");
                await Anime.Animate(props => props
                    .Targets(rings)
                    .Draw("0 1")
                    .Duration(3900)
                    .Ease(Easing.InOutCirc)
                    .Delay(Stagger.Create(190, options => options.Reversed(true))));
                """,
                "Components/AdditionalFunExamples/AnimatedSphere.razor"),
            Entry("easter-icons", "Easter icons",
                "Five icons play in sequence, then each one can be replayed.",
                "A timeline draws the egg, chick, basket, brush, and rabbit. Complete callbacks restart the shiver loops, and the buttons start a single icon again.",
                """
                await timeline.AddAsync(props => props
                    .Targets(".easter-demo .gift-icon__egg")
                    .ScaleY([
                        frame => frame.To(0.9).Duration(170),
                        frame => frame.To(1.1).Duration(170),
                        frame => frame.To(1).Duration(170)
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
                    .Ease(Easing.Linear)
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
                        frame => frame.To(-8).Duration(300),
                        frame => frame.To(8).Duration(600),
                        frame => frame.To(0).Duration(300)
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
