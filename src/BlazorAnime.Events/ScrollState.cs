namespace BlazorAnime;

/// <summary>Snapshot passed to a scroll callback. It is not the live observer.</summary>
public sealed class ScrollState
{
    public double Progress { get; set; }
    public bool IsInView { get; set; }
    public bool Began { get; set; }
    public bool Completed { get; set; }
    public bool Backward { get; set; }
    public double Velocity { get; set; }
    public double Scroll { get; set; }
}
