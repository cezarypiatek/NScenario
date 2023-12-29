using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace NScenario;

public static class ReportExtensions
{
    public static void SaveAsReport(this IReadOnlyList<ScenarioInfo> scenarios, string outputPath)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        var sourceControlInfo = RepoPathResolver.GetSourceControlInfo(callingAssembly);
        var repositoryRootDir = RepoPathResolver.FindRepoRootDirectory(scenarios.FirstOrDefault()?.FilePath);
        var template = ResourceExtractor.GetEmbeddedResourceContent("NScenario.report-browser-template.html");
        var scenariosPayload = JsonSerializer.Serialize(new
        {
            SourceControlInfo = new
            {
                RepositoryUrl = sourceControlInfo?.RepositoryUrl,
                Revision = sourceControlInfo?.Revision,
                RepositoryRootDir = repositoryRootDir
            },
            Scenarios = scenarios
        });
        var report = template.Replace("//[DATA_PLACEHOLDER]", scenariosPayload);
        File.WriteAllText(outputPath, report);
    }
}