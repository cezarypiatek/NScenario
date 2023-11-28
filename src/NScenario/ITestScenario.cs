using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    public interface ITestScenario
    {
        Task Step(string description, Func<Task> action, [CallerFilePath] string filePath = "",  [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0);
        Task Step(string description, Action action, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0);
        ScenarioInfo GetScenarioInfo();
    }
}