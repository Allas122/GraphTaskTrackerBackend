namespace GraphTaskTrackerBackend.Application.DTO;

public class EdgeDto
{
    public Guid FromNodeId { get; set; }
    public Guid ToNodeId { get; set; }
    public long Weight { get; set; }
}