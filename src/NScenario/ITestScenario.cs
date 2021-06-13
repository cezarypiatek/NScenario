using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NScenario
{
    public interface ITestScenario
    {
        Task Step(string description, Func<Task> action, [CallerMemberName] string scenarioName = null);
        Task Step(string description, Action action, [CallerMemberName] string scenarioName = null);
    }
}