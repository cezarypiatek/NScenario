using System;
using System.Threading.Tasks;

namespace NScenario
{
    public interface IScenarioStepExecutor
    {
        Task Step(string scenarioName, string stepDescription, Func<Task> action, StepContext stepContext);
        Task Step(string scenarioName, string stepDescription, Action action, StepContext stepContext);
    }

   public class StepContext
   {
        public string ScenarioName { get; set; }
        public string StepMethodName { get; set; }
        public string StepFilePath { get; set; }
        public int StepLineNumber { get; set; }
    }
}