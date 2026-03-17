namespace GraphTaskTrackerBackend.Domain.Entities;

public class Graph
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public User User { get; set; }
    public Guid UserId { get; set; }

    private ICollection<Node> nodes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}