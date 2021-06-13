using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    public static class TestScenarioExtensions
    {
        public static async Task<T> Step<T>(this ITestScenario scenario, string description, Func<Task<T>> action, [CallerMemberName] string scenarioName = null)
        {
            T result = default(T);
            await scenario.Step(description, async () => { result = await action(); }, scenarioName);
            return result;
        }
        
        public static async Task<T> Step<T>(this ITestScenario scenario, string description, Func<T> action, [CallerMemberName] string scenarioName = null)
        {
            T result = default(T);
            await scenario.Step(description,  () => { result =  action(); }, scenarioName);
            return result;
        }
    }
}