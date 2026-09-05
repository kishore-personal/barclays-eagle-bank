using EagleBank.Application.Accounts.CreateAccount;
using EagleBank.Application.Accounts.DeleteAccount;
using EagleBank.Application.Accounts.GetAccount;
using EagleBank.Application.Accounts.ListAccounts;
using EagleBank.Application.Accounts.UpdateAccount;
using EagleBank.Application.Auth.Login;
using EagleBank.Application.Errors;
using EagleBank.Application.Transactions.CreateTransaction;
using EagleBank.Application.Transactions.GetTransaction;
using EagleBank.Application.Transactions.ListTransactions;
using EagleBank.Application.Users.CreateUser;
using EagleBank.Application.Users.DeleteUser;
using EagleBank.Application.Users.GetUser;
using EagleBank.Application.Users.UpdateUser;
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
        services.AddScoped<IGetUserHandler, GetUserHandler>();
        services.AddScoped<IUpdateUserHandler, UpdateUserHandler>();
        services.AddScoped<IDeleteUserHandler, DeleteUserHandler>();
        services.AddScoped<ICreateAccountHandler, CreateAccountHandler>();
        services.AddScoped<IGetAccountHandler, GetAccountHandler>();
        services.AddScoped<IListAccountsHandler, ListAccountsHandler>();
        services.AddScoped<IUpdateAccountHandler, UpdateAccountHandler>();
        services.AddScoped<IDeleteAccountHandler, DeleteAccountHandler>();
        services.AddScoped<ICreateTransactionHandler, CreateTransactionHandler>();
        services.AddScoped<IGetTransactionHandler, GetTransactionHandler>();
        services.AddScoped<IListTransactionsHandler, ListTransactionsHandler>();
        services.AddScoped<ILoginHandler, LoginHandler>();
        return services;
    }
}
