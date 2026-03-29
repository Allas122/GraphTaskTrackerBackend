namespace GraphTaskTrackerBackend.Application.Exceptions;

public class Conflict : ApplicationExceptionBase
{
    public Conflict(string message) : base(message)
    {
    }
}