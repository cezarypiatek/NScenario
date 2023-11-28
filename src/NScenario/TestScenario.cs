using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    class TestScenario : ITestScenario
    {
        private readonly IScenarioStepExecutor _stepExecutor;
        private readonly ScenarioInfoCollectorExecutor _stepInfoCollector;
        private readonly string _scenarioName;
        private readonly ScenarioContext _context;

        public TestScenario(IScenarioStepExecutor stepExecutor, string scenarioName, ScenarioContext context)
        {
            _stepExecutor =  _stepInfoCollector = new ScenarioInfoCollectorExecutor(stepExecutor);
            _scenarioName =  scenarioName;
            _context = context;
        }

        public async Task Step(string description, Func<Task> action, [CallerFilePath] string filePath = "",  [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                var context = new StepContext
                {
                    ScenarioName = _scenarioName,
                    StepName = description,
                    StepFilePath = filePath,
                    StepLineNumber = lineNumber,
                    StepMethodName = methodName
                };
                await _stepExecutor.Step(_scenarioName, description, action, context);
            }
            catch
            {
                _context.ScenarioExecutionStatus = ScenarioExecutionStatus.Failed;
                throw;
            }
        }

        public async Task Step(string description, Action action, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                var context = new StepContext
                {
                    ScenarioName = _scenarioName,
                    StepName = description,
                    StepFilePath = filePath,
                    StepLineNumber = lineNumber,
                    StepMethodName = methodName
                };
                await _stepExecutor.Step(_scenarioName, description, action, context);
            }
            catch(Exception ex)
            {
                if (ex.Data.Contains("NScenarioHandled"))
                {
                    ex.Data.Remove("NScenarioHandled");
                }

                _context.ScenarioExecutionStatus = ScenarioExecutionStatus.Failed;
                throw;
            }
        }
        
        public ScenarioInfo GetScenarioInfo() => new ()
        {
            ScenarioTitle = _scenarioName,
            MethodName = _context.MethodName,
            FilePath = _context.FilePath,
            LineNumber = _context.LineNumber,
            Status = _context.ScenarioExecutionStatus,
            Steps = _stepInfoCollector.Steps
        };
    }
}