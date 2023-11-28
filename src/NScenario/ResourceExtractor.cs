using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace NScenario;

internal static class ResourceExtractor
{
    public static string GetEmbeddedResourceContent(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException("Resource not found: " + resourceName);
        }
        try
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
        finally
        {
            stream?.Dispose();
        }
    }
}