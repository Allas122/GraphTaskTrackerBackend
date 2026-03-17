namespace GraphTaskTrackerBackend.Infrastructure.Security.Abstractions;

public interface IJwtService
{
    string GenerateToken(Guid userId);
}