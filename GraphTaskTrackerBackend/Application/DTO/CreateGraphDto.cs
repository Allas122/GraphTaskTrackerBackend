namespace GraphTaskTrackerBackend.Application.DTO;

public class CreateGraphDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
}