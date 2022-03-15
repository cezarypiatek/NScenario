using System.Collections.Generic;
using NScenario.StepExecutors;

namespace NScenario.OutputWriters
{
    public class ComposeScenarioOutputWriter : IScenarioOutputWriter
    {
        private readonly IReadOnlyList<IScenarioOutputWriter> _writers;

        public ComposeScenarioOutputWriter(IReadOnlyList<IScenarioOutputWriter> writers)
        {
            _writers = writers;
        }

        public void WriteStepDescription(string description)
        {
            foreach (var writer in _writers)
            {
                writer.WriteStepDescription(description);
            }
        }

        public void WriteScenarioTitle(string scenarioTitle)
        {
            foreach (var writer in _writers)
            {
                writer.WriteScenarioTitle(scenarioTitle);
            }
        }
    }
}