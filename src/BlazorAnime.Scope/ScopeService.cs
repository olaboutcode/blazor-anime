using Microsoft.JSInterop;

namespace BlazorAnime;

public sealed class ScopeService(IJSRuntime jsRuntime) : IScope
{
    public Task<Scope> Create() => Create(static builder => builder);

    public async Task<Scope> Create(Func<ScopeBuilder, ScopeBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(new ScopeBuilder()) ?? throw new ArgumentNullException(nameof(configure));
        var reference = await Js.InvokeAsync<IJSObjectReference>(
            IdentifierCreate,
            builder.HasRoot ? builder.RootValue : null,
            builder.DefaultsValue ?? new Dictionary<string, object>(),
            builder.MediaQueries);
        return new Scope(reference);
    }

    private IJSRuntime Js { get; } = jsRuntime;
    private const string IdentifierCreate = "AnimeJsScope.create";
}

public interface IScope
{
    Task<Scope> Create();
    Task<Scope> Create(Func<ScopeBuilder, ScopeBuilder> configure);
}
