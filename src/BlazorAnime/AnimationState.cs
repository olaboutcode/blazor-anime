namespace BlazorAnime;

/// <summary>
/// Snapshot of an anime.js instance passed to lifecycle callbacks.
/// </summary>
public sealed class AnimationState
{
    public string Id { get; init; } = "";
    public float Progress { get; init; }
    public bool Began { get; init; }
    public bool Completed { get; init; }
    public bool Paused { get; init; }
    public bool Reversed { get; init; }
    public bool Backwards { get; init; }
    public bool Alternate { get; init; }
    public double Duration { get; init; }
    public double Delay { get; init; }
    public double LoopDelay { get; init; }
    public double CurrentTime { get; init; }
    public float IterationProgress { get; init; }
    public int CurrentIteration { get; init; }

    /// <summary>Configured loop count. -1 when the animation loops forever.</summary>
    public int Loop { get; init; }
}
