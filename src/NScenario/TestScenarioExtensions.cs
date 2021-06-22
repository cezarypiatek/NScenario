using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    public static class TestScenarioExtensions
    {
        public static async Task<T> Step<T>(this ITestScenario scenarioStepExecutor, string description, Func<Task<T>> action, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            T result = default(T);
            await scenarioStepExecutor.Step(description, async () => { result = await action(); }, filePath, methodName, lineNumber);
            return result;
        }

        public static async Task<T> Step<T>(this ITestScenario scenarioStepExecutor, string description, Func<T> action,  [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
            T result = default(T);
            await scenarioStepExecutor.Step(description, () => { result = action(); }, filePath, methodName, lineNumber);
            return result;
        }
    }
}