using GraphTaskTrackerBackend.Application;
using GraphTaskTrackerBackend.Infrastructure;
using GraphTaskTrackerBackend.Infrastructure.Middlewares;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}

app.UseHttpsRedirection();

app.Run();