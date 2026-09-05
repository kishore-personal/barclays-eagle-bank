using System.Net.Http.Headers;
using System.Text;
using EagleBank.Tests.Support;
using Reqnroll;

namespace EagleBank.Tests.StepDefinitions;

[Binding]
public sealed class HttpSteps
{
    private readonly ScenarioContext _scenarioContext;

    public HttpSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("I am not authenticated")]
    public void GivenIAmNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    [Given("I am authenticated as a registered user")]
    public void GivenIAmAuthenticatedAsARegisteredUser()
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "pending-task-004");
    }

    [Given("another registered user exists")]
    public void GivenAnotherRegisteredUserExists()
    {
    }

    [Given("I have a personal bank account")]
    public void GivenIHaveAPersonalBankAccount()
    {
    }

    [Given("that account has a balance of {decimal}")]
    public void GivenThatAccountHasABalanceOf(decimal balance)
    {
        _scenarioContext.Set(balance, "ExpectedBalance");
    }

    [When("I send a {word} request to {string}")]
    public async Task WhenISendARequestTo(string method, string path)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), path);
        var response = await Client.SendAsync(request);
        _scenarioContext.Set(response);
        _scenarioContext.Set(await response.Content.ReadAsStringAsync(), "ResponseBody");
    }

    [Then("I receive a {int} response")]
    public void ThenIReceiveAResponse(int statusCode)
    {
        Assert.Equal(statusCode, (int)_scenarioContext.Get<HttpResponseMessage>().StatusCode);
    }

    [Then("the response is a UserResponse \\/ BankAccountResponse \\/ TransactionResponse \\/ token payload")]
    public void ThenTheResponseIsASuccessPayload()
    {
        Assert.False(string.IsNullOrWhiteSpace(_scenarioContext.Get<string>("ResponseBody")));
    }

    [Then("the response is an ErrorResponse \\/ BadRequestErrorResponse")]
    public void ThenTheResponseIsAnErrorPayload()
    {
        var body = _scenarioContext.Get<string>("ResponseBody");
        Assert.Contains("message", body, StringComparison.OrdinalIgnoreCase);
    }

    [Then("the account balance is {decimal}")]
    public void ThenTheAccountBalanceIs(decimal balance)
    {
        _scenarioContext.Set(balance, "AssertedBalance");
    }

    [Then("the test log sink contains no denylist PII")]
    public void ThenTheTestLogSinkContainsNoDenylistPii()
    {
        var sink = _scenarioContext.Get<TestLogSink>();
        var combined = string.Join(Environment.NewLine, sink.Entries);
        foreach (var banned in LogDenylist.Values)
        {
            Assert.DoesNotContain(banned, combined, StringComparison.OrdinalIgnoreCase);
        }
    }

    private HttpClient Client => _scenarioContext.Get<HttpClient>();
}
