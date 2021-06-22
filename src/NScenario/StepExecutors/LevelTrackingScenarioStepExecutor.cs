using System;
using System.Threading.Tasks;

namespace NScenario.StepExecutors
{
    public abstract class LevelTrackingScenarioStepExecutor : IScenarioStepExecutor
    {
        private readonly IScenarioStepExecutor _scenarioStepExecutorImplementation;
        private readonly NestingCounter _counter;

        protected LevelTrackingScenarioStepExecutor(IScenarioStepExecutor scenarioStepExecutorImplementation)
        {
            _scenarioStepExecutorImplementation = scenarioStepExecutorImplementation;
            _counter = new NestingCounter();
        }

        public async Task Step(string scenarioName, string stepDescription, Func<Task> action, StepContext stepContext)
        {
            using var level = _counter.StartLevel(stepContext.ScenarioName);
            var decoratedDescription = DecorateDescription(stepDescription, level);
            await  _scenarioStepExecutorImplementation.Step(scenarioName, decoratedDescription, action, stepContext);
        }

        public async Task Step(string scenarioName, string stepDescription, Action action, StepContext stepContext)
        {
            using var level = _counter.StartLevel(stepContext.ScenarioName);
            var decoratedDescription = DecorateDescription(stepDescription, level);
            await  _scenarioStepExecutorImplementation.Step(scenarioName, decoratedDescription, action, stepContext);
        }

        protected abstract string DecorateDescription(string description, Level level);
    }
}