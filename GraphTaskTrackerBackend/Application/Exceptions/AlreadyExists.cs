namespace GraphTaskTrackerBackend.Application.Exceptions;

public class AlreadyExistsException : ApplicationExceptionBase
{
    public AlreadyExistsException(string message) : base(message)
    {
    }
}