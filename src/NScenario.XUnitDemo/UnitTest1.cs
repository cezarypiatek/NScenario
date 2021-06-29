using System;
using System.Threading.Tasks;
using Xunit;

namespace NScenario.XUnitDemo
{
    public class UnitTest1
    {
        [Fact]
        public async Task Assert_which_fails_as_expected()
        {
            var scenario = TestScenarioFactory.Default();
            // Works perfect
            await scenario.Step("I am assert which fails", async () =>
            {
                Assert.True(false);
            });
        }

        [Fact]
        public async Task Assert_Which_is_not_throwing_exception()
        {
            var scenario = TestScenarioFactory.Default();
            // Actual Result:
            // Test succeed
            // Expected result: Test failure
            await scenario.Step("I am assert which fails", () =>
            {
                Assert.True(false);
            });
        }
    }
}
