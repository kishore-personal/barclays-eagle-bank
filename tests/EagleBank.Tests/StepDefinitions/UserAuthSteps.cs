using System.Text.Json;
using EagleBank.Tests.Support;
using Reqnroll;

namespace EagleBank.Tests.StepDefinitions;

[Binding]
public sealed class UserAuthSteps
{
    private readonly ScenarioContext _scenarioContext;

    public UserAuthSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("a registered user exists")]
    public async Task GivenARegisteredUserExists()
    {
        var response = await Client.PostAsync("/v1/users", UserFixtures.JsonBody(UserFixtures.ValidCreateBody()));
        response.EnsureSuccessStatusCode();
    }

    [When("I create a user with all the required data")]
    public async Task WhenICreateAUserWithAllTheRequiredData()
    {
        await PostAsync("/v1/users", UserFixtures.ValidCreateBody());
    }

    [When("I create a user with missing required data")]
    public async Task WhenICreateAUserWithMissingRequiredData()
    {
        await PostAsync("/v1/users", new { name = UserFixtures.Name });
    }

    [When("I create a user with an invalid phone number")]
    public async Task WhenICreateAUserWithAnInvalidPhoneNumber()
    {
        await PostAsync("/v1/users", new
        {
            name = UserFixtures.Name,
            address = new
            {
                line1 = UserFixtures.Line1,
                town = UserFixtures.Town,
                county = UserFixtures.County,
                postcode = UserFixtures.Postcode
            },
            phoneNumber = "012345",
            email = UserFixtures.Email,
            password = UserFixtures.Password
        });
    }

    [When("I create a user with an invalid email")]
    public async Task WhenICreateAUserWithAnInvalidEmail()
    {
        await PostAsync("/v1/users", new
        {
            name = UserFixtures.Name,
            address = new
            {
                line1 = UserFixtures.Line1,
                town = UserFixtures.Town,
                county = UserFixtures.County,
                postcode = UserFixtures.Postcode
            },
            phoneNumber = UserFixtures.Phone,
            email = "not-an-email",
            password = UserFixtures.Password
        });
    }

    [When("I log in with valid credentials")]
    public async Task WhenILogInWithValidCredentials()
    {
        await PostAsync("/v1/auth/login", new { email = UserFixtures.Email, password = UserFixtures.Password });
    }

    [When("I log in with a malformed body")]
    public async Task WhenILogInWithAMalformedBody()
    {
        await PostAsync("/v1/auth/login", new { email = UserFixtures.Email });
    }

    [When("I log in with an unknown email")]
    public async Task WhenILogInWithAnUnknownEmail()
    {
        await PostAsync("/v1/auth/login", new { email = "unknown@example.com", password = UserFixtures.Password });
    }

    [When("I log in with a wrong password")]
    public async Task WhenILogInWithAWrongPassword()
    {
        await PostAsync("/v1/auth/login", new { email = UserFixtures.Email, password = "WrongPass1" });
    }

    [Then("the response is a UserResponse")]
    public void ThenTheResponseIsAUserResponse()
    {
        using var document = JsonDocument.Parse(Body);
        var root = document.RootElement;
        Assert.True(root.TryGetProperty("id", out var id));
        Assert.StartsWith("usr-", id.GetString());
        Assert.Equal(UserFixtures.Name, root.GetProperty("name").GetString());
        Assert.False(root.TryGetProperty("password", out _));
        Assert.False(root.TryGetProperty("passwordHash", out _));
    }

    [Then("the user response has no password")]
    public void ThenTheUserResponseHasNoPassword()
    {
        Assert.DoesNotContain("password", Body, StringComparison.OrdinalIgnoreCase);
    }

    [Then("the response is a BadRequestErrorResponse")]
    public void ThenTheResponseIsABadRequestErrorResponse()
    {
        using var document = JsonDocument.Parse(Body);
        Assert.True(document.RootElement.TryGetProperty("message", out _));
        Assert.True(document.RootElement.TryGetProperty("details", out var details));
        Assert.True(details.GetArrayLength() > 0);
    }

    [Then("the response is a token payload")]
    public void ThenTheResponseIsATokenPayload()
    {
        using var document = JsonDocument.Parse(Body);
        var token = document.RootElement.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Contains('.', token);
    }

    private async Task PostAsync(string path, object body)
    {
        var response = await Client.PostAsync(path, UserFixtures.JsonBody(body));
        _scenarioContext.Set(response);
        _scenarioContext.Set(await response.Content.ReadAsStringAsync(), "ResponseBody");
    }

    private HttpClient Client => _scenarioContext.Get<HttpClient>();

    private string Body => _scenarioContext.Get<string>("ResponseBody");
}
