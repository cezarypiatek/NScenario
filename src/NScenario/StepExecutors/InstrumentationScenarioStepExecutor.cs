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
        try
        {
            await _scenarioStepExecutorImplementation.Step(scenarioName, stepDescription, action, stepContext);
        }
        catch (Exception e)
        {
            TryToMarkActivityAsFailed(activity, e);
            throw;
        }
    }

    private static void TryToMarkActivityAsFailed(Activity? activity, Exception e)
    {
        if (activity != null)
        {
            activity.AddEvent(new ActivityEvent("exception",tags: new ActivityTagsCollection
            {
                ["operation.status"] = "failed",
                ["exception.escaped"] = true,
                ["exception.type"] = e.GetType().ToString(),
                ["exception.message"] = e.Message,
                ["exception.stacktrace"] = e.StackTrace
            }));
            activity.SetStatus(ActivityStatusCode.Error, e.Message);
        }
    }

    public async Task Step(string scenarioName, string stepDescription, Action action, StepContext stepContext)
    {
        using var activity = NScenarioInstrumentation.DiagnosticSource.StartActivity(name: stepDescription);
        try
        {
            await _scenarioStepExecutorImplementation.Step(scenarioName, stepDescription, action, stepContext);
        }
        catch (Exception e)
        {
            TryToMarkActivityAsFailed(activity, e);
            throw;
        }
    }
}