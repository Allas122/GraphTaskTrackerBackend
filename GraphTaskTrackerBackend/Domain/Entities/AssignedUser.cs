namespace GraphTaskTrackerBackend.Domain.Entities;

public class AssignedUser
{
    public Guid UserId { get; set; }
    public Guid NodeId { get; set; }
    
    public User User { get; set; }
    public Node Node { get; set; }
}