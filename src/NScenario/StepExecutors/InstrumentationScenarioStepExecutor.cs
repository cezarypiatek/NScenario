using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NScenario.StepExecutors;

public static class NScenarioInstrumentation
{
    public static ActivitySource DiagnosticSource = new ActivitySource("NScenario");
}

public class InstrumentationScenarioStepExecutor : IScenarioStepExecutor
{
    private readonly IScenarioStepExecutor _scenarioStepExecutorImplementation;


    public InstrumentationScenarioStepExecutor(IScenarioStepExecutor scenarioStepExecutorImplementation)
    {
        _scenarioStepExecutorImplementation = scenarioStepExecutorImplementation;
    }

    public async Task Step(string scenarioName, string stepDescription, Func<Task> action, StepContext stepContext)
    {
        using var activity = NScenarioInstrumentation.DiagnosticSource.StartActivity(name: stepDescription);
        await _scenarioStepExecutorImplementation.Step(scenarioName, stepDescription, action, stepContext);
    }

    public async Task Step(string scenarioName, string stepDescription, Action action, StepContext stepContext)
    {
        using var activity = NScenarioInstrumentation.DiagnosticSource.StartActivity(name: stepDescription);
        await _scenarioStepExecutorImplementation.Step(scenarioName, stepDescription, action, stepContext);
    }
}