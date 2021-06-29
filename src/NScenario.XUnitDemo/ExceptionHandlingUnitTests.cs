using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace NScenario.XUnitDemo
{
    public class ExceptionHandlingUnitTests
    {
        private Action _AssertAction = () =>
        {
            Assert.True(false);
        };

        [Fact]
        public async Task XUnit_Assert_in_regular_step_action()
        {
            var scenario = TestScenarioFactory.Default();
            await Assert.ThrowsAsync<TrueException>(async () => 
                await scenario.Step("I am assert which should fail", _AssertAction));
        }

        [Fact]
        public async Task XUnit_Assert_in_async_step_action()
        {
            var scenario = TestScenarioFactory.Default();
            await Assert.ThrowsAsync<TrueException>(async () =>
                await scenario.Step("I am assert which should fail", async () => {
                    await Task.Delay(1);
                    _AssertAction();
                }));
        }
    }
}
