using System;
using System.IO;
using System.Text;
using MarkdownSharp;
using NScenario.StepExecutors;

namespace NScenario.OutputWriters
{
    public class MarkdownFormatterOutputWriter: IScenarioOutputWriter
    {
        private readonly StringBuilder _outputBuilder = new StringBuilder();

        public MarkdownFormatterOutputWriter(string? title = null)
        {
            if (title != null)
            {
                _outputBuilder.AppendLine($"# {title}");
            }
        }

        public void WriteStepDescription(string description)
        {
            var prefixPosition = description.IndexOf("STEP", StringComparison.InvariantCultureIgnoreCase);
            if (prefixPosition >= 0)
            {
                var newDescription = description.Insert(prefixPosition, "* ");
                _outputBuilder.AppendLine(newDescription);
            }
            else
            {
                _outputBuilder.AppendLine("* " + description);
            }
        }

        public void WriteScenarioTitle(string scenarioTitle)
        {
            _outputBuilder.AppendLine($"\r\n## {scenarioTitle}\r\n");
        }

        public string GetMarkdown() => _outputBuilder.ToString();

        public void Save(string path) =>  File.WriteAllText(path, _outputBuilder.ToString(), Encoding.UTF8);


        public void ExportToHtml(string path)
        {
            var markdownContent = _outputBuilder.ToString();
            var mm = new Markdown();
            var htmlContent = mm.Transform(markdownContent);
            File.WriteAllText(path, $"<html><head></head><body>{htmlContent}<body></html>", Encoding.UTF8);
        }
    }
}