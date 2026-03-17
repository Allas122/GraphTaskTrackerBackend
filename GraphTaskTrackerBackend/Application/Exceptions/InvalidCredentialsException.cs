namespace GraphTaskTrackerBackend.Application.Exceptions;

public class InvalidCredentialsException : ApplicationExceptionBase
{
    public InvalidCredentialsException(string message) : base(message)
    {
    }
}