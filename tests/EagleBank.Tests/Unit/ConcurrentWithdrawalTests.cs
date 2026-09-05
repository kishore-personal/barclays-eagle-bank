using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EagleBank.Infrastructure.Persistence;
using EagleBank.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBank.Tests.Unit;

public class ConcurrentWithdrawalTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public ConcurrentWithdrawalTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Parallel_full_balance_withdrawals_allow_only_one_debit()
    {
        var client = _factory.CreateClient();
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EagleBankDbContext>();
            await db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");
            await db.Database.ExecuteSqlRawAsync("PRAGMA busy_timeout=5000;");
        }

        var createUser = await client.PostAsync("/v1/users", UserFixtures.JsonBody(new
        {
            name = "Concurrent User",
            address = new
            {
                line1 = "1 Race Road",
                town = "London",
                county = "Greater London",
                postcode = "E1 6AN"
            },
            phoneNumber = "+442222222222",
            email = "concurrent-user@example.com",
            password = "S3cretPass!"
        }));
        createUser.EnsureSuccessStatusCode();

        var login = await client.PostAsJsonAsync(
            "/v1/auth/login",
            new { email = "concurrent-user@example.com", password = "S3cretPass!" },
            UserFixtures.JsonOptions);
        login.EnsureSuccessStatusCode();
        using var tokenDoc = JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        var token = tokenDoc.RootElement.GetProperty("token").GetString()!;

        using var authenticated = _factory.CreateClient();
        authenticated.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createAccount = await authenticated.PostAsync(
            "/v1/accounts",
            UserFixtures.JsonBody(AccountFixtures.ValidCreateBody()));
        createAccount.EnsureSuccessStatusCode();
        using var accountDoc = JsonDocument.Parse(await createAccount.Content.ReadAsStringAsync());
        var accountNumber = accountDoc.RootElement.GetProperty("accountNumber").GetString()!;

        var deposit = await authenticated.PostAsync(
            $"/v1/accounts/{accountNumber}/transactions",
            UserFixtures.JsonBody(TransactionFixtures.DepositBody(10.00m)));
        deposit.EnsureSuccessStatusCode();

        using var first = _factory.CreateClient();
        first.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var second = _factory.CreateClient();
        second.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var path = $"/v1/accounts/{accountNumber}/transactions";

        var firstTask = first.PostAsync(path, UserFixtures.JsonBody(TransactionFixtures.WithdrawalBody(10.00m)));
        var secondTask = second.PostAsync(path, UserFixtures.JsonBody(TransactionFixtures.WithdrawalBody(10.00m)));
        var responses = await Task.WhenAll(firstTask, secondTask);

        var statuses = responses.Select(response => response.StatusCode).ToArray();
        Assert.Contains(HttpStatusCode.Created, statuses);
        Assert.Contains((HttpStatusCode)422, statuses);

        var fetch = await authenticated.GetAsync($"/v1/accounts/{accountNumber}");
        fetch.EnsureSuccessStatusCode();
        using var fetched = JsonDocument.Parse(await fetch.Content.ReadAsStringAsync());
        var balance = fetched.RootElement.GetProperty("balance").GetDecimal();

        Assert.True(balance >= 0m);
        Assert.Equal(0.00m, balance);
    }
}
