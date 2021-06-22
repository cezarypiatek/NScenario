using System;
using System.Threading.Tasks;

namespace NScenario.StepExecutors
{
    class PrefixedScenarioStepExecutor : IScenarioStepExecutor
    {
        private readonly string _scenarioPrefix;
        private readonly string _stepPrefix;
        private readonly IScenarioStepExecutor _scenarioStepExecutorImplementation;

        public PrefixedScenarioStepExecutor(IScenarioStepExecutor scenarioStepExecutorImplementation, string scenarioPrefix = null, string stepPrefix = null)
        {
            _scenarioStepExecutorImplementation = scenarioStepExecutorImplementation;
            _scenarioPrefix = scenarioPrefix ?? "SCENARIO";
            _stepPrefix = stepPrefix ?? "STEP";
        }

        public Task Step(string scenarioName, string stepDescription, Func<Task> action, StepContext stepContext)
        {
            return _scenarioStepExecutorImplementation.Step($"{_scenarioPrefix}: {scenarioName}", $"{_stepPrefix} {stepDescription}", action, stepContext);
        }

        public Task Step(string scenarioName, string stepDescription, Action action, StepContext stepContext)
        {
            return _scenarioStepExecutorImplementation.Step($"{_scenarioPrefix}: {scenarioName}", $"{_stepPrefix} {stepDescription}", action, stepContext);
        }
    }
}