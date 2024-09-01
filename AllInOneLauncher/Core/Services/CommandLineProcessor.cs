using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Popups;

namespace AllInOneLauncher.Core.Services;

public static class CommandLineProcessor
{
    public static void ProcessArgs()
    {
        if (App.Args.Length > 0)
        {
            if (App.Args[0] == "--LauncherChangelog")
                PopupVisualizer.ShowPopup(new LauncherChangelogPopup());
        }
    }
}