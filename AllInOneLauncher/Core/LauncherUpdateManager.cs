﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AllInOneLauncher.Core.Utils;
using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Popups;
using BfmeFoundationProject.HttpInstruments;

namespace AllInOneLauncher.Core;

public static class LauncherUpdateManager
{
    private const string LATEST_VERSION_SOURCE_URL = "https://bfmeladder.com/api/applications/versionHash?name=all-in-one-launcher&version=main";
    private const string LATEST_BUILD_SOURCE_URL = "https://arena-files.bfmeladder.com/application-builds/all-in-one-launcher-main";

    public static string LauncherAppDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BFME All In One Launcher");

    public static async void CheckForUpdates()
    {
#if DEBUG
        return;
#endif

        try
        {
            if (!Directory.Exists(LauncherAppDirectory))
                Directory.CreateDirectory(LauncherAppDirectory);

            string curentVersionHash = "";
            string latestVersionHash = "";

            try
            {
                curentVersionHash = File.Exists(Path.Combine(LauncherAppDirectory, "AllInOneLauncher.exe")) ? await Task.Run(() => FileUtils.GetFileMd5Hash(Path.Combine(LauncherAppDirectory, "AllInOneLauncher.exe"))) : "";
                latestVersionHash = await HttpMarshal.GetString(url: LATEST_VERSION_SOURCE_URL, headers: []);
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
                try
                {
                    File.Move(Path.Combine(LauncherAppDirectory, "AllInOneLauncher_new.exe"), Path.Combine(LauncherAppDirectory, "AllInOneLauncher.exe"), true);
                    RestartLauncher(afterUpdate: false);
                }
                catch
                {
                    RestartLauncher(afterUpdate: true);
                }

                return;
            }

            LauncherUpdatePopup updatePopup = new();
            PopupVisualizer.ShowPopup(updatePopup);

            await HttpMarshal.GetFile(
            url: LATEST_BUILD_SOURCE_URL,
            localPath: Path.Combine(LauncherAppDirectory, "AllInOneLauncher_new.exe"),
            headers: [],
            OnProgressUpdate: (progress) => updatePopup.Dispatcher.Invoke(() => updatePopup.LoadProgress = progress));

            RestartLauncher(afterUpdate: true);
        }
        catch (Exception ex)
        {
            PopupVisualizer.HidePopup();
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
            WorkingDirectory = LauncherAppDirectory,
            FileName = afterUpdate
                ? Path.Combine(LauncherAppDirectory, "AllInOneLauncher_new.exe")
                : Path.Combine(LauncherAppDirectory, "AllInOneLauncher.exe"),
            Verb = "runas"
        };
        Process.Start(process);

        Environment.Exit(0);
    }
}