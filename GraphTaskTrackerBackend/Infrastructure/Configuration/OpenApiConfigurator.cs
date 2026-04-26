using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace GraphTaskTrackerBackend.Infrastructure.Configuration;

public static class OpenApiConfigurator
{
    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services, string apiPrefix)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, ct) =>
            {
                if (!string.IsNullOrEmpty(apiPrefix))
                {
                    document.Servers = new List<OpenApiServer> { new OpenApiServer { Url = apiPrefix } };
                }
                
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