namespace BlazorAnime;

/// <summary>Snapshot passed to <c>onComplete</c>. It is not the live animation.</summary>
public sealed class WaapiState
{
    public double CurrentTime { get; set; }
    public double Progress { get; set; }
    public double Duration { get; set; }
    public bool Completed { get; set; }
    public bool Paused { get; set; }
    public bool Reversed { get; set; }
}
