namespace GraphTaskTrackerBackend.Api.Models;

public class GraphCartResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
        
    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}