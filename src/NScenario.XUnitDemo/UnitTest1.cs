using System;
using System.Threading.Tasks;
using Xunit;

namespace NScenario.XUnitDemo
{
    public class UnitTest1
    {
        private Action _AssertAction = () =>
        {
            Assert.True(false);
        };

        [Fact]
        public async Task Assert_in_step_action_not_throwing_exception()
        {
            var scenario = TestScenarioFactory.Default();
            await scenario.Step("I am assert which should fail", _AssertAction);
        }

        [Fact]
        public async Task Assert_in_step_action_throws_an_exception_when_async_used()
        {
            var scenario = TestScenarioFactory.Default();
            await scenario.Step("I am assert which should fail", async () => { 
                // CS1998 Warning here but it fails
                _AssertAction(); 
            });
        }

        [Fact]
        public async Task Plain_action_fails_in_async_test_method()
        {
            _AssertAction();
        }

        [Fact]
        public void Plain_action_fails_in_regular_test_method()
        {
            _AssertAction();
        }
    }
}
