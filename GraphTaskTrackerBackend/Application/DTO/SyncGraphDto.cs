namespace GraphTaskTrackerBackend.Application.DTO;

public class SyncGraphDto
{ 
    public Guid GraphId { get; set; }
    public Guid UserId { get; set; }
    public ICollection<NodeDto> Nodes { get; set; }
    public ICollection<EdgeDto> Edges { get; set; }
}