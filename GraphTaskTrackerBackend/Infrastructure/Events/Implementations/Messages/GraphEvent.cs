namespace GraphTaskTrackerBackend.Infrastructure.Events.Implementations.Messages;

public enum GraphEventType
{
    Delete,
    Update
}


public class GraphEvent
{
    public Guid GraphId { get; set; }
    public GraphEventType EventType { get; set; }
}