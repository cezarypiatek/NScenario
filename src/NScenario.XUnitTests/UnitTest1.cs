using System;
using System.Threading.Tasks;
using NScenario.OutputWriters;
using NScenario.StepExecutors;
using NScenario.XunitTests;
using Xunit;
using Xunit.Abstractions;

namespace NScenario.Demo
{
    [Collection("NScenarioCollection")]
    public class Tests 
    {
        private readonly IScenarioOutputWriter scenarioOutputWriter;
        public Tests(ITestOutputHelper output, NScenarioReporting mdSetup)
        {
           scenarioOutputWriter = new ComposeScenarioOutputWriter(new IScenarioOutputWriter[]
           {
             mdSetup.ReportWriter,
             new StreamScenarioOutputWriter(new XUnitOutputAdapter(output))
           });
        }


        [Fact]
        public async Task should_present_basic_scenario()
        {
            var scenario = TestScenarioFactory.Default(scenarioOutputWriter);

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

        [Fact]
        public async Task should_present_basic_scenario_with_steps_returning_value()
        {
            var scenario = TestScenarioFactory.Default(scenarioOutputWriter);

            var valFromStep1 = await scenario.Step("This is the first step", () =>
            {
                // Here comes the logic
                return 1;
            });

            var valFromStep2 = await scenario.Step("This is the second step", async () =>
            {
                // Here comes the logic
                await Task.Yield();
                return valFromStep1 + 1;
            });

            await scenario.Step("This is the third step", () =>
            {
                // Here comes the logic
                _ = valFromStep1 + valFromStep2;
            });
        }

        [Fact]
        public async Task should_present_scenario_with_sub_steps()
        {
            var scenario = TestScenarioFactory.Default(scenarioOutputWriter);

            await scenario.Step("This is the first step", async () =>
            {
                await scenario.Step("This is the first sub-step of first step", () =>
                {
                    // Here comes the logic
                });
                await scenario.Step("This is the second sub-step of first step", async () =>
                {
                    await scenario.Step("Yet another nesting level p1", () =>
                    {
                        // Here comes the logic
                    });
                    await scenario.Step("Yet another nesting level p2", () =>
                    {
                        // Here comes the logic
                    });
                });
            });
            
            await PerformReusableScenarioPart(scenario);
            
            await scenario.Step("This is the third step", () =>
            {
                // Here comes the logic
            });
        }

        private static async Task PerformReusableScenarioPart(ITestScenario scenario)
        {
            await scenario.Step("This is the second step", async () =>
            {
                await scenario.Step("This is the first sub-step of second step", () =>
                {
                    // Here comes the logic
                });
                await scenario.Step("This is the second sub-step of second step", () =>
                {
                    // Here comes the logic
                });
            });
        }
    }
}