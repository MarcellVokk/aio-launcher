using System;
using System.IO;
using System.Linq;
using System.Windows;
using AllInOneLauncher.Data;
using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Popups;
using BfmeFoundationProject.RegistryKit;
using BfmeFoundationProject.RegistryKit.Data;

namespace AllInOneLauncher.Core.Services;

public static class GameInstallationChecker
{
    public static void CheckForInvalidInstallation()
    {
        foreach (BfmeGame game in Enum.GetValues(typeof(BfmeGame)).Cast<BfmeGame>().Where(g => g != BfmeGame.NONE))
        {
            if (IsGameInstalledInInvalidLocation(game))
            {
                ShowInvalidInstallationPopup();
                break;
            }
        }
    }

    private static bool IsGameInstalledInInvalidLocation(BfmeGame game)
    {
        return BfmeRegistryManager.IsInstalled(game) && 
               BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.InstallPath)
                   .Contains(Path.GetDirectoryName(Environment.ProcessPath)!);
    }

    private static void ShowInvalidInstallationPopup()
    {
        PopupVisualizer.ShowPopup(
            new MessagePopup("INVALID INSTALL LOCATION",
                "The All In One Launcher has been installed inside one of the game's folders. This is not allowed, please reinstall the launcher in a different location!"),
            OnPopupClosed: () =>
            {
                Application.Current.Shutdown();
            });
    }
}