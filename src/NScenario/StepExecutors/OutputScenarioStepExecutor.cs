using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace NScenario.StepExecutors
{
    public class OutputScenarioStepExecutor : IScenarioStepExecutor
    {
        private readonly TextWriter _outputWriter;

        private readonly ConcurrentDictionary<string, bool> _startedScenarios = new ConcurrentDictionary<string, bool>();

        public OutputScenarioStepExecutor(TextWriter outputWriter = null)
        {
            _outputWriter = outputWriter ?? Console.Out;
        }

        public async Task Step(string scenarioName, string stepDescription, Func<Task> action, StepContext stepContext)
        {
            TryWriteScenarioTitle(scenarioName);
            WriteStepDescription(stepDescription);
            await action();
        }

        public Task Step(string scenarioName, string stepDescription, Action action, StepContext stepContext)
        {

            TryWriteScenarioTitle(scenarioName);
            WriteStepDescription(stepDescription);
            action();
            return Task.CompletedTask;
        }

        private void WriteStepDescription(string description)
        {
            _outputWriter.WriteLine(description);
        }

        private void TryWriteScenarioTitle(string scenario)
        {
            if (_startedScenarios.TryAdd(scenario, true))
            {
                var scenarioTitle = scenario.Replace("_", " ");
                _outputWriter.WriteLine(scenarioTitle);
                _outputWriter.WriteLine();
            }
        }
    }
}