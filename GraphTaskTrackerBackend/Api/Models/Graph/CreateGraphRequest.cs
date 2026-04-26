namespace GraphTaskTrackerBackend.Api.Models;

public class CreateGraphRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public TimeSpan Time { get; set; } = new TimeSpan(0, 0, 0);
}