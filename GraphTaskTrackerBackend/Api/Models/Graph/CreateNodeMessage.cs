namespace GraphTaskTrackerBackend.Api.Models;

public class CreateNodeMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}