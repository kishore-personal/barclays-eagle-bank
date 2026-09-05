using EagleBank.Application.Auth.Login;
using EagleBank.Application.Errors;
using EagleBank.Application.Users.CreateUser;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBank.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionResponseFactory, ExceptionResponseFactory>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
        services.AddScoped<ILoginHandler, LoginHandler>();
        return services;
    }
}
