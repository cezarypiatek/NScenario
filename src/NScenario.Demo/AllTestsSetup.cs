using Newtonsoft.Json;
using NScenario.OutputWriters;
using NScenario.StepExecutors;
using NUnit.Framework;

namespace NScenario.Demo
{
    [SetUpFixture]
    public class AllTestsSetup
    {
        private readonly MarkdownFormatterOutputWriter _reportWriter = new MarkdownFormatterOutputWriter(title: "Sample tests with NScenario");

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            TestScenarioFactory.DefaultScenarioOutputWriter = new ComposeScenarioOutputWriter(new IScenarioOutputWriter[]
            {
                //INFO: Configure live reporting to console with NUnit
                new StreamScenarioOutputWriter(TestContext.Progress),
                //INFO: Configure collecting transcription as markdown
                _reportWriter
            });

        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            // INFO: Save the raw Markdown to a file
            _reportWriter.Save("Report.md");
            //INFO: Export the markdown to HTML file
            _reportWriter.ExportToHtml("Report.html");
            
            // INFO: Export nice html report
            TestScenarioFactory.GetAllExecutedScenarios().SaveAsReport("AllReports.html");
        }
    }
}