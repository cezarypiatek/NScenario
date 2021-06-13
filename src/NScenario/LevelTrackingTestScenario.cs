using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    public abstract class LevelTrackingTestScenario : ITestScenario
    {
        private readonly ITestScenario _testScenarioImplementation;
        private readonly NestingCounter _counter;

        protected LevelTrackingTestScenario(ITestScenario testScenarioImplementation)
        {
            _testScenarioImplementation = testScenarioImplementation;
            _counter = new NestingCounter();
        }

        public async Task Step(string description, Func<Task> action, [CallerMemberName] string scenarioName = null)
        {
            using var level = _counter.StartLevel(scenarioName);
            var decoratedDescription = DecorateDescription(description, level);
            await  _testScenarioImplementation.Step(decoratedDescription, action, scenarioName);
        }

        public async Task Step(string description, Action action, [CallerMemberName] string scenarioName = null)
        {
            using var level = _counter.StartLevel(scenarioName);
            var decoratedDescription = DecorateDescription(description, level);
            await  _testScenarioImplementation.Step(decoratedDescription, action, scenarioName);
        }

        protected abstract string DecorateDescription(string description, Level level);
    }
}