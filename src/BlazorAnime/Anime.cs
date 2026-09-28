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
    /// Create a timeline.
    /// </summary>
    /// <param name="setDefaults"></param>
    /// <returns>Timeline</returns>
    public async Task<Timeline> Timeline(Func<PropsBuilder, PropsBuilder> setDefaults)
    {
        var builder = setDefaults(new PropsBuilder());
        return await Create(IdentifierCreateTimeline, builder, static (js, callbacks) => new Timeline(js, callbacks));
    }

    /// <summary>
    /// Create a timeline.
    /// </summary>
    /// <param name="setDefaults"></param>
    /// <returns>Timeline</returns>
    public async Task<Timeline> Timeline(Func<PropsBuilder, Task<PropsBuilder>> setDefaults)
    {
        var builder = await setDefaults(new PropsBuilder());
        return await Create(IdentifierCreateTimeline, builder, static (js, callbacks) => new Timeline(js, callbacks));
    }

    /// <summary>
    /// Returns an <c>anime.path()</c> for an SVG element. <paramref name="percent"/> is how much of the path to travel (0-100).
    /// </summary>
    public async Task<SvgPath> GetSvgPath(string svgSelector, double percent = 100)
    {
        var pathJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierGetSvgPath,
            svgSelector,
            percent
        );
        return new SvgPath(pathJsRef);
    }

    /// <summary>
    /// Returns an <c>anime.path()</c> for an SVG element.
    /// </summary>
    public async Task<SvgPath> GetSvgPath(ElementReference element, double percent = 100)
    {
        var pathJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierGetSvgPath,
            element,
            percent
        );
        return new SvgPath(pathJsRef);
    }

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
    /// Immediately sets values to the specified targets.
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="build"></param>
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
    /// Converts a pixel value to another unit using <c>anime.convertPx</c>.
    /// </summary>
    public async Task<double> ConvertPx(ElementReference element, string value, string unit) =>
        await JsRuntime.InvokeAsync<double>(IdentifierConvertPx, element, value, unit);

    /// <summary>
    /// Converts a pixel value to another unit using <c>anime.convertPx</c>.
    /// </summary>
    public async Task<double> ConvertPx(string selector, string value, string unit) =>
        await JsRuntime.InvokeAsync<double>(IdentifierConvertPx, selector, value, unit);

    /// <summary>
    /// Sets <c>stroke-dasharray</c> to the path length and returns that length. This is <c>anime.setDashoffset</c>.
    /// </summary>
    public async Task<double> SetDashoffset(ElementReference element) =>
        await JsRuntime.InvokeAsync<double>(IdentifierSetDashoffset, element);

    /// <summary>
    /// Sets <c>stroke-dasharray</c> to the path length and returns that length.
    /// </summary>
    public async Task<double> SetDashoffset(string selector) =>
        await JsRuntime.InvokeAsync<double>(IdentifierSetDashoffset, selector);

    /// <summary>
    /// Get the original value of an element.
    /// <para>
    /// Since anime.js uses getComputedStyle to access original CSS, the values are almost always returned in 'px',
    /// </para>
    /// </summary>
    /// <param name="target"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
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
    /// Returns a random integer within a specific range.
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public async Task<int> Random(int minValue, int maxValue)
    {
        return await JsRuntime.InvokeAsync<int>(
            IdentifierGetRandomValue,
            minValue,
            maxValue
        );
    }

    /// <summary>
    /// Returns the number of all active anime.js instances currently running.
    /// </summary>
    /// <returns></returns>
    public async Task<int> RunningLength()
    {
        return await JsRuntime.InvokeAsync<int>(IdentifierGetRunningLength);
    }

    /// <summary>
    /// By default, all animations are paused when switching tabs, 
    /// <br/>useful if you want to make sure the user sees everything and doesn't miss an important part of your animation.
    /// <br/>But you can choose to let the animation runs normally without any pause, 
    /// <br/>like a video or an audio track that can continuously plays in the background.
    /// </summary>
    /// <param name="value"></param>
    public async Task SuspendWhenDocumentHidden(bool value)
    {
        await JsRuntime.InvokeVoidAsync(IdentifierSuspendWhenDocHidden, value);
    }

    /// <summary>
    /// Whether animations pause while the document is hidden. anime.js defaults this to true.
    /// </summary>
    public async Task<bool> GetSuspendWhenDocumentHidden() =>
        await JsRuntime.InvokeAsync<bool>(IdentifierGetSuspendWhenDocHidden);

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
    private const string IdentifierGetRunningLength = "AnimeJs.runningLength";
    private const string IdentifierGetSvgPath = "AnimeJs.path";
    private const string IdentifierSuspendWhenDocHidden = "AnimeJs.suspendWhenDocumentHidden";
    private const string IdentifierGetSuspendWhenDocHidden = "AnimeJs.getSuspendWhenDocumentHidden";
    private const string IdentifierRemove = "AnimeJs.remove";
    private const string IdentifierGetSpeed = "AnimeJs.getSpeed";
    private const string IdentifierSetSpeed = "AnimeJs.setSpeed";
    private const string IdentifierVersion = "AnimeJs.version";
    private const string IdentifierConvertPx = "AnimeJs.convertPx";
    private const string IdentifierSetDashoffset = "AnimeJs.setDashoffset";
    private const string IdentifierCreateObject = "AnimeJs.createObject";
}

public interface IAnime
{
    Task<Animation> Animate(Func<PropsBuilder, PropsBuilder> configure);
    Task<Animation> Animate(Func<PropsBuilder, Task<PropsBuilder>> configure);
    Task<Timeline> Timeline(Func<PropsBuilder, PropsBuilder> configureDefaults);
    Task<Timeline> Timeline(Func<PropsBuilder, Task<PropsBuilder>> configureDefaults);
    Task<SvgPath> GetSvgPath(string svgSelector, double percent = 100);
    Task<SvgPath> GetSvgPath(ElementReference element, double percent = 100);
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
    Task<double> ConvertPx(ElementReference element, string value, string unit);
    Task<double> ConvertPx(string selector, string value, string unit);
    Task<double> SetDashoffset(ElementReference element);
    Task<double> SetDashoffset(string selector);
    Task SuspendWhenDocumentHidden(bool value);
    Task<bool> GetSuspendWhenDocumentHidden();
    Task<int> RunningLength();
}

public static class PropExtensions
{
    internal static Dictionary<string, object> ToObject(this IEnumerable<Prop> props)
    {
        return props.ToDictionary(p => p.GetName(), p => p.GetValue());
    }
}