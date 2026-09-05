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
    }
}
