using EagleBank.Application.Errors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBank.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionResponseFactory, ExceptionResponseFactory>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
