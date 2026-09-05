using System.Text.Json;
using EagleBank.Api.Authentication;
using EagleBank.Api.ExceptionHandling;
using EagleBank.Api.Middleware;
using EagleBank.Api.OpenApi;
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
builder.Services.AddEagleBankAuthentication(builder.Configuration);

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
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Eagle Bank";
    options.SwaggerEndpoint(SubmittedOpenApi.DocumentPath, "Eagle Bank");
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet(SubmittedOpenApi.DocumentPath, (IWebHostEnvironment environment) =>
    Results.File(SubmittedOpenApi.ResolveFilePath(environment), "application/yaml", "openapi.yaml"));

app.Run();

public partial class Program;
