using GraphTaskTrackerBackend.Application.Services.Abstractions;
using GraphTaskTrackerBackend.Application.Services.Implementations;

namespace GraphTaskTrackerBackend.Application;

public static class ApplicationInjector
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGraphService, GraphService>();
        return services;
    }
}