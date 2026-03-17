namespace GraphTaskTrackerBackend.Api.Models;

public class ValidationErrorResponse
{
    public string Title { get; set; } = "One or more validation errors occurred.";
    public int Status { get; set; } = 400;
    public Dictionary<string, string[]> Errors { get; set; } = new();
}