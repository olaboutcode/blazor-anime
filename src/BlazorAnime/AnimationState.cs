namespace BlazorAnime;

/// <summary>
/// Snapshot of an anime.js instance passed to lifecycle callbacks.
/// </summary>
public sealed class AnimationState
{
    public int Id { get; init; }
    public float Progress { get; init; }
    public bool Began { get; init; }
    public bool Completed { get; init; }
    public bool ChangeBegan { get; init; }
    public bool ChangeCompleted { get; init; }
    public bool LoopBegan { get; init; }
    public bool Paused { get; init; }
    public bool Reversed { get; init; }
    public bool ReversePlayback { get; init; }
    public double Duration { get; init; }
    public double Delay { get; init; }
    public double EndDelay { get; init; }
    public double CurrentTime { get; init; }

    /// <summary>Loops still left. -1 when the animation loops forever.</summary>
    public int Remaining { get; init; }

    /// <summary>Configured loop count. -1 when the animation loops forever.</summary>
    public int Loop { get; init; }

    public string Direction { get; init; } = string.Empty;
}
