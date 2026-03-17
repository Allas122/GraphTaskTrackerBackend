namespace GraphTaskTrackerBackend.Application.DTO;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
}