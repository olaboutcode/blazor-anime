using Microsoft.JSInterop;

namespace BlazorAnime;

public class Anime(IJSRuntime jSRuntime) : IAnime
{
    /// <summary>
    /// Create an animation.
    /// </summary>
    /// <param name="props"></param>
    /// <returns>Animation</returns>
    public async Task<Animation> Animate(params Prop[] props)
    {
        var animationJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierCreateAnimation,
            props.ToObject()
        );
        return new Animation(animationJsRef);
    }
    
    /// <summary>
    /// Create an animation timeline.
    /// </summary>
    /// <param name="defaultProps"></param>
    /// <returns>Timeline</returns>
    public async Task<Timeline> Timeline(params Prop[] defaultProps)
    {
        var timelineJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierCreateTimeline,
            defaultProps.ToObject()
        );
        return new Timeline(timelineJsRef);
    }

    public async Task<Animation> Animate(Func<PropsBuilder, PropsBuilder> configure)
    {
        var animationJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierCreateAnimation,
            configure(new PropsBuilder()).Build().ToObject()
        );
        return new Animation(animationJsRef);
    }

    public async Task<Timeline> Timeline(Func<PropsBuilder, PropsBuilder> configureDefault)
    {
        var timelineJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierCreateTimeline,
            configureDefault(new PropsBuilder()).Build().ToObject()
        );
        return new Timeline(timelineJsRef);
    }

    /// <summary>
    /// Returns a new Path that returns the specified property.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public async Task<Path> Path(string target)
    {
        var pathJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierGetPath,
            target
        );
        return new Path(pathJsRef);
    }

    /// <summary>
    /// Immediately sets values to the specified targets.
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="props"></param>
    public async Task Set(string[] targets, List<Prop> props)
    {
        await JsRuntime.InvokeVoidAsync(IdentifierSetElementValue, targets, props.ToObject());
    }
    
    /// <summary>
    /// Immediately sets values to the specified targets.
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="props"></param>
    public async Task Set(object[] targets, List<Prop> props)
    {
        await JsRuntime.InvokeVoidAsync(IdentifierSetElementValue, targets, props.ToObject());
    }

    /// <summary>
    /// Get the original value of an element.
    /// <para>
    /// Since anime.js uses getComputedStyle to access original CSS, the values are almost always returned in 'px',
    /// </para>
    /// </summary>
    /// <param name="target"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
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

    private IJSRuntime JsRuntime { get; } = jSRuntime;

    private const string IdentifierCreateAnimation = "AnimeJs.createAnimation";
    private const string IdentifierCreateTimeline = "AnimeJs.createTimeline";
    private const string IdentifierSetElementValue = "AnimeJs.set";
    private const string IdentifierGetElementValue = "AnimeJs.get";
    private const string IdentifierGetRandomValue = "AnimeJs.random";
    private const string IdentifierGetRunningLength = "AnimeJs.runningLength";
    private const string IdentifierGetPath = "AnimeJs.path";
    private const string IdentifierSuspendWhenDocHidden = "AnimeJs.suspendWhenDocumentHidden";
}

public interface IAnime
{
    Task<Path> Path(string target);
    
    Task<Animation> Animate(params Prop[] props);
    
    Task<Timeline> Timeline(params Prop[] props);

    Task Set(string[] targets, List<Prop> props);

    Task Set(object[] targets, List<Prop> props);

    Task<string> Get(object target, string propName);

    Task<double> Get(object target, string propName, string cssUnit);

    Task<int> Random(int minValue, int maxValue);
    
    Task SuspendWhenDocumentHidden(bool value);

    Task<int> RunningLength();
}