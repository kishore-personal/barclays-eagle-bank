using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using EagleBank.Domain;
using EagleBank.Tests.Support;
using Reqnroll;

namespace EagleBank.Tests.StepDefinitions;

[Binding]
public sealed class AccountSteps
{
    private static readonly Regex AccountNumberPattern = new(@"^01\d{6}$", RegexOptions.Compiled);

    private readonly ScenarioContext _scenarioContext;

    public AccountSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("I have a personal bank account")]
    public async Task GivenIHaveAPersonalBankAccount()
    {
        var accountNumber = await CreateAccountAsync();
        _scenarioContext.Set(accountNumber, "CreatedAccountNumber");
    }

    [Given("I have a second personal bank account")]
    public async Task GivenIHaveASecondPersonalBankAccount()
    {
        var accountNumber = await CreateAccountAsync();
        _scenarioContext.Set(accountNumber, "SecondAccountNumber");
    }

    [Given("the other user has a personal bank account")]
    public async Task GivenTheOtherUserHasAPersonalBankAccount()
    {
        var previous = Client.DefaultRequestHeaders.Authorization;
        await AuthenticateAsync(UserFixtures.OtherEmail, UserFixtures.Password);
        var accountNumber = await CreateAccountAsync();
        _scenarioContext.Set(accountNumber, "OtherAccountNumber");
        Client.DefaultRequestHeaders.Authorization = previous;
    }

    [When("I create an account with the required data")]
    public async Task WhenICreateAnAccountWithTheRequiredData()
    {
        await PostAsync("/v1/accounts", AccountFixtures.ValidCreateBody());
    }

    [When("I create an account with missing required data")]
    public async Task WhenICreateAnAccountWithMissingRequiredData()
    {
        await PostAsync("/v1/accounts", new { name = AccountFixtures.Name });
    }

    [When(@"I create an account with accountType {string}")]
    public async Task WhenICreateAnAccountWithAccountType(string accountType)
    {
        await PostAsync("/v1/accounts", new { name = AccountFixtures.Name, accountType });
    }

    [When("I fetch the created account")]
    public async Task WhenIFetchTheCreatedAccount()
    {
        await GetAsync($"/v1/accounts/{_scenarioContext.Get<string>("CreatedAccountNumber")}");
    }

    [When("I fetch the other user's account")]
    public async Task WhenIFetchTheOtherUsersAccount()
    {
        await GetAsync($"/v1/accounts/{_scenarioContext.Get<string>("OtherAccountNumber")}");
    }

    [When("I fetch an unknown account")]
    public async Task WhenIFetchAnUnknownAccount()
    {
        await GetAsync("/v1/accounts/01999999");
    }

    [When("I fetch an account with a bad accountNumber")]
    public async Task WhenIFetchAnAccountWithABadAccountNumber()
    {
        await GetAsync("/v1/accounts/not-an-account");
    }

    [When(@"I fetch an account by number {string}")]
    public async Task WhenIFetchAnAccountByNumber(string accountNumber)
    {
        await GetAsync($"/v1/accounts/{accountNumber}");
    }

    [Then("the response is a BankAccountResponse")]
    public void ThenTheResponseIsABankAccountResponse()
    {
        using var document = JsonDocument.Parse(Body);
        var root = document.RootElement;
        var accountNumber = root.GetProperty("accountNumber").GetString();
        Assert.Matches(AccountNumberPattern, accountNumber);
        Assert.Equal(BankAccount.FixedSortCode, root.GetProperty("sortCode").GetString());
        Assert.Equal(AccountFixtures.Name, root.GetProperty("name").GetString());
        Assert.Equal(BankAccount.PersonalAccountType, root.GetProperty("accountType").GetString());
        Assert.Equal(0.00m, root.GetProperty("balance").GetDecimal());
        Assert.Equal(Money.GbpCurrency, root.GetProperty("currency").GetString());
        Assert.True(root.TryGetProperty("createdTimestamp", out _));
        Assert.True(root.TryGetProperty("updatedTimestamp", out _));
        _scenarioContext.Set(accountNumber!, "CreatedAccountNumber");
    }

    private async Task<string> CreateAccountAsync()
    {
        var response = await Client.PostAsync("/v1/accounts", UserFixtures.JsonBody(AccountFixtures.ValidCreateBody()));
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("accountNumber").GetString()!;
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

    private void StoreResponse(HttpResponseMessage response, string body)
    {
        _scenarioContext.Set(response);
        _scenarioContext.Set(body, "ResponseBody");
    }

    private HttpClient Client => _scenarioContext.Get<HttpClient>();

    private string Body => _scenarioContext.Get<string>("ResponseBody");
}
