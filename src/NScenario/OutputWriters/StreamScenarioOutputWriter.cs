using System.IO;
using NScenario.StepExecutors;

namespace NScenario.OutputWriters
{
    public class StreamScenarioOutputWriter : IScenarioOutputWriter
    {
        private readonly TextWriter _outputWriter;

        public StreamScenarioOutputWriter(TextWriter outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public void WriteStepDescription(string description)
        {
            _outputWriter.WriteLine(description);
        }

        public void WriteScenarioTitle(string scenarioTitle)
        {
            _outputWriter.WriteLine(scenarioTitle);
        }
    }
}