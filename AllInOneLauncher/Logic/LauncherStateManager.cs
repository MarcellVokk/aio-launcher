using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace AllInOneLauncher.Logic
{
    internal static class LauncherStateManager
    {
        internal static Dictionary<string, Type>? TypeMap { get; private set; }

        internal static void Init()
        {
            if (!File.Exists(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath))
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Reload();
                Properties.Settings.Default.Save();
            }

            TypeMap = Assembly.GetExecutingAssembly().GetTypes().DistinctBy(x => x.Name).ToDictionary(x => x.Name, x => x);

            if (Properties.Settings.Default.LauncherLanguage == -1)
                Language = 0;
            else
                Language = Properties.Settings.Default.LauncherLanguage;
        }

        internal static bool Visible
        {
            get => MainWindow.Instance!.ShowInTaskbar;
            set
            {
                if (value && MainWindow.Instance!.WindowState == WindowState.Minimized)
                {
                    try
                    {
                        MainWindow.Instance!.WindowState = WindowState.Normal;
                    }
                    catch { }
                    MainWindow.Instance!.ShowInTaskbar = true;
                    MainWindow.Instance!.Activate();
                }
                else if (!value && MainWindow.Instance!.WindowState == WindowState.Normal || MainWindow.Instance!.WindowState == WindowState.Maximized)
                {
                    MainWindow.Instance!.WindowState = WindowState.Minimized;
                    MainWindow.Instance!.ShowInTaskbar = false;
                }

                MainWindow.Instance!.TrayIcon.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private static int _language;
        internal static int Language
        {
            get => _language;
            set
            {
                _language = value;

                var languageDictionary = new Dictionary<int, string>
                {
                    { 0, "LanguageResources.en.xaml" },
                    { 1, "LanguageResources.de.xaml" },
                    { 2, "LanguageResources.hu.xaml" },
                    { 3, "LanguageResources.fr.xaml" },
                    { 4, "LanguageResources.it.xaml" },
                    { 5, "LanguageResources.es.xaml" },
                    { 6, "LanguageResources.sv.xaml" },
                    { 7, "LanguageResources.nl.xaml" },
                    { 8, "LanguageResources.pl.xaml" },
                    { 9, "LanguageResources.no.xaml" },
                    { 10, "LanguageResources.ru.xaml" },
                    { 11, "LanguageResources.ar.xaml" }
                };

                ResourceDictionary resourceDictionary = [];
                if (languageDictionary.TryGetValue(value, out string? resourceFileName))
                {
                    resourceDictionary.Source = new Uri($"..\\..\\..\\Resources\\Dictionary\\{resourceFileName}", UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Invalid language selection.");
                }

                Properties.Settings.Default.LauncherLanguage = value;
                Properties.Settings.Default.Save();
            }
        }

        internal static void Restart(bool update = false)
        {
            App.Mutex?.Dispose();
            App.Mutex = null;

            ProcessStartInfo elevated = new()
            {
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? "./",
                FileName = update ? Environment.ProcessPath?.Replace(".exe", "_new.exe") : Environment.ProcessPath?.Replace("_new.exe", ".exe"),
                Verb = "runas"
            };
            Process.Start(elevated);

            Environment.Exit(0);
        }
    }
}
