using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AllInOneLauncher.Core.Utils;
using AllInOneLauncher.Elements;
using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Popups;
using BfmeFoundationProject.HttpInstruments;
using Windows.Storage;

namespace AllInOneLauncher.Core.Managers;

public static class LauncherUpdateManager
{
    public static string LauncherAppDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BFME All In One Launcher");

    public static async void CheckForUpdates()
    {
        #if DEBUG
        return;
        #endif

        try
        {
            using HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };

            if (!Directory.Exists(LauncherAppDirectory))
                Directory.CreateDirectory(LauncherAppDirectory);

            string curentVersionHash = "";
            string latestVersionHash = "";

            try
            {
                curentVersionHash = File.Exists(Path.Combine(LauncherAppDirectory, "AllInOneLauncher.exe")) ? await Task.Run(() => FileUtils.GetFileMd5Hash(Path.Combine(LauncherAppDirectory, "AllInOneLauncher.exe"))) : "";
                latestVersionHash = await client.GetStringAsync($"https://bfmeladder.com/api/applications/versionHash?name=all-in-one-launcher&version=main");
            }
            catch
            {
                return;
            }

            if (curentVersionHash == latestVersionHash)
            {
                if (File.Exists(Path.Combine(LauncherAppDirectory, "AllInOneLauncher_new.exe")))
                    File.Delete(Path.Combine(LauncherAppDirectory, "AllInOneLauncher_new.exe"));

                return;
            }
            else if (File.Exists(Path.Combine(LauncherAppDirectory, "AllInOneLauncher_new.exe")))
            {
                File.Move(Path.Combine(LauncherAppDirectory, "AllInOneLauncher_new.exe"), Path.Combine(LauncherAppDirectory, "AllInOneLauncher.exe"), true);
                RestartLauncher(afterUpdate: false);
                return;
            }

            LauncherUpdatePopup updatePopup = new();
            PopupVisualizer.ShowPopup(updatePopup);

            await HttpsInstruments.Download($"https://arena-files.bfmeladder.com/application-builds/all-in-one-launcher-main", Path.Combine(LauncherAppDirectory, "AllInOneLauncher_new.exe"), (progress) => updatePopup.LoadProgress = progress);
            RestartLauncher(afterUpdate: true);
        }
        catch (Exception ex)
        {
            PopupVisualizer.ShowPopup(new ErrorPopup(ex));
        }
    }

    private static void RestartLauncher(bool afterUpdate)
    {
        App.Mutex?.Dispose();
        App.Mutex = null;

        ProcessStartInfo process = new()
        {
            UseShellExecute = true,
            WorkingDirectory = LauncherUpdateManager.LauncherAppDirectory,
            FileName = afterUpdate
                ? Path.Combine(LauncherUpdateManager.LauncherAppDirectory, "AllInOneLauncher_new.exe")
                : Path.Combine(LauncherUpdateManager.LauncherAppDirectory, "AllInOneLauncher.exe"),
            Verb = "runas"
        };
        Process.Start(process);

        Environment.Exit(0);
    }
}