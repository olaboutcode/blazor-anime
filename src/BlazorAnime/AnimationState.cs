namespace BlazorAnime;

public sealed class AnimationState
{
    public int Id { get; init; }
    public float Value { get; init; }
    public float Progress { get; init; }
    public bool Began { get; init; }
    public bool Completed { get; init; }
    public bool ChangeBegan { get; init; }
    public bool ChangeCompleted { get; init; }
    public bool LoopBegan { get; init; }
    public bool Paused { get; init; }
    public bool Reversed { get; init; }
    public double Duration { get; init; }
    public double Delay { get; init; }
    public string Direction { get; init; } = string.Empty;
}
