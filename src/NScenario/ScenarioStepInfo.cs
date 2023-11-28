using System;
using System.Collections.Generic;

namespace NScenario
{
    public class ScenarioStepInfo
    {
        public string Description { get; set; }
        public int LineNumber { get; set; }
        public string FilePath { get; set; }

        public TimeSpan ExecutionTime { get; set; }
        public StepExecutionStatus Status { get; set; }

        public string Exception { get; set; }
        public List<ScenarioStepInfo>? SubSteps { get; set; }
    }

    public enum StepExecutionStatus
    {
        NotStarted,
        Started,
        Failed,
        Success
    }
}