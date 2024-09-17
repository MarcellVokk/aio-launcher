using System;
using System.Diagnostics;
using System.IO;
using BfmeFoundationProject.RegistryKit;
using BfmeFoundationProject.RegistryKit.Data;
using BfmeFoundationProject.WorkshopKit.Logic;

namespace AllInOneLauncher.Core.Managers;

internal static class BfmeLaunchManager
{
    internal static async void LaunchGame(Data.BfmeGame game)
    {
        ProcessStartInfo startInfo = new()
        {
            WorkingDirectory = BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.InstallPath),
            FileName = Path.Combine(BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.InstallPath), 
                BfmeDefaults.DefaultGameExecutableNames[(int)game])
        };

        string? activeModPath = await BfmeWorkshopStateManager.GetActiveModPath((int)game);
        if (activeModPath != null)
        {
            startInfo.ArgumentList.Add("-mod");
            startInfo.ArgumentList.Add(activeModPath);
        }

        using Process? gameProcess = Process.Start(startInfo);
        if (gameProcess == null) return;

        gameProcess.WaitForExit();
    }
}