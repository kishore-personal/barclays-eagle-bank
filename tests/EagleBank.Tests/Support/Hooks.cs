using Reqnroll;

namespace EagleBank.Tests.Support;

[Binding]
public sealed class Hooks
{
    private readonly ScenarioContext _scenarioContext;

    public Hooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var factory = new ApiWebApplicationFactory();
        _scenarioContext.Set(factory);
        _scenarioContext.Set(factory.CreateClient());
        _scenarioContext.Set(factory.LogSink);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        if (_scenarioContext.TryGetValue<ApiWebApplicationFactory>(out var factory))
        {
            factory.Dispose();
        }
    }
}
