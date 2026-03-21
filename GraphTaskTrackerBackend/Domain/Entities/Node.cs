namespace GraphTaskTrackerBackend.Domain.Entities;

public class Node
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<Node> Nodes { get; set; }

    public Guid GraphId { get; set; }
    public Graph Graph { get; set; }
}