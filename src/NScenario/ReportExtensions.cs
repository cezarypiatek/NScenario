using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NScenario;

public static class ReportExtensions
{
    public static void SaveAsReport(this IReadOnlyList<ScenarioInfo> scenarios, string outputPath)
    {
        var template = ResourceExtractor.GetEmbeddedResourceContent("NScenario.report-browser-template.html");
        var report = template.Replace("//[DATA_PLACEHOLDER]", JsonSerializer.Serialize(scenarios));
        File.WriteAllText(outputPath, report);
    }
}