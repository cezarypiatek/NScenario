using System;
using System.Threading.Tasks;

namespace NScenario
{
    class PrefixedTestScenario : ITestScenario
    {
        private readonly string _scenarioPrefix;
        private readonly string _stepPrefix;
        private readonly ITestScenario _testScenarioImplementation;

        public PrefixedTestScenario(ITestScenario testScenarioImplementation, string scenarioPrefix = "SCENARIO", string stepPrefix = "STEP")
        {
            _testScenarioImplementation = testScenarioImplementation;
            _scenarioPrefix = scenarioPrefix;
            _stepPrefix = stepPrefix;
        }

        public Task Step(string description, Func<Task> action, string scenarioName = null)
        {
            return _testScenarioImplementation.Step($"{_stepPrefix} {description}", action, $"{_scenarioPrefix} {scenarioName}");
        }

        public Task Step(string description, Action action, string scenarioName = null)
        {
            return _testScenarioImplementation.Step($"{_stepPrefix} {description}", action, $"{_scenarioPrefix} {scenarioName}");
        }
    }
}