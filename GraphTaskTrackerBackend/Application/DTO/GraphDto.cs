namespace GraphTaskTrackerBackend.Application.DTO;

public class GraphDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public UserDto User { get; set; }
    public Guid UserId { get; set; }

    public ICollection<NodeDto> Nodes { get; set; } =  new List<NodeDto>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}