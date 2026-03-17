namespace GraphTaskTrackerBackend.Application.Exceptions;

public abstract class ApplicationExceptionBase : Exception
{
    protected ApplicationExceptionBase(string message) : base(message)
    {
    }
}