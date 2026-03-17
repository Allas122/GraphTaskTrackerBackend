namespace GraphTaskTrackerBackend.Domain.Entities;

public class Edge
{
    public Guid FromNodeId { get; set; }
    public Guid ToNodeId { get; set; }
    public Node FromNode { get; set; }
    public Node ToNode { get; set; }

    public long Weight { get; set; }
}