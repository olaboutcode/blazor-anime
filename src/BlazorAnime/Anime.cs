using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAnime;

public class Anime(IJSRuntime jSRuntime) : IAnime
{
    /// <summary>
    /// Create an animation.
    /// </summary>
    /// <param name="configure"></param>
    /// <returns>Animation</returns>
    public async Task<Animation> Animate(Func<PropsBuilder, PropsBuilder> configure)
    {
        var builder = configure(new PropsBuilder());
        return await Create(IdentifierCreateAnimation, builder, static (js, callbacks) => new Animation(js, callbacks));
    }

    /// <summary>
    /// Create an animation.
    /// </summary>
    /// <param name="configure"></param>
    /// <returns>Animation</returns>
    public async Task<Animation> Animate(Func<PropsBuilder, Task<PropsBuilder>> configure)
    {
        var builder = await configure(new PropsBuilder());
        return await Create(IdentifierCreateAnimation, builder, static (js, callbacks) => new Animation(js, callbacks));
    }

    /// <summary>
    /// Create a timeline. Autoplay, loop, and callbacks stay on the timeline.
    /// Duration, ease, and every other key are child defaults.
    /// </summary>
    /// <param name="setDefaults"></param>
    /// <returns>Timeline</returns>
    public async Task<Timeline> CreateTimeline(Func<PropsBuilder, PropsBuilder> setDefaults)
    {
        var builder = setDefaults(new PropsBuilder());
        return await Create(IdentifierCreateTimeline, builder, static (js, callbacks) => new Timeline(js, callbacks));
    }

    /// <summary>
    /// Create a timeline. Autoplay, loop, and callbacks stay on the timeline.
    /// Duration, ease, and every other key are child defaults.
    /// </summary>
    /// <param name="setDefaults"></param>
    /// <returns>Timeline</returns>
    public async Task<Timeline> CreateTimeline(Func<PropsBuilder, Task<PropsBuilder>> setDefaults)
    {
        var builder = await setDefaults(new PropsBuilder());
        return await Create(IdentifierCreateTimeline, builder, static (js, callbacks) => new Timeline(js, callbacks));
    }

    /// <summary>
    /// Motion-path functions for an SVG path, polygon, or polyline.
    /// <paramref name="offset"/> is 0–1 along the length. 0 starts at the beginning.
    /// </summary>
    public async Task<MotionPath> CreateMotionPath(string svgSelector, double offset = 0)
    {
        var holder = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierCreateMotionPath,
            svgSelector,
            offset);
        return await ReadMotionPath(holder);
    }

    /// <summary>
    /// Motion-path functions for an SVG path, polygon, or polyline.
    /// <paramref name="offset"/> is 0–1 along the length. 0 starts at the beginning.
    /// </summary>
    public async Task<MotionPath> CreateMotionPath(ElementReference element, double offset = 0)
    {
        var holder = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierCreateMotionPath,
            element,
            offset);
        return await ReadMotionPath(holder);
    }

    /// <summary>
    /// Drawable proxies for SVG geometry. Animate them with <c>Draw</c>.
    /// </summary>
    public async Task<DrawableTarget> CreateDrawable(string selector)
    {
        var reference = await JsRuntime.InvokeAsync<IJSObjectReference>(IdentifierCreateDrawable, selector);
        return new DrawableTarget(reference);
    }

    /// <summary>
    /// Drawable proxies for one SVG element. Animate them with <c>Draw</c>.
    /// </summary>
    public async Task<DrawableTarget> CreateDrawable(ElementReference element)
    {
        var reference = await JsRuntime.InvokeAsync<IJSObjectReference>(IdentifierCreateDrawable, element);
        return new DrawableTarget(reference);
    }

    /// <summary>
    /// The function <c>morphTo</c> returns. Pass it to <c>D</c> or <c>Points</c>.
    /// <paramref name="precision"/> 0 copies the target shape. The default is 0.33.
    /// </summary>
    public async Task<IJSObjectReference> MorphTo(string shapeSelector, double precision = 0.33) =>
        await JsRuntime.InvokeAsync<IJSObjectReference>(IdentifierMorphTo, shapeSelector, precision);

    /// <summary>
    /// The function <c>morphTo</c> returns. Pass it to <c>D</c> or <c>Points</c>.
    /// </summary>
    public async Task<IJSObjectReference> MorphTo(ElementReference element, double precision = 0.33) =>
        await JsRuntime.InvokeAsync<IJSObjectReference>(IdentifierMorphTo, element, precision);

    /// <summary>
    /// Creates a JavaScript object whose numeric properties can be animation targets.
    /// Dictionary keys are sent as-is.
    /// </summary>
    public async Task<JsTarget> CreateObject(IReadOnlyDictionary<string, object> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var reference = await JsRuntime.InvokeAsync<IJSObjectReference>(IdentifierCreateObject, values);
        return new JsTarget(reference);
    }

    /// <summary>
    /// Immediately sets values. This is <c>utils.set</c>. The animation it returns is not exposed.
    /// </summary>
    public async Task Set(string[] targets, Func<PropsBuilder, PropsBuilder> build)
    {
        await JsRuntime.InvokeVoidAsync(
            IdentifierSetElementValue, 
            targets, 
            build(new PropsBuilder()).Build().ToObject());
    }
    
    /// <summary>
    /// Immediately sets values to the specified targets.
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="build"></param>
    public async Task Set(object[] targets, Func<PropsBuilder, PropsBuilder> build)
    {
        await JsRuntime.InvokeVoidAsync(
            IdentifierSetElementValue, 
            targets, 
            build(new PropsBuilder()).Build().ToObject());
    }

    /// <summary>
    /// Immediately sets one property on a CSS selector.
    /// </summary>
    public async Task Set(string target, string property, double value) =>
        await Set([target], props => props.Prop(property, value));

    /// <summary>
    /// Immediately sets one property on a CSS selector.
    /// </summary>
    public async Task Set(string target, string property, string value) =>
        await Set([target], props => props.Prop(property, value));

    /// <summary>
    /// Immediately sets values on an element.
    /// </summary>
    public async Task Set(ElementReference target, Func<PropsBuilder, PropsBuilder> build)
    {
        await JsRuntime.InvokeVoidAsync(
            IdentifierSetElementValue,
            target,
            build(new PropsBuilder()).Build().ToObject());
    }

    /// <summary>
    /// Removes targets from every running animation. This is <c>anime.remove()</c>.
    /// </summary>
    public async Task Remove(string targets) =>
        await JsRuntime.InvokeVoidAsync(IdentifierRemove, targets);

    /// <summary>
    /// Removes an element from every running animation.
    /// </summary>
    public async Task Remove(ElementReference target) =>
        await JsRuntime.InvokeVoidAsync(IdentifierRemove, target);

    /// <summary>
    /// Global playback speed. <c>1</c> is normal speed. This is <c>anime.speed</c>.
    /// </summary>
    public async Task<double> GetSpeed() =>
        await JsRuntime.InvokeAsync<double>(IdentifierGetSpeed);

    /// <summary>
    /// Sets <c>anime.speed</c>.
    /// </summary>
    public async Task SetSpeed(double speed) =>
        await JsRuntime.InvokeVoidAsync(IdentifierSetSpeed, speed);

    /// <summary>
    /// The embedded anime.js version.
    /// </summary>
    public async Task<string> Version() =>
        await JsRuntime.InvokeAsync<string>(IdentifierVersion);

    /// <summary>
    /// Reads a value with <c>utils.get</c>. Pass a unit to convert it.
    /// </summary>
    public async Task<string> Get(ElementReference target, string propName) =>
        await JsRuntime.InvokeAsync<string>(IdentifierGetElementValue, target, propName, null);

    /// <summary>
    /// Get the original value of an element, converted to <paramref name="cssUnit"/>.
    /// </summary>
    public async Task<double> Get(ElementReference target, string propName, string cssUnit) =>
        await JsRuntime.InvokeAsync<double>(IdentifierGetElementValue, target, propName, cssUnit);

    public async Task<string> Get(object target, string propName)
    {
        return await JsRuntime.InvokeAsync<string>(
            IdentifierGetElementValue,
            target,
            propName,
            null
        );
    }
    /// <summary>
    /// Get the original value of an element.
    /// <para>
    /// Since anime.js uses getComputedStyle to access original CSS,
    /// the values are almost always returned in 'px',
    /// <br/>the third argument converts the value in the desired unit.
    /// </para>
    /// </summary>
    /// <param name="target"></param>
    /// <param name="propName"></param>
    /// <param name="cssUnit"></param>
    /// <returns></returns>
    public async Task<double> Get(object target, string propName, string cssUnit)
    {
        return await JsRuntime.InvokeAsync<double>(
            IdentifierGetElementValue,
            target,
            propName,
            cssUnit
        );
    }

    /// <summary>
    /// Returns a random integer in the range. This is <c>utils.random</c>.
    /// </summary>
    public async Task<int> Random(int minValue, int maxValue)
    {
        return await JsRuntime.InvokeAsync<int>(
            IdentifierGetRandomValue,
            minValue,
            maxValue
        );
    }

    /// <summary>
    /// Sets <c>engine.pauseOnDocumentHidden</c>. anime.js defaults this to true.
    /// </summary>
    public async Task PauseOnDocumentHidden(bool value)
    {
        await JsRuntime.InvokeVoidAsync(IdentifierPauseOnDocumentHidden, value);
    }

    /// <summary>
    /// Whether animations pause while the document is hidden. anime.js defaults this to true.
    /// </summary>
    public async Task<bool> GetPauseOnDocumentHidden() =>
        await JsRuntime.InvokeAsync<bool>(IdentifierGetPauseOnDocumentHidden);

    private static async Task<MotionPath> ReadMotionPath(IJSObjectReference holder)
    {
        IJSObjectReference? translateX = null;
        IJSObjectReference? translateY = null;
        IJSObjectReference? rotate = null;
        try
        {
            translateX = await holder.InvokeAsync<IJSObjectReference>("getTranslateX");
            translateY = await holder.InvokeAsync<IJSObjectReference>("getTranslateY");
            rotate = await holder.InvokeAsync<IJSObjectReference>("getRotate");
            return new MotionPath(holder, translateX, translateY, rotate);
        }
        catch
        {
            if (translateX is not null)
                await translateX.DisposeAsync();
            if (translateY is not null)
                await translateY.DisposeAsync();
            if (rotate is not null)
                await rotate.DisposeAsync();
            await holder.DisposeAsync();
            throw;
        }
    }

    private async Task<TInstance> Create<TInstance>(
        string identifier,
        PropsBuilder builder,
        Func<IJSObjectReference, IReadOnlyList<IDisposable>, TInstance> factory)
    {
        try
        {
            var jsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
                identifier,
                builder.Build().ToObject());
            return factory(jsRef, builder.CallbackHandles);
        }
        catch
        {
            foreach (var handle in builder.CallbackHandles)
                handle.Dispose();
            throw;
        }
    }

    private IJSRuntime JsRuntime { get; } = jSRuntime;

    private const string IdentifierCreateAnimation = "AnimeJs.createAnimation";
    private const string IdentifierCreateTimeline = "AnimeJs.createTimeline";
    private const string IdentifierSetElementValue = "AnimeJs.set";
    private const string IdentifierGetElementValue = "AnimeJs.get";
    private const string IdentifierGetRandomValue = "AnimeJs.random";
    private const string IdentifierCreateMotionPath = "AnimeJs.createMotionPath";
    private const string IdentifierCreateDrawable = "AnimeJs.createDrawable";
    private const string IdentifierMorphTo = "AnimeJs.morphTo";
    private const string IdentifierPauseOnDocumentHidden = "AnimeJs.pauseOnDocumentHidden";
    private const string IdentifierGetPauseOnDocumentHidden = "AnimeJs.getPauseOnDocumentHidden";
    private const string IdentifierRemove = "AnimeJs.remove";
    private const string IdentifierGetSpeed = "AnimeJs.getSpeed";
    private const string IdentifierSetSpeed = "AnimeJs.setSpeed";
    private const string IdentifierVersion = "AnimeJs.version";
    private const string IdentifierCreateObject = "AnimeJs.createObject";
}

public interface IAnime
{
    Task<Animation> Animate(Func<PropsBuilder, PropsBuilder> configure);
    Task<Animation> Animate(Func<PropsBuilder, Task<PropsBuilder>> configure);
    Task<Timeline> CreateTimeline(Func<PropsBuilder, PropsBuilder> configureDefaults);
    Task<Timeline> CreateTimeline(Func<PropsBuilder, Task<PropsBuilder>> configureDefaults);
    Task<MotionPath> CreateMotionPath(string svgSelector, double offset = 0);
    Task<MotionPath> CreateMotionPath(ElementReference element, double offset = 0);
    Task<DrawableTarget> CreateDrawable(string selector);
    Task<DrawableTarget> CreateDrawable(ElementReference element);
    Task<IJSObjectReference> MorphTo(string shapeSelector, double precision = 0.33);
    Task<IJSObjectReference> MorphTo(ElementReference element, double precision = 0.33);
    Task<JsTarget> CreateObject(IReadOnlyDictionary<string, object> values);
    Task Set(string[] targets, Func<PropsBuilder, PropsBuilder> build);
    Task Set(object[] targets, Func<PropsBuilder, PropsBuilder> build);
    Task Set(string target, string property, double value);
    Task Set(string target, string property, string value);
    Task Set(ElementReference target, Func<PropsBuilder, PropsBuilder> build);
    Task Remove(string targets);
    Task Remove(ElementReference target);
    Task<string> Get(object target, string propName);
    Task<string> Get(ElementReference target, string propName);
    Task<double> Get(object target, string propName, string cssUnit);
    Task<double> Get(ElementReference target, string propName, string cssUnit);
    Task<int> Random(int minValue, int maxValue);
    Task<double> GetSpeed();
    Task SetSpeed(double speed);
    Task<string> Version();
    Task PauseOnDocumentHidden(bool value);
    Task<bool> GetPauseOnDocumentHidden();
}

public static class PropExtensions
{
    internal static Dictionary<string, object> ToObject(this IEnumerable<Prop> props)
    {
        return props.ToDictionary(p => p.GetName(), p => p.GetValue());
    }
}