namespace GraphTaskTrackerBackend.Application.Exceptions;

public class Forbidden : ApplicationExceptionBase
{
    public Forbidden(string message) : base(message)
    {
    }
}