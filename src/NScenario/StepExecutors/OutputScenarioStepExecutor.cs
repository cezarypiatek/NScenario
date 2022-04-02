using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NScenario.OutputWriters;

namespace NScenario.StepExecutors
{
    public class OutputScenarioStepExecutor : IScenarioStepExecutor
    {
        private readonly IScenarioOutputWriter _scenarioOutputWriter;

        private readonly ConcurrentDictionary<string, bool> _startedScenarios = new ConcurrentDictionary<string, bool>();

        public OutputScenarioStepExecutor(TextWriter outputWriter = null)
        {
            _scenarioOutputWriter = new StreamScenarioOutputWriter(outputWriter ?? Console.Out);
        }

        public OutputScenarioStepExecutor(IScenarioOutputWriter scenarioOutputWriter)
        {
            _scenarioOutputWriter = scenarioOutputWriter;
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
            _scenarioOutputWriter.WriteStepDescription(description);
        }

        private void TryWriteScenarioTitle(string scenario)
        {
            if (_startedScenarios.TryAdd(scenario, true))
            {
                
                _scenarioOutputWriter.WriteScenarioTitle(scenario);
            }
        }

       
    }
}