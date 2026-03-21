namespace GraphTaskTrackerBackend.Application.Exceptions;

public class Unprocessable: ApplicationExceptionBase
{
    public Unprocessable(string message) : base(message) { }
}