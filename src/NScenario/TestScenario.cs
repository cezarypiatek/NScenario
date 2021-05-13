using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    public class TestScenario
    {
        public static string ScenarioPrefix = "SCENARIO";
        public static string StepPrefix = "STEP";

        private readonly string _scenarioPrefix;
        private readonly string _stepPrefix;
        private readonly TextWriter _outputWriter;
        private readonly ConcurrentDictionary<string, int> _stepNumber = new ConcurrentDictionary<string, int>();

        public TestScenario(TextWriter outputWriter = null, string scenarioPrefix = null, string stepPrefix = null)
        {
            _scenarioPrefix = scenarioPrefix ?? ScenarioPrefix;
            _stepPrefix = stepPrefix ?? StepPrefix;
            _outputWriter = outputWriter ?? Console.Out;
        }

        public async Task Step(string description, Func<Task> action, [CallerMemberName] string scenarioName = null)
        {
            var currentStepNumber = IncrementCurrentStepNumber(scenarioName);
            TryWriteScenarioTitle(scenarioName, currentStepNumber);
            WriteStepDescription(description, currentStepNumber);
            await action();
        }

        private int IncrementCurrentStepNumber(string scenario)
        {
            return _stepNumber.AddOrUpdate(scenario, 1, (s, i) => ++i);
        }

        public Task Step(string description, Action action, [CallerMemberName] string scenarioName = null)
        {
            var currentStepNumber = IncrementCurrentStepNumber(scenarioName);
            TryWriteScenarioTitle(scenarioName, currentStepNumber);
            WriteStepDescription(description, currentStepNumber);
            action();
            return Task.CompletedTask;
        }

        private void WriteStepDescription(string description, int currentStepNumber)
        {
            _outputWriter.WriteLine($"{_stepPrefix} {currentStepNumber}: {description}");
        }

        private void TryWriteScenarioTitle(string scenario, int currentStepNumber)
        {
            if (currentStepNumber == 1)
            {
                var scenarioTitle = scenario.Replace("_", " ");
                _outputWriter.WriteLine($"{_scenarioPrefix}: {scenarioTitle}");
                _outputWriter.WriteLine();
            }
        }
    }
}