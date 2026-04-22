namespace GraphTaskTrackerBackend.Api.Models;

public class SyncGraphResponse
{
    public Guid GraphId { get; set; }
    public ICollection<NodeMessage> Nodes { get; set; }
    public ICollection<EdgeMessage> Edges { get; set; }
}