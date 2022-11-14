using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MarkdownSharp;
using NScenario.StepExecutors;

namespace NScenario.OutputWriters
{
    public class MarkdownFormatterOutputWriter: IScenarioOutputWriter
    {
        private readonly Func<string> _currentTestIdentifierAccessor;
        private readonly Dictionary<string, StringBuilder> _outputBuilders = new Dictionary<string, StringBuilder>();

        private StringBuilder OutputBuilder
        {
            get
            {
                var testId = _currentTestIdentifierAccessor();
                if (_outputBuilders.TryGetValue(testId, out var builder))
                {
                    return builder;
                }

                return _outputBuilders[testId] = new StringBuilder();
            }
        }
        
        /// <summary>
        ///     Export test scenario transcript to markdown report
        /// </summary>
        /// <param name="title">Report title</param>
        /// <param name="currentTestIdentifierAccessor">Function that returns unique identifier representing currently executed scenario. Should be provided when tests are executed in parallel mode</param>
        public MarkdownFormatterOutputWriter(string? title = null, Func<string>? currentTestIdentifierAccessor = null)
        {
            _currentTestIdentifierAccessor = currentTestIdentifierAccessor ?? (() => "SHARED_ID");
            if (title != null)
            {
                OutputBuilder.AppendLine($"# {title}");
            }
        }

        public void WriteStepDescription(string description)
        {
            var prefixPosition = description.IndexOf("STEP", StringComparison.InvariantCultureIgnoreCase);
            if (prefixPosition >= 0)
            {
                var newDescription = description.Insert(prefixPosition, "* ");
                OutputBuilder.AppendLine(newDescription);
            }
            else
            {
                OutputBuilder.AppendLine("* " + description);
            }
        }

        public void WriteScenarioTitle(string scenarioTitle)
        {
            OutputBuilder.AppendLine($"\r\n## {scenarioTitle}\r\n");
        }

        public string GetMarkdown()
        {
            var mergedMarkdown = new StringBuilder();
            foreach (var builder in _outputBuilders.Values)
            {
                mergedMarkdown.Append(builder.ToString());
            }
            return mergedMarkdown.ToString();
        }

        public void Save(string path) =>  File.WriteAllText(path, GetMarkdown(), Encoding.UTF8);


        public void ExportToHtml(string path)
        {
            var markdownContent = GetMarkdown();
            var mm = new Markdown();
            var htmlContent = mm.Transform(markdownContent);
            File.WriteAllText(path, $"<html><head></head><body>{htmlContent}<body></html>", Encoding.UTF8);
        }
    }
}