using System;
using System.Diagnostics;
using System.IO;
using BfmeFoundationProject.BfmeKit;
using BfmeFoundationProject.BfmeKit.Data;
using BfmeFoundationProject.WorkshopKit.Logic;

namespace AllInOneLauncher.Core.Managers;

internal static class BfmeLaunchManager
{
    internal static async void LaunchGame(Data.BfmeGame game)
    {
        LauncherStateManager.Visible = false;

        ProcessStartInfo startInfo = new()
        {
            WorkingDirectory = BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.InstallPath),
            FileName = Path.Combine(BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.InstallPath), 
                BfmeDefaults.DefaultGameExecutableNames[(int)game])
        };

        string? activeModPath = await BfmeWorkshopManager.GetActiveModPath((int)game);
        if (activeModPath != null)
        {
            startInfo.ArgumentList.Add("-mod");
            startInfo.ArgumentList.Add(activeModPath);
        }

        BfmeRegistryManager.EnsureCompatibilitySettings(
            Path.Combine(BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.InstallPath), BfmeDefaults.DefaultGameExecutableNames[(int)game]));
        BfmeRegistryManager.EnsureCompatibilitySettings(
            Path.Combine(BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.InstallPath), "game.dat"));

        using Process? gameProcess = Process.Start(startInfo);
        if (gameProcess == null) return;

        gameProcess.WaitForExit();
        LauncherStateManager.Visible = true;
    }
}