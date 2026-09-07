using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EagleBank.Tests.Support;

namespace EagleBank.Tests.Unit;

public class HostSmokeTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public HostSmokeTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_health_returns_ok()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Request_log_uses_route_template_and_honors_request_id()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Request-Id", "req-test-001");

        var response = await client.GetAsync("/health?email=hidden@example.com");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("req-test-001", response.Headers.GetValues("X-Request-Id").Single());

        var logs = string.Join(Environment.NewLine, _factory.LogSink.Entries);
        Assert.Contains("GET", logs);
        Assert.Contains("/health", logs);
        Assert.DoesNotContain("hidden@example.com", logs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("?email=", logs, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Get_submitted_openapi_includes_login()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/openapi.yaml");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("/v1/auth/login", body);
        Assert.Contains("password", body);
        Assert.Contains("Implemented in this repository", body);
        Assert.Contains("PATCH", body);
        Assert.DoesNotContain("swagger/v1/swagger.json", body);
    }

    [Fact]
    public async Task Get_swagger_ui_is_reachable()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/index.html");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("swagger-ui", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Wired in this repository", body);
        Assert.Contains("PATCH", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Authenticated_request_log_scope_includes_user_id()
    {
        var client = _factory.CreateClient();
        var create = await client.PostAsync("/v1/users", UserFixtures.JsonBody(new
        {
            name = "Scope User",
            address = new
            {
                line1 = "1 Scope Road",
                town = "London",
                county = "Greater London",
                postcode = "SW1A 1AA"
            },
            phoneNumber = "+441111111111",
            email = "scope-user@example.com",
            password = "S3cretPass!"
        }));
        create.EnsureSuccessStatusCode();
        using var created = JsonDocument.Parse(await create.Content.ReadAsStringAsync());
        var userId = created.RootElement.GetProperty("id").GetString()!;

        var login = await client.PostAsJsonAsync(
            "/v1/auth/login",
            new { email = "scope-user@example.com", password = "S3cretPass!" },
            UserFixtures.JsonOptions);
        login.EnsureSuccessStatusCode();
        using var tokenDoc = JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDoc.RootElement.GetProperty("token").GetString());

        var response = await client.GetAsync($"/v1/users/{userId}");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var completed = _factory.LogSink.Entries.FirstOrDefault(entry =>
            entry.Contains("HTTP request completed", StringComparison.Ordinal)
            && entry.Contains("GET", StringComparison.Ordinal)
            && entry.Contains("users/{userId}", StringComparison.Ordinal));

        Assert.False(string.IsNullOrWhiteSpace(completed));
        Assert.Contains($"userId={userId}", completed);
        Assert.Contains("RequestId=", completed);
    }
}
