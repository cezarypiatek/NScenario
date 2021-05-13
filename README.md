# NScenario
Dead simple, test framework independent, without any magic, a library for annotating steps of test case scenarios.

## How to use it

Just create an instance of `NScenario.TestScenario` class and start annotating your test steps by wrapping it in `Step` method call.
Example test can look as follows:

```cs
[Test]
public async Task should_activate_community_supporter_license_when_eligible()
{
    using var driver = await LicenseServerTestDriver.Create();
    var scenario = new TestScenario();

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
        var activationResult = await driver.ActivateLicense(activationData.email, activationData.licenseKey);
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
STEP 4: Verify that license activated properly
```

Benefits:
- Obvious way to enforce step descriptions
- More readable test scenario
- Sub-scopes for repeatable steps
- Readable output that facilitates broken scenario investigation


## Why not XBehave.net

[xBehave.net](https://github.com/adamralph/xbehave.net) is the `XUnit` extension so it can be used only with XUnit based tests. In my opinion, it is also quite cryptic (string extension methods might not obvious) and a little bit over-complicated. **BUT THIS IS MY PERSONAL OPINION**
