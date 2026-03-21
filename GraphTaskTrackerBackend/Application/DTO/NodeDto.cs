namespace GraphTaskTrackerBackend.Application.DTO;

public class NodeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<NodeDto> Nodes { get; set; }

    public Guid GraphId { get; set; }
    public GraphDto Graph { get; set; }
}