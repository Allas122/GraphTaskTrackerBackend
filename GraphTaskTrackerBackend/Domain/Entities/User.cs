namespace GraphTaskTrackerBackend.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? FullName { get; set; }
    public string PasswordHash { get; set; }
    
    public ICollection<Graph> Graphs { get; set; }
    public ICollection<Node> AssignedNodes { get; set; }
    public ICollection<Node> AuthorNodes { get; set; }
}