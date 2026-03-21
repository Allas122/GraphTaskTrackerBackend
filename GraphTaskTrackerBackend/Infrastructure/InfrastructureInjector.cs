using System.Text;
using FluentValidation;
using GraphTaskTrackerBackend.Application.Validators;
using GraphTaskTrackerBackend.Infrastructure.Configuration;
using GraphTaskTrackerBackend.Infrastructure.DataBase;
using GraphTaskTrackerBackend.Infrastructure.Options;
using GraphTaskTrackerBackend.Infrastructure.Security.Abstractions;
using GraphTaskTrackerBackend.Infrastructure.Security.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace GraphTaskTrackerBackend.Infrastructure;

public static class InfrastructureInjector
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        services.AddOptionsPart(configuration);
        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        var apiPrefix = configuration["APP_BASEPATH"] ?? "";

        services.ConfigureAuthentification(jwtSettings);
        services.AddAuthorization();
        services.AddControllers();
        services.AddDbContext<DatabaseContext>(ob => { ob.UseNpgsql(connectionString); });
        services.AddScoped<IJwtService, JwtService>();
        services.AddEndpointsApiExplorer();
        services.AddValidatorsFromAssemblyContaining<UserRegistrationRequestValidator>();
        services.ConfigureOpenApi(apiPrefix);
        return services;
    }
}