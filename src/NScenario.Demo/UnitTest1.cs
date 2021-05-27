using System.Threading.Tasks;
using NUnit.Framework;

namespace NScenario.Demo
{
    public class Tests
    {

        [Test]
        public async Task should_present_basic_scenario()
        {
            var scenario = new TestScenario();

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
        public async Task should_present_scenario_with_sub_steps()
        {
            var scenario = new TestScenario();

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
            
            await scenario.Step("This is the third step", () =>
            {
                // Here comes the logic
            });
        }
    }
}