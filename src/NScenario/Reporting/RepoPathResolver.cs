using System.IO;
using System.Linq;
using System.Reflection;

namespace NScenario;

internal static class RepoPathResolver
{
    public static string? FindRepoRootDirectory(string? fileInTheRepo)
    {
        if (fileInTheRepo == null)
            return null;
        
        var currentDirectory = new DirectoryInfo(Path.GetDirectoryName(fileInTheRepo) ?? string.Empty);

        while (currentDirectory is { Exists: true })
        {
            if (Directory.Exists(Path.Combine(currentDirectory.FullName, ".git")))
            {
                return currentDirectory.FullName;
            }
            currentDirectory = currentDirectory.Parent;
        }

        return null;
    }

    public static SourceControlInfo? GetSourceControlInfo(Assembly assembly)
    {
        var repositoryUrl = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == "RepositoryUrl")?.Value;
        var revision = assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().FirstOrDefault()?.InformationalVersion?.Split('+').LastOrDefault();

        if (string.IsNullOrWhiteSpace(repositoryUrl) == false && string.IsNullOrWhiteSpace(revision) == false)
        {
            return new SourceControlInfo(repositoryUrl, revision);
        }

        return null;
    }
}