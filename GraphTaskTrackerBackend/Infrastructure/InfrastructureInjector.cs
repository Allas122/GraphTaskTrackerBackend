using System.Text;
using FluentValidation;
using GraphTaskTrackerBackend.Application.Validators;
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

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });
        services.AddAuthorization();
        services.AddControllers();
        services.AddDbContext<DatabaseContext>(ob => { ob.UseNpgsql(connectionString); });
        services.AddScoped<IJwtService, JwtService>();
        services.AddEndpointsApiExplorer();
        services.AddValidatorsFromAssemblyContaining<UserRegistrationRequestValidator>();
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, ct) =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes.Add("Bearer", securityScheme);
                return Task.CompletedTask;
            });
            options.AddOperationTransformer((operation, context, ct) =>
            {
                var authAttributes = context.Description.ActionDescriptor.EndpointMetadata
                    .OfType<AuthorizeAttribute>();

                if (authAttributes.Any())
                {
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new() {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference 
                                    { 
                                        Type = ReferenceType.SecurityScheme, 
                                        Id = "Bearer" 
                                    }
                                },
                                Array.Empty<string>()
                            }
                        }
                    };
                }
                return Task.CompletedTask;
            });
        });
        return services;
    }
}