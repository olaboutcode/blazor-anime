namespace BlazorAnime;

/// <summary>
/// Snapshot of a draggable passed to grab, drag, release, and settle callbacks.
/// </summary>
public sealed class DragState
{
    public double X { get; init; }
    public double Y { get; init; }
    public double ProgressX { get; init; }
    public double ProgressY { get; init; }
    public double Velocity { get; init; }
    public double Angle { get; init; }
    public bool Grabbed { get; init; }
    public bool Dragged { get; init; }
    public bool Enabled { get; init; }
}
