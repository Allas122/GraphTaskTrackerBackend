namespace GraphTaskTrackerBackend.Api.Models;

public class SyncGraphRequest
{
    public Guid GraphId { get; set; }
    public ICollection<CreateNodeMessage> Nodes { get; set; }
    public ICollection<EdgeMessage> Edges { get; set; }
}