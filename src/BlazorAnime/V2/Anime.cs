using Microsoft.JSInterop;

namespace BlazorAnime.V2;

public class Anime(IJSRuntime jSRuntime) : IAnime
{
    public async Task<Animation> Animate(params IEnumerable<AnimeProp> props)
    {
        var animationJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierCreateAnimation,
            props.GetValue()
        );
        return new Animation(animationJsRef);
    }

    public async Task<Timeline> Timeline(params IEnumerable<AnimeProp> props)
    {
        var timelineJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierCreateTimeline,
            props.GetValue()
        );
        return new Timeline(timelineJsRef);
    }

    public async Task<string> Get(object target, string propName)
    {
        return await JsRuntime.InvokeAsync<string>(
            IdentifierGetElementValue,
            target,
            propName,
            null
        );
    }

    public async Task<double> Get(object target, string propName, string cssUnit)
    {
        return await JsRuntime.InvokeAsync<double>(
            IdentifierGetElementValue,
            target,
            propName,
            cssUnit
        );
    }

    public async Task<Path> Path(string target)
    {
        var pathJsRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            IdentifierGetPath,
            target
        );
        return new Path(pathJsRef);
    }

    public async Task<int> Random(int minValue, int maxValue)
    {
        return await JsRuntime.InvokeAsync<int>(
            IdentifierGetRandomValue,
            minValue,
            maxValue
        );
    }

    public async Task<int> RunningLength()
    {
        return await JsRuntime.InvokeAsync<int>(IdentifierGetRunningLength);
    }

    public async Task Set(string[] targets, IEnumerable<AnimeProp> props)
    {
        await JsRuntime.InvokeVoidAsync(IdentifierSetElementValue, targets, props.GetValue());
    }

    public async Task Set(object[] targets, IEnumerable<AnimeProp> props)
    {
        await JsRuntime.InvokeVoidAsync(IdentifierSetElementValue, targets, props.GetValue());
    }

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
    
    Task<Animation> Animate(params IEnumerable<AnimeProp> props);
    
    Task<Timeline> Timeline(params IEnumerable<AnimeProp> props);

    Task Set(string[] targets, IEnumerable<AnimeProp> props);

    Task Set(object[] targets, IEnumerable<AnimeProp> props);

    Task<string> Get(object target, string propName);

    Task<double> Get(object target, string propName, string cssUnit);

    Task<int> Random(int minValue, int maxValue);
    
    Task SuspendWhenDocumentHidden(bool value);

    Task<int> RunningLength();
}
