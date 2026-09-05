using System.Text.Json;
using EagleBank.Api.ExceptionHandling;
using EagleBank.Api.Middleware;
using EagleBank.Api.Validation;
using EagleBank.Application;
using EagleBank.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services
    .AddControllers(options => options.Filters.Add<FluentValidationActionFilter>())
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services.AddExceptionHandler<EagleBankExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment()
    && !string.Equals(app.Configuration["SkipMigrations"], "true", StringComparison.OrdinalIgnoreCase))
{
    Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "data"));
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EagleBank.Infrastructure.Persistence.EagleBankDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<RequestContextMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
