namespace GraphTaskTrackerBackend.Api.Models;

public class MakeAssignedRequest
{
    public Guid UserId { get; set; }
    public Guid NodeId { get; set; }
}