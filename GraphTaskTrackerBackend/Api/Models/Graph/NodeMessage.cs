namespace GraphTaskTrackerBackend.Api.Models;

public class NodeMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public TimeSpan Time { get; set; }
    
    public UserMessage Author { get; set; }
    public List<UserMessage> Assigned { get; set; }
}