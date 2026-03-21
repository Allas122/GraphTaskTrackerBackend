using GraphTaskTrackerBackend.Application;
using GraphTaskTrackerBackend.Infrastructure;
using GraphTaskTrackerBackend.Infrastructure.DataBase;
using GraphTaskTrackerBackend.Infrastructure.Middlewares;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

var app = builder.Build();
var apiPrefix = app.Configuration["APP_BASEPATH"];

if (!string.IsNullOrEmpty(apiPrefix))
{
    app.UsePathBase(apiPrefix);
}

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}

app.Run();