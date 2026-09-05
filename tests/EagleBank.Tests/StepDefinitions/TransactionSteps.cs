using System.Text.Json;
using System.Text.RegularExpressions;
using EagleBank.Domain;
using EagleBank.Tests.Support;
using Reqnroll;

namespace EagleBank.Tests.StepDefinitions;

[Binding]
public sealed class TransactionSteps
{
    private static readonly Regex TransactionIdPattern = new(@"^tan-[A-Za-z0-9]+$", RegexOptions.Compiled);

    private readonly ScenarioContext _scenarioContext;

    public TransactionSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("that account has a balance of {decimal}")]
    public async Task GivenThatAccountHasABalanceOf(decimal balance)
    {
        var response = await Client.PostAsync(
            $"/v1/accounts/{CreatedAccountNumber}/transactions",
            UserFixtures.JsonBody(TransactionFixtures.DepositBody(balance)));
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        _scenarioContext.Set(document.RootElement.GetProperty("id").GetString()!, "CreatedTransactionId");
    }

    [Given("I have deposited {decimal} into that account")]
    public async Task GivenIHaveDepositedIntoThatAccount(decimal amount)
    {
        var response = await Client.PostAsync(
            $"/v1/accounts/{CreatedAccountNumber}/transactions",
            UserFixtures.JsonBody(TransactionFixtures.DepositBody(amount)));
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        _scenarioContext.Set(document.RootElement.GetProperty("id").GetString()!, "CreatedTransactionId");
    }

    [When("I deposit {decimal} into that account")]
    public async Task WhenIDepositIntoThatAccount(decimal amount)
    {
        await PostAsync($"/v1/accounts/{CreatedAccountNumber}/transactions", TransactionFixtures.DepositBody(amount));
    }

    [When(@"I deposit {decimal} into account {string}")]
    public async Task WhenIDepositIntoAccount(decimal amount, string accountNumber)
    {
        await PostAsync($"/v1/accounts/{accountNumber}/transactions", TransactionFixtures.DepositBody(amount));
    }

    [When("I withdraw {decimal} from that account")]
    public async Task WhenIWithdrawFromThatAccount(decimal amount)
    {
        _scenarioContext.Set("withdrawal", "ExpectedTransactionType");
        await PostAsync(
            $"/v1/accounts/{CreatedAccountNumber}/transactions",
            TransactionFixtures.WithdrawalBody(amount));
    }

    [When("I deposit {decimal} into the other user's account")]
    public async Task WhenIDepositIntoTheOtherUsersAccount(decimal amount)
    {
        await PostAsync(
            $"/v1/accounts/{_scenarioContext.Get<string>("OtherAccountNumber")}/transactions",
            TransactionFixtures.DepositBody(amount));
    }

    [When("I fetch the created transaction")]
    public async Task WhenIFetchTheCreatedTransaction()
    {
        await GetAsync($"/v1/accounts/{CreatedAccountNumber}/transactions/{CreatedTransactionId}");
    }

    [When("I fetch the created transaction on the second account")]
    public async Task WhenIFetchTheCreatedTransactionOnTheSecondAccount()
    {
        await GetAsync(
            $"/v1/accounts/{_scenarioContext.Get<string>("SecondAccountNumber")}/transactions/{CreatedTransactionId}");
    }

    [When(@"I fetch transaction {string} on account {string}")]
    public async Task WhenIFetchTransactionOnAccount(string transactionId, string accountNumber)
    {
        await GetAsync($"/v1/accounts/{accountNumber}/transactions/{transactionId}");
    }

    [When(@"I fetch transaction {string} on the created account")]
    public async Task WhenIFetchTransactionOnTheCreatedAccount(string transactionId)
    {
        await GetAsync($"/v1/accounts/{CreatedAccountNumber}/transactions/{transactionId}");
    }

    [When(@"I fetch transaction {string} on the other user's account")]
    public async Task WhenIFetchTransactionOnTheOtherUsersAccount(string transactionId)
    {
        await GetAsync(
            $"/v1/accounts/{_scenarioContext.Get<string>("OtherAccountNumber")}/transactions/{transactionId}");
    }

    [Then("the response is a TransactionResponse")]
    public void ThenTheResponseIsATransactionResponse()
    {
        using var document = JsonDocument.Parse(Body);
        var root = document.RootElement;
        var id = root.GetProperty("id").GetString();
        Assert.Matches(TransactionIdPattern, id);
        Assert.Equal(Money.GbpCurrency, root.GetProperty("currency").GetString());
        var type = root.GetProperty("type").GetString();
        var expectedType = _scenarioContext.TryGetValue<string>("ExpectedTransactionType", out var stored)
            ? stored
            : "deposit";
        Assert.Equal(expectedType, type);
        Assert.True(root.TryGetProperty("createdTimestamp", out _));
        Assert.StartsWith("usr-", root.GetProperty("userId").GetString());
        _scenarioContext.Set(id!, "CreatedTransactionId");
    }

    private async Task PostAsync(string path, object body)
    {
        var response = await Client.PostAsync(path, UserFixtures.JsonBody(body));
        _scenarioContext.Set(response);
        _scenarioContext.Set(await response.Content.ReadAsStringAsync(), "ResponseBody");
    }

    private async Task GetAsync(string path)
    {
        var response = await Client.GetAsync(path);
        _scenarioContext.Set(response);
        _scenarioContext.Set(await response.Content.ReadAsStringAsync(), "ResponseBody");
    }

    private HttpClient Client => _scenarioContext.Get<HttpClient>();

    private string Body => _scenarioContext.Get<string>("ResponseBody");

    private string CreatedAccountNumber => _scenarioContext.Get<string>("CreatedAccountNumber");

    private string CreatedTransactionId => _scenarioContext.Get<string>("CreatedTransactionId");
}
