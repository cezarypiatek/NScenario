using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    class TestScenario : ITestScenario
    {
        private readonly IScenarioStepExecutor _stepExecutor;
        private readonly string _scenarioName;

        public TestScenario(IScenarioStepExecutor stepExecutor, string scenarioName)
        {
            _stepExecutor = stepExecutor;
            _scenarioName = scenarioName;
        }

        public async Task Step(string description, Func<Task> action, [CallerFilePath] string filePath = "",  [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var context = new StepContext
            {
                ScenarioName = _scenarioName,
                StepFilePath = filePath,
                StepLineNumber = lineNumber,
                StepMethodName = methodName
            };
            await _stepExecutor.Step(_scenarioName, description, action, context);
        }

        public Task Step(string description, Action action, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var context = new StepContext
            {
                ScenarioName = _scenarioName,
                StepFilePath = filePath,
                StepLineNumber = lineNumber,
                StepMethodName = methodName
            };
            _stepExecutor.Step(_scenarioName, description, action, context);
            return Task.CompletedTask;
        }
    }
}