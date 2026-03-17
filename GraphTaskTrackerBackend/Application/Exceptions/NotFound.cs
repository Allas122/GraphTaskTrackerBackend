namespace GraphTaskTrackerBackend.Application.Exceptions;

public class NotFound : ApplicationExceptionBase
{
    public NotFound(string message) : base(message) { }
}