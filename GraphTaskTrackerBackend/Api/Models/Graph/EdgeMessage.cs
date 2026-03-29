namespace GraphTaskTrackerBackend.Api.Models;

public class EdgeMessage
{
    public Guid FromNodeId { get; set; }
    public Guid ToNodeId { get; set; }
    public long Weight { get; set; }
}