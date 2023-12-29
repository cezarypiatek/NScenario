# NScenario
[![NuGet](https://img.shields.io/nuget/vpre/NScenario.svg)](https://www.nuget.org/packages/NScenario/)

Dead simple, test framework independent, without any magic, a library for annotating steps of test case scenarios.
Please read [Readable and clear tests for ASP.NET Core services](https://cezarypiatek.github.io/post/component-tests-scenarios/) article for better exaplanation and example use cases.

This library was discussed during [ASP.NET Community Standup 2021-08-24](https://www.youtube.com/watch?v=Zw11P2z_ptc&t=831s)

## How to install
`NScenario` is distribute as a nuget package [NScenario](https://www.nuget.org/packages/NScenario/)


## How to use it

Just create an instance of `NScenario.TestScenario` class and start annotating your test steps by wrapping it in `Step` method call.
You can create `TestScenario` instance manually by providing a desired composition of `IScenarioStepExecutor` instances or simply by calling `TestScenarioFactory.Default()` method.

Example test can look as follows:

```cs
[Test]
public async Task should_activate_community_supporter_license_when_eligible()
{
    using var driver = await LicenseServerTestDriver.Create();
    var scenario = TestScenarioFactory.Default();

    var activationData = new
    {
        email = "abs@test.com",
        licenseKey = "WTKP4-66NL5-HMKQW-GFSCZ"
    };

    await scenario.Step("Import supporters", async () =>
    {
        await driver.ImportSupporters("BuyMeCoffee", new[] { "john@done.com", "jane@done.com", activationData.email });
    });

    await scenario.Step("Register purchase for supporter email", async () =>
    {
        await driver.RegisterPurchaseWithCoupon("Extension for VS2017", activationData.email, activationData.licenseKey, "OssSupporter");
    });

    await scenario.Step("Activate the license with supporter email", async () =>
    {
        var activationResult = await scenario.Step("Call active license endpoint" () => 
        {
            return await driver.ActivateLicense(activationData.email, activationData.licenseKey);
        });
        
        await scenario.Step("Verify that license activated properly", () =>
        {
            Assert.AreEqual(true, activationResult.Activated);
            Assert.AreEqual("Unlimited", activationResult.Capabilities["VsVersion"]);
        });
    });
}
```

Console output

```plaintext
SCENARIO: should activate community supporter license when eligible

STEP 1: Import supporters
STEP 2: Register purchase for supporter email
STEP 3: Activate the license with supporter email
    STEP 3.1: Call active license endpoint
    STEP 3.2: Verify that license activated properly
```

Benefits:
- Obvious way to enforce step descriptions
- More readable test scenario
- Sub-scopes for repeatable steps
- Readable output that facilitates broken scenario investigation

## Console output
Some test runners are hijacking console output and provide a custom stream for logging. By default `NScenario` is writing scenario description to the console, but this can be overridden by providing a custom `TextWriter` stream to `TestScenarioFactory.Default()` method. 

### Example setup for `NUnit`

```cs
public class MyTests
{
    [Test]
    public void sample_test_case()
    {
        var scenario = TestScenarioFactory.Default(TestContext.Progress);    
    }
}
```

### Example setup for `XUnit`

```cs

public class XUnitOutputAdapter : TextWriter
{
    private readonly ITestOutputHelper _output;
    public XUnitOutputAdapter(ITestOutputHelper output) => _output = output;
    public override void WriteLine(string? value) => _output.WriteLine(value);
    public override Encoding Encoding { get; }
}

public class MyTests
{
    private readonly ITestOutputHelper _output;
    public MyTests(ITestOutputHelper output) => this._output = output;
    
    [Fact]
    public void sample_test_case()
    {
        var scenario = TestScenarioFactory.Default(new XUnitOutputAdapter(_output));
    }
}
```

More info about [capturing console output in XUnit](https://xunit.net/docs/capturing-output)

## Global setup for scenario output
Test scenario output can be configured globally by setting `TestScenarioFactory.DefaultScenarioOutputWriter`.

### Example using [Module initializer](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/module-initializers) :

```cs
using NScenario.OutputWriters;
using NScenario.StepExecutors;

public static class GlobalSetup
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Setup()
    {
        TestScenarioFactory.DefaultScenarioOutputWriter = new StreamScenarioOutputWriter(TestContext.Progress);
    }
}
```

### Example using [SetUpFixture](https://docs.nunit.org/articles/nunit/writing-tests/attributes/setupfixture.html) for `NUnit`

You should put that code under the default namespace:

```cs
using NScenario.OutputWriters;
using NScenario.StepExecutors;
using NUnit.Framework;

[SetUpFixture]
public class AllTestsSetup
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        TestScenarioFactory.DefaultScenarioOutputWriter = new StreamScenarioOutputWriter(TestContext.Progress);
    }
}
```

## Exporting scenario transcription

You save the test scenario transcription as Markdown (with option to export as HTML) using `MarkdownFormatterOutputWriter`.

Sample setup with exporting scenario transcription to a file:

```cs
using NScenario.OutputWriters;
using NScenario.StepExecutors;
using NUnit.Framework;

[SetUpFixture]
public class AllTestsSetup
{
    private readonly MarkdownFormatterOutputWriter _reportWriter = new (title: "Sample tests with NScenario", currentTestIdentifierAccessor: ()=> TestContext.CurrentContext.Test.ID);

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        TestScenarioFactory.DefaultScenarioOutputWriter = new ComposeScenarioOutputWriter(new IScenarioOutputWriter[]
        {
            //INFO: Configure live reporting to console with NUnit
            new StreamScenarioOutputWriter(TestContext.Progress),
            //INFO: Configure collecting transcription as markdown
            _reportWriter
        });

    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        // INFO: Save the raw Markdown to a file
        _reportWriter.Save("Report.md");
        //INFO: Export the markdown to HTML file
        _reportWriter.ExportToHtml("Report.html");
    }
}
```


There's also an option to generate a nice html report for all test scenarios. Just invoke `TestScenarioFactory.GetAllExecutedScenarios().SaveAsReport("AllReports.html");` in your global teardown to get a report like the one below:

![image](https://github.com/cezarypiatek/NScenario/assets/7759991/13c501d6-d26b-4406-93fb-1da29dca9bff)


This report browser supports links to scenario steps definition. To make it work, you need to set the following msbuild properties (or environment variables):
- RepositoryUrl
- SourceRevisionId

## Test scenario title

Test scenario title is generated by removing underscores and splitting camel/pascalcase string from the test method name (`[CallerMemberName]` is used to retrieve that name). This allows for immediate review of the test name (I saw many, extremely long and totally ridiculous test method names. A good test method name should reveal the intention of the test case, not its details). You can always override the default title by setting it explicitly during test scenario creation (especially useful for parametrized test methods):

```cs
[TestCase(false)]
[TestCase(true)]
public async Task should_present_basic_scenario_with_explicit_title(bool someFlag)
{
    var scenario = TestScenarioFactory.Default(title: $"some scenario when flag set to '{someFlag}'");

    await scenario.Step("This is the first step", () =>
    {
        // Here comes the logic
    });

    await scenario.Step("This is the second step", () =>
    {
        // Here comes the logic
    });

    await scenario.Step("This is the third step", () =>
    {
        // Here comes the logic
    });
}
```

`NScenario` is prefixing scenario title with `SCENARIO:` prefix and every step is prefixed with `STEP`. If you are writing step descriptions in other languages than English, you can override those prefixes by specifing them explicitly why calling `TestScenarioFactory.Default()` method.

```cs
var scenario = TestScenarioFactory.Default(scenarioPrefix: "SCENARIUSZ", stepPrefix: "KROK");
```



## Why not XBehave.net

[xBehave.net](https://github.com/adamralph/xbehave.net) is the `XUnit` extension so it can be used only with XUnit based tests. In my opinion, it is also quite cryptic (string extension methods called with single letter might not obvious) and a little bit over-complicated. **BUT THIS IS MY PERSONAL OPINION**
