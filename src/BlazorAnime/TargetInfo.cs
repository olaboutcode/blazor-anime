namespace BlazorAnime;

/// <summary>
/// Snapshot of one animation target, taken when the animation is created.
/// anime.js reads the element only while it builds the tween, so the callback
/// receives the attributes from that moment.
/// </summary>
public sealed record TargetInfo(
    int Index,
    int Total,
    string Id,
    string TagName,
    IReadOnlyDictionary<string, string> Dataset);
