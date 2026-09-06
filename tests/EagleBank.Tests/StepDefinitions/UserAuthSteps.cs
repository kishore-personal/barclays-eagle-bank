using System.Net.Http.Headers;
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

    [Given("I am authenticated as a registered user")]
    public async Task GivenIAmAuthenticatedAsARegisteredUser()
    {
        var created = await RegisterAsync(UserFixtures.ValidCreateBody());
        _scenarioContext.Set(created, "AuthenticatedUserId");
        await AuthenticateAsync(UserFixtures.Email, UserFixtures.Password);
    }

    [Given("another registered user exists")]
    public async Task GivenAnotherRegisteredUserExists()
    {
        var created = await RegisterAsync(UserFixtures.OtherCreateBody());
        _scenarioContext.Set(created, "OtherUserId");
    }

    [When("I fetch the authenticated user")]
    public async Task WhenIFetchTheAuthenticatedUser()
    {
        await GetAsync($"/v1/users/{_scenarioContext.Get<string>("AuthenticatedUserId")}");
    }

    [When("I fetch the other existing user")]
    public async Task WhenIFetchTheOtherExistingUser()
    {
        await GetAsync($"/v1/users/{_scenarioContext.Get<string>("OtherUserId")}");
    }

    [When("I fetch an unknown user")]
    public async Task WhenIFetchAnUnknownUser()
    {
        await GetAsync("/v1/users/usr-doesnotexist99");
    }

    [When("I fetch a user with a bad userId")]
    public async Task WhenIFetchAUserWithABadUserId()
    {
        await GetAsync("/v1/users/not-a-valid-id");
    }

    [When(@"I fetch a user by id {string}")]
    public async Task WhenIFetchAUserById(string userId)
    {
        await GetAsync($"/v1/users/{userId}");
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

    [When("I patch the authenticated user with name {string}")]
    public async Task WhenIPatchTheAuthenticatedUserWithName(string name)
    {
        await PatchAsync($"/v1/users/{_scenarioContext.Get<string>("AuthenticatedUserId")}", new { name });
    }

    [When(@"I patch user {string} with name {string}")]
    public async Task WhenIPatchUserWithName(string userId, string name)
    {
        await PatchAsync($"/v1/users/{userId}", new { name });
    }

    [When("I patch the other existing user with name {string}")]
    public async Task WhenIPatchTheOtherExistingUserWithName(string name)
    {
        await PatchAsync($"/v1/users/{_scenarioContext.Get<string>("OtherUserId")}", new { name });
    }

    [When("I patch the authenticated user with phone number {string}")]
    public async Task WhenIPatchTheAuthenticatedUserWithPhoneNumber(string phoneNumber)
    {
        await PatchAsync($"/v1/users/{_scenarioContext.Get<string>("AuthenticatedUserId")}", new { phoneNumber });
    }

    [When("I patch the authenticated user with the other user's email")]
    public async Task WhenIPatchTheAuthenticatedUserWithTheOtherUsersEmail()
    {
        await PatchAsync(
            $"/v1/users/{_scenarioContext.Get<string>("AuthenticatedUserId")}",
            new { email = UserFixtures.OtherEmail });
    }

    [When("I delete the authenticated user")]
    public async Task WhenIDeleteTheAuthenticatedUser()
    {
        await DeleteAsync($"/v1/users/{_scenarioContext.Get<string>("AuthenticatedUserId")}");
    }

    [When(@"I delete user {string}")]
    public async Task WhenIDeleteUser(string userId)
    {
        await DeleteAsync($"/v1/users/{userId}");
    }

    [When("I delete the other existing user")]
    public async Task WhenIDeleteTheOtherExistingUser()
    {
        await DeleteAsync($"/v1/users/{_scenarioContext.Get<string>("OtherUserId")}");
    }

    [Then("the user name is {string}")]
    public void ThenTheUserNameIs(string name)
    {
        using var document = JsonDocument.Parse(Body);
        Assert.Equal(name, document.RootElement.GetProperty("name").GetString());
        Assert.False(document.RootElement.TryGetProperty("password", out _));
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
        StoreResponse(response, await response.Content.ReadAsStringAsync());
    }

    private async Task GetAsync(string path)
    {
        var response = await Client.GetAsync(path);
        StoreResponse(response, await response.Content.ReadAsStringAsync());
    }

    private async Task PatchAsync(string path, object body)
    {
        var response = await Client.PatchAsync(path, UserFixtures.JsonBody(body));
        StoreResponse(response, await response.Content.ReadAsStringAsync());
    }

    private async Task DeleteAsync(string path)
    {
        var response = await Client.DeleteAsync(path);
        StoreResponse(response, await response.Content.ReadAsStringAsync());
    }

    private async Task<string> RegisterAsync(object body)
    {
        var response = await Client.PostAsync("/v1/users", UserFixtures.JsonBody(body));
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task AuthenticateAsync(string email, string password)
    {
        var response = await Client.PostAsync(
            "/v1/auth/login",
            UserFixtures.JsonBody(new { email, password }));
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var token = document.RootElement.GetProperty("token").GetString();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private void StoreResponse(HttpResponseMessage response, string body)
    {
        _scenarioContext.Set(response);
        _scenarioContext.Set(body, "ResponseBody");
    }

    private HttpClient Client => _scenarioContext.Get<HttpClient>();

    private string Body => _scenarioContext.Get<string>("ResponseBody");
}
