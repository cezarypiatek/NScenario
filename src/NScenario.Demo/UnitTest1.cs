using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NScenario.Demo
{
    public class Tests
    {

        [Test]
        public async Task should_present_basic_scenario()
        {
            var scenario = TestScenarioFactory.Default();

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
        
        [Test]
        public async Task should_present_basic_scenario_with_steps_returning_value()
        {
            var scenario = TestScenarioFactory.Default();

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
        
        [Test]
        public async Task should_present_scenario_with_sub_steps()
        {
            var scenario = TestScenarioFactory.Default();

            await scenario.Step("This is the first step", async () =>
            {
                await scenario.Step("This is the first sub-step of first step", () =>
                {
                    // Here comes the logic
                });
                await scenario.Step("This is the second sub-step of first step", () =>
                {
                    // Here comes the logic
                });
            });
            
            await PerformReusableScenarioPart(scenario);
            
            await scenario.Step("This is the third step", () =>
            {
                // Here comes the logic
            });
        }

        [Test]
        public async Task should_be_able_to_use_tasks_without_function()
        {
            var scenario = TestScenarioFactory.Default();

            await scenario.Step("This is step which waits for 100 ms", Task.Delay(100));

            var x = await scenario.Step("This is step which uses Task with result", Sum(5, 10));
            Assert.AreEqual(15, x);
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
        private static Task<int> Sum(int a, int b)
        {
            return Task.FromResult(a + b);
        }
    }
}