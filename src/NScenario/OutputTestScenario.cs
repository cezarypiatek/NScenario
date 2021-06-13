using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    public class OutputTestScenario : ITestScenario
    {
        private readonly TextWriter _outputWriter;

        private readonly ConcurrentDictionary<string, bool> _startedScenarios = new ConcurrentDictionary<string, bool>();

        public OutputTestScenario(TextWriter outputWriter = null)
        {
            _outputWriter = outputWriter ?? Console.Out;
        }

        public async Task Step(string description, Func<Task> action, [CallerMemberName] string scenarioName = null)
        {
            TryWriteScenarioTitle(scenarioName);
            WriteStepDescription(description);
            await action();
        }

        public Task Step(string description, Action action, [CallerMemberName] string scenarioName = null)
        {
            
            TryWriteScenarioTitle(scenarioName);
            WriteStepDescription(description);
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