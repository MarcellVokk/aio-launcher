using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using AllInOneLauncher.Core.Builder;
using AllInOneLauncher.Core.Managers;
using AllInOneLauncher.Core.Services;
using AllInOneLauncher.Data;
using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Pages.Primary;
using AllInOneLauncher.Popups;
using BfmeFoundationProject.RegistryKit;
using BfmeFoundationProject.RegistryKit.Data;
using BfmeFoundationProject.WorkshopKit.Data;
using BfmeFoundationProject.WorkshopKit.Logic;
using Settings = AllInOneLauncher.Properties.Settings;

namespace AllInOneLauncher;

public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        
        InitializeServices();
        SetupUI();
        SetupEventHandlers();
    }
    
    private void InitializeServices()
    {
        LauncherStateManager.Init();
        LauncherUpdateManager.CheckForUpdates();

        LibraryLocationManager.EnsureLibraryLocation();
        GameInstallationChecker.CheckForInvalidInstallation();
    }

    private void SetupUI()
    {
        TrayIcon.Visibility = Visibility.Collapsed;
        fullContent.Visibility = Visibility.Visible;

        Width = SystemParameters.WorkArea.Width * 0.72;
        Height = SystemParameters.WorkArea.Height * 0.85;

        CheckSize();
        ReloadContextMenu();
        ShowOffline();
    }

    private void SetupEventHandlers()
    {
        Application.Current.Exit += OnApplicationExit;
        Loaded += (sender, e) => CommandLineProcessor.ProcessArgs();

        BfmeWorkshopSyncManager.OnSyncBegin += OnSyncBegin;
        BfmeWorkshopSyncManager.OnSyncEnd += OnSyncEnd;
    }

    private static void ProcessCommandLineArgs()
    {
        if (App.Args.Length > 0)
        {
            if (App.Args[0] == "--LauncherChangelog")
                PopupVisualizer.ShowPopup(new LauncherChangelogPopup());
        }
    }

    private void OnSyncBegin(BfmeWorkshopEntry entry)
    {
        Dispatcher.Invoke(() =>
        {
            settingsIcon.IsHitTestVisible = false;
            settingsIcon.Opacity = 0.4;
        });
    }

    private void OnSyncEnd()
    {
        Dispatcher.Invoke(() =>
        {
            settingsIcon.IsHitTestVisible = true;
            settingsIcon.Opacity = 1;
        });
    }

    public static void SetContent(FrameworkElement? newContent) => Instance!.content.Child = newContent;

    public static async void SetFullContent(FrameworkElement? newContent)
    {
        Instance!.content.Visibility = newContent != null ? Visibility.Collapsed : Visibility.Visible;
        Instance.fullContent.Child = newContent;

        Instance.tabs.Visibility = newContent != null ? Visibility.Collapsed : Visibility.Visible;
        Instance.icons.Visibility = newContent != null ? Visibility.Collapsed : Visibility.Visible;

        Instance.background.Effect = newContent is Pages.Primary.Settings ? new BlurEffect() { Radius = 20 } : null;

        ContentSynchronizer.SyncIfNeeded(Pages.Primary.Settings.NeedsResync);

        Pages.Primary.Settings.NeedsResync = false;
    }


    public static void ShowOffline()
    {
        SetContent(Offline.Instance);
        TabManager.HighlightTab(Instance!.offlineTab, Instance.tabs.Children);
    }

    public static void ShowOnline()
    {
        SetContent(Online.Instance);
        TabManager.HighlightTab(Instance!.onlineTab, Instance.tabs.Children);
    }

    public static void ShowGuides()
    {
        SetContent(Guides.Instance);
        TabManager.HighlightTab(Instance!.guidesTab, Instance.tabs.Children);
    }

    public static void ShowPatreons()
    {
        SetContent(Patreons.Instance);
        TabManager.HighlightTab(Instance!.patreonsTab, Instance.tabs.Children);
    }

    private void OnOfflineTabClicked(object sender, MouseButtonEventArgs e) => ShowOffline();
    private void OnOnlineTabClicked(object sender, MouseButtonEventArgs e) => ShowOnline();
    private void OnGuidesTabClicked(object sender, MouseButtonEventArgs e) => ShowGuides();
    private void OnPatreonsTabClicked(object sender, MouseButtonEventArgs e) => ShowPatreons();

    private void OnSettingsButtonClicked(object sender, MouseButtonEventArgs e) => SetFullContent(new Pages.Primary.Settings("LauncherGeneral"));
    private void OnLinkButtonClicked(object sender, MouseButtonEventArgs e) => Process.Start(new ProcessStartInfo("explorer", ((FrameworkElement)sender).Tag.ToString() ?? ""));

    private void OnLoad(object sender, RoutedEventArgs e) => CheckSize();
    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => CheckSize();

    public void CheckSize() => windowGrid.LayoutTransform = new ScaleTransform(Math.Min(10, Math.Min((ActualWidth / 1700), (ActualHeight / 1200))), Math.Min(10, Math.Min((ActualWidth / 1700), (ActualHeight / 1200))));

    private void OnTrayIconDoubleClicked(object sender, RoutedEventArgs e) => LauncherStateManager.Visible = true;

    private void LauncherMainWindow_Closing(object sender, CancelEventArgs e)
    {
        if (Settings.Default.HideToTrayOnClose)
        {
            e.Cancel = true;
            ReloadContextMenu();
            TrayIcon.Visibility = Visibility.Visible;
            LauncherStateManager.Visible = false;
        }
        else
        {
            Application.Current.Shutdown();
        }
    }

    private void ReloadContextMenu()
    {
        TrayIcon.ContextMenu = TrayIconContextMenuBuilder.BuildContextMenu();
    }

    private void OnApplicationExit(object sender, ExitEventArgs e)
    {
        TempFileCleaner.CleanUp();
    }
}