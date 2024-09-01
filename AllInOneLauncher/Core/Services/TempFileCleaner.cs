using System;
using System.IO;
using System.Reflection;

namespace AllInOneLauncher.Core.Services;

internal static class TempFileCleaner
{
    public static void CleanUp()
    {
        string appTempPath = GetApplicationTempPath();

        if (Directory.Exists(appTempPath))
        {
            try
            {
                Directory.Delete(appTempPath, true);
            }
            catch (Exception ex)
            {
                LogError($"Error deleting the folder: {ex.Message}");
            }
        }
    }

    private static string GetApplicationTempPath()
    {
        string appName = Assembly.GetExecutingAssembly().GetName().Name!;
        return Path.Combine(Path.GetTempPath(), appName);
    }

    private static void LogError(string message)
    {
        Console.WriteLine(message);
    }
}