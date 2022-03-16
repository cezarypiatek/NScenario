using System;
using System.Threading.Tasks;
using NScenario.OutputWriters;
using NScenario.StepExecutors;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NScenario.XunitTests
{
    [Collection("NScenarioCollection")]
    public class ExceptionHandlingUnitTests
    {
        private Action _AssertAction = () =>
        {
            Assert.True(false);
        };

        private readonly IScenarioOutputWriter scenarioOutputWriter;

        public ExceptionHandlingUnitTests(ITestOutputHelper output, NScenarioReporting mdSetup)
        {
            scenarioOutputWriter = new ComposeScenarioOutputWriter(new IScenarioOutputWriter[]
            {
                mdSetup.ReportWriter,
                new StreamScenarioOutputWriter(new XUnitOutputAdapter(output))
            });
        }


        [Fact]
        public async Task Xunit_Assert_in_regular_step_action()
        {
            var scenario = TestScenarioFactory.Default(scenarioOutputWriter);
            await Assert.ThrowsAsync<TrueException>(async () =>
                await scenario.Step("I am assert which should fail", _AssertAction));
        }

        [Fact]
        public async Task Xunit_Assert_in_async_step_action()
        {
            var scenario = TestScenarioFactory.Default(scenarioOutputWriter);
            await Assert.ThrowsAsync<TrueException>(async () =>
                await scenario.Step("I am assert which should fail", async () =>
                {
                    await Task.Delay(1);
                    _AssertAction();
                }));
        }
    }
}
