namespace GraphTaskTrackerBackend.Infrastructure.Options;

public static class OptionsInjector
{
    public static IServiceCollection AddOptionsPart(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}