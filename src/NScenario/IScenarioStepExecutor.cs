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
        public string StepName { get; set; }
   }
   
   public class ScenarioContext
   {
        public string Title { get; set; }
        public string MethodName { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public ScenarioExecutionStatus ScenarioExecutionStatus { get; set; }
   }

   public enum ScenarioExecutionStatus
   {
       Success,
       Failed
   }
}