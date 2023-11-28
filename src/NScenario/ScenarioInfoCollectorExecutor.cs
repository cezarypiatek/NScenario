using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NScenario
{
    abstract record MaybeAsyncAction;

    record AsyncAction(Func<Task> ToInvoke) : MaybeAsyncAction;

    record NonAsyncAction(Action ToInvoke) : MaybeAsyncAction;

    class ScenarioInfoCollectorExecutor : IScenarioStepExecutor
    {
        private readonly IScenarioStepExecutor _scenarioStepExecutorImplementation;
        private readonly ScenarioStepInfo _data = new ScenarioStepInfo();
        private ScenarioStepInfo? _current;

        public ScenarioInfoCollectorExecutor(IScenarioStepExecutor scenarioStepExecutorImplementation)
        {
            _scenarioStepExecutorImplementation = scenarioStepExecutorImplementation;
        }

        public List<ScenarioStepInfo> Steps => _data.SubSteps ?? new List<ScenarioStepInfo>();

        public async Task Step(string scenarioName, string stepDescription, Func<Task> action, StepContext stepContext)
        {
            await Step(scenarioName, stepDescription, new AsyncAction(action), stepContext);
        }

        public async Task Step(string scenarioName, string stepDescription, Action action, StepContext stepContext)
        {
            await Step(scenarioName, stepDescription, new NonAsyncAction(action), stepContext);
        }

        private async Task Step(string scenarioName, string stepDescription, MaybeAsyncAction action, StepContext stepContext)
        {
            _current ??= _data;
            _current.SubSteps ??= new List<ScenarioStepInfo>();

            var newStep = new ScenarioStepInfo
            {
                Description = stepDescription,
                LineNumber = stepContext.StepLineNumber,
                FilePath = stepContext.StepFilePath,
                Status = StepExecutionStatus.Started
            };

            _current.SubSteps.Add(newStep);

            var toRestore = _current;
            _current = newStep;

            var timer = Stopwatch.StartNew();
            try
            {
                switch (action)
                {
                    case AsyncAction asyncAction:
                        await _scenarioStepExecutorImplementation.Step(scenarioName, stepDescription,
                            asyncAction.ToInvoke, stepContext);
                        break;
                    case NonAsyncAction nonAsyncAction:
                        await _scenarioStepExecutorImplementation.Step(scenarioName, stepDescription,
                            nonAsyncAction.ToInvoke, stepContext);
                        break;
                }

                _current.Status = StepExecutionStatus.Success;
            }
            catch (Exception e)
            {
                _current.Status = StepExecutionStatus.Failed;
                if (e.Data.Contains("NScenarioHandled") == false)
                {
                    _current.Exception = e.ToString();
                    e.Data["NScenarioHandled"] = true;
                }
                throw;
            }
            finally
            {
                timer.Stop();
                _current.ExecutionTime = timer.Elapsed;
                _current = toRestore;    
            }
        }
    }
}