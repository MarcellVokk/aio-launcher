using System;
using System.Collections.Specialized;
using System.IO;
using AllInOneLauncher.Properties;

namespace AllInOneLauncher.Core.Managers;

public static class LibraryLocationManager
{
    private static readonly string DefaultLibraryLocation = Path.Combine(
        Path.GetPathRoot(Environment.ProcessPath) ?? "C:/", "BfmeLibrary");

    public static void EnsureLibraryLocation()
    {
        if (Settings.Default.LibraryLocations.Contains("NotSet"))
        {
            SetDefaultLibraryLocation();
        }
        else if (!Settings.Default.LibraryLocations.Contains(DefaultLibraryLocation))
        {
            AddDefaultLibraryLocation();
        }
    }

    private static void SetDefaultLibraryLocation()
    {
        Settings.Default.LibraryLocations = new StringCollection { DefaultLibraryLocation };
        SaveSettings();
    }

    private static void AddDefaultLibraryLocation()
    {
        Settings.Default.LibraryLocations.Add(DefaultLibraryLocation);
        SaveSettings();
    }

    private static void SaveSettings()
    {
        Settings.Default.Save();
    }
}