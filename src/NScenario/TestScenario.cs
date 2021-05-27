using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ConcurrentDictionary<string, Stack<(bool, int)>> _stepNumber = new ConcurrentDictionary<string, Stack<(bool, int)>>();

        public TestScenario(TextWriter outputWriter = null, string scenarioPrefix = null, string stepPrefix = null)
        {
            _scenarioPrefix = scenarioPrefix ?? ScenarioPrefix;
            _stepPrefix = stepPrefix ?? StepPrefix;
            _outputWriter = outputWriter ?? Console.Out;
        }

        public async Task Step(string description, Func<Task> action, [CallerMemberName] string scenarioName = null)
        {
            var height = StartStep(scenarioName);
            var currentStepNumber = IncrementCurrentStepNumber(scenarioName);
            TryWriteScenarioTitle(scenarioName, currentStepNumber);
            WriteStepDescription(description, currentStepNumber);
            await action();
            EndStep(scenarioName, height);
        }

        public Task Step(string description, Action action, [CallerMemberName] string scenarioName = null)
        {
            var height = StartStep(scenarioName);
            var currentStepNumber = IncrementCurrentStepNumber(scenarioName);
            TryWriteScenarioTitle(scenarioName, currentStepNumber);
            WriteStepDescription(description, currentStepNumber);
            action();
            EndStep(scenarioName, height);
            return Task.CompletedTask;
        }

        private int StartStep(string scenario)
        {
           _ = _stepNumber.AddOrUpdate(scenario, s =>
            {
                var stack = new Stack<(bool,int)>();
                stack.Push((true,0));
                return stack;
            }, (s, stack) =>
           {
               var (used, counter) = stack.Pop();
               stack.Push((true, counter));
               if (used)
               { 
                   stack.Push((true, 0));
               }
              
               return stack;
            });
           return _stepNumber[scenario].Count;
        }

        private void EndStep(string scenario, int height)
        {
            if (_stepNumber.TryGetValue(scenario, out var stepStack))
            {
                if (height < stepStack.Count)
                {
                    stepStack.Pop();
                }

                var (_, counter) = stepStack.Pop();
                stepStack.Push((false, counter));
            }
        }

        private Stack<(bool,int)> IncrementCurrentStepNumber(string scenario)
        {
            var stepStack = _stepNumber[scenario];
            var (used,counter) = stepStack.Pop();
            counter++;
            stepStack.Push((used, counter));
            return stepStack;
        }

        private void WriteStepDescription(string description, Stack<(bool, int)> currentStepNumber)
        {
            var stepNumberDescription = string.Join(".", currentStepNumber.ToArray().Reverse().Select(x=> x.Item2));
            var indentation = new string('\t', currentStepNumber.Count - 1);
            _outputWriter.WriteLine($"{indentation}{_stepPrefix} {stepNumberDescription}: {description}");
        }

        private void TryWriteScenarioTitle(string scenario, Stack<(bool, int)> currentStepNumber)
        {
            if (currentStepNumber.Count == 1 && currentStepNumber.Peek() is (_, 1))
            {
                var scenarioTitle = scenario.Replace("_", " ");
                _outputWriter.WriteLine($"{_scenarioPrefix}: {scenarioTitle}");
                _outputWriter.WriteLine();
            }
        }
    }
}