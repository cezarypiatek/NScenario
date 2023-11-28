using System.Collections.Generic;

namespace NScenario
{
    public class ScenarioInfo
    {
        public string ScenarioTitle { get; set; }
        public string MethodName { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public ScenarioExecutionStatus Status { get; set; }
        public IReadOnlyList<ScenarioStepInfo> Steps { get; set; }
    }
}