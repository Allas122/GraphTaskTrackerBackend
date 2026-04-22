using System.Runtime.Intrinsics.X86;

namespace GraphTaskTrackerBackend.Domain.Entities;

public class Node
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public Guid AuthorId { get; set; }
    public User Author { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public TimeSpan Time { get; set; }
    
    public ICollection<User> Assigned { get; set; }

    public ICollection<Node> Nodes { get; set; }

    public Guid GraphId { get; set; }
    public Graph Graph { get; set; }
}