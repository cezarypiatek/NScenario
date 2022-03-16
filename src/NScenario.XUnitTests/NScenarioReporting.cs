using System;
using NScenario.OutputWriters;
using Xunit;

namespace NScenario.XunitTests
{
    [CollectionDefinition("NScenarioCollection")]
    public class NScenarioCollection : ICollectionFixture<NScenarioReporting>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class NScenarioReporting : IDisposable
    {
        public MarkdownFormatterOutputWriter ReportWriter { get; } = new MarkdownFormatterOutputWriter(title: "Sample tests with NScenario");

        public NScenarioReporting()
        {
        }

        public void Dispose()
        {
            // clean-up code
            ReportWriter.Save("Report.md");
            ReportWriter.ExportToHtml("Report.html");
        }
    }
}