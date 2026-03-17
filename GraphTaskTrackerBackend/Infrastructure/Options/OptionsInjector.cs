namespace GraphTaskTrackerBackend.Infrastructure.Options;

public static class OptionsInjector
{
    public static IServiceCollection AddOptionsPart(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));
        return services;
    }
}