namespace GraphTaskTrackerBackend.Api.Models;

public class SyncGraphRequest
{
    public Guid GraphId { get; set; }
    public List<CreateNodeMessage> Nodes { get; set; }
    public List<EdgeMessage> Edges { get; set; }
}