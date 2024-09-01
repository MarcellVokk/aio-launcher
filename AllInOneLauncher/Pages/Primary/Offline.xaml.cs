using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AllInOneLauncher.Core.Managers;
using AllInOneLauncher.Data;
using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Elements.Offline;
using AllInOneLauncher.Popups;
using BfmeFoundationProject.RegistryKit;
using BfmeFoundationProject.WorkshopKit.Data;
using BfmeFoundationProject.WorkshopKit.Logic;

namespace AllInOneLauncher.Pages.Primary;

public partial class Offline : UserControl
{
    internal static readonly Offline Instance = new();
    private int previousSelectedIndex = -1;

    public Offline()
    {
        InitializeComponent();

        Properties.Settings.Default.SettingsSaving += (s, e) =>
        {
            UpdateTitleImage();
            UpdatePlayButton();
        };

        BfmeWorkshopSyncManager.OnSyncBegin += OnSyncBegin;
        BfmeWorkshopSyncManager.OnSyncEnd += OnSyncEnd;
    }

    private void OnNewsTabClicked(object sender, MouseButtonEventArgs e) => ShowNews();
    private void OnLibraryTabClicked(object sender, MouseButtonEventArgs e) => ShowLibrary();
    private void OnWorkshopTabClicked(object sender, MouseButtonEventArgs e) => ShowWorkshop();

    private void OnSyncBegin(BfmeWorkshopEntry entry)
    {
        Dispatcher.Invoke(() =>
        {
            if (entry.Game == gameTabs.SelectedIndex && (entry.Type == 0 || entry.Type == 1 || entry.Type == 4))
            {
                activeEntry.WorkshopEntry = entry;
                activeEntry.IsLoading = true;
            }

            Disabled = true;
        });
    }

    private void OnSyncEnd()
    {
        Dispatcher.Invoke(() =>
        {
            activeEntry.IsLoading = false;
            Disabled = false;
            UpdateEnabledEnhancements();
        });
    }

    public bool Disabled
    {
        get => gameTabs.IsHitTestVisible == false;
        set
        {
            gameTabs.IsHitTestVisible = !value;
            innerTabs.IsHitTestVisible = !value;
            library.IsHitTestVisible = !value;
            enabledEnhancements.IsHitTestVisible = !value;
        }
    }

    public void ShowNews()
    {
        foreach (Border tab in Instance!.innerTabs.Children.OfType<Border>())
        {
            if (tab == newsTab)
                tab.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1EFFFFFF"));
            else
                tab.Background = Brushes.Transparent;
        }

        news.Visibility = Visibility.Visible;
        library.Visibility = Visibility.Hidden;
        workshop.Visibility = Visibility.Hidden;

        news.Load((BfmeGame)gameTabs.SelectedIndex);
    }

    public void ShowLibrary()
    {
        foreach (Border tab in Instance!.innerTabs.Children.OfType<Border>())
        {
            if (tab == libraryTab)
                tab.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1EFFFFFF"));
            else
                tab.Background = Brushes.Transparent;
        }

        news.Visibility = Visibility.Hidden;
        library.Visibility = Visibility.Visible;
        workshop.Visibility = Visibility.Hidden;

        library.Load(gameTabs.SelectedIndex);
    }

    public void ShowWorkshop()
    {
        foreach (Border tab in Instance!.innerTabs.Children.OfType<Border>())
        {
            if (tab == workshopTab)
                tab.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1EFFFFFF"));
            else
                tab.Background = Brushes.Transparent;
        }

        news.Visibility = Visibility.Hidden;
        library.Visibility = Visibility.Hidden;
        workshop.Visibility = Visibility.Visible;

        workshop.Load(gameTabs.SelectedIndex);
    }

    private void OnLaunchGameClicked(object sender, EventArgs e)
    {
        LauncherStateManager.Visible = false;
        BfmeLaunchManager.LaunchGame((BfmeGame)gameTabs.SelectedIndex, windowMode.Selected);
        LauncherStateManager.Visible = true;
    }

    private void OnInstallGameClicked(object sender, EventArgs e)
    {
        PopupVisualizer.ShowPopup(new InstallGameDialog(),
            OnPopupSubmited: (submittedData) => InstallGame((BfmeGame)gameTabs.SelectedIndex, submittedData[0], submittedData[1]));
    }

    public async void InstallGame(BfmeGame game, string selectedLanguage, string selectedLocation)
    {
        try
        {
            // BfmeRegistryManager.CreateNewInstallRegistry(game, Path.Combine(selectedLocation, game < 2 ? $"BFME{game + 1}" : "RotWK"), selectedLanguage);
            // if (game == 2 && !BfmeRegistryManager.IsInstalled(BfmeGame.BFME2)) BfmeRegistryManager.CreateNewInstallRegistry(BfmeGame.BFME2, Path.Combine(selectedLocation, "BFME2"), selectedLanguage);
            // await BfmeWorkshopSyncManager.Sync(await BfmeWorkshopEntry.OfficialPatch(game));

            BfmeRegistryManager.CreateNewInstallRegistry(game, Path.Combine(selectedLocation, game == BfmeGame.ROTWK ? "RotWK" : $"BFME{(int)game + 1}"), selectedLanguage);

            if (game == BfmeGame.ROTWK && !BfmeRegistryManager.IsInstalled(BfmeGame.BFME2))
                BfmeRegistryManager.CreateNewInstallRegistry(BfmeGame.BFME2, Path.Combine(selectedLocation, "BFME2"), selectedLanguage);

            await BfmeWorkshopSyncManager.Sync(await BfmeWorkshopEntry.OfficialPatch((int)game));

            UpdatePlayButton();
        }
        catch (Exception ex)
        {
            PopupVisualizer.ShowPopup(new ErrorPopup(ex));
        }
    }

    private async void TabChanged(object sender, EventArgs e)
    {
        if (gameTabs.SelectedIndex != previousSelectedIndex)
        {
            previousSelectedIndex = gameTabs.SelectedIndex;
            activeEntry.WorkshopEntry = await BfmeWorkshopStateManager.GetActivePatch(previousSelectedIndex);

            UpdateTitleImage();
            UpdatePlayButton();
            UpdateEnabledEnhancements();

            if (news.Visibility == Visibility.Visible)
                ShowNews();
            else if (library.Visibility == Visibility.Visible)
                ShowLibrary();
            else if (workshop.Visibility == Visibility.Visible)
                ShowWorkshop();
        }
    }

    private void UpdateTitleImage()
    {
        string game;
        if (gameTabs.SelectedIndex == 0)
            game = "BFME1";
        else if (gameTabs.SelectedIndex == 1)
            game = "BFME2";
        else if (gameTabs.SelectedIndex == 2)
            game = "Rotwk";
        else
            return;

        string language = "en";
        if (LauncherStateManager.Language == 1)
            language = "de";

        titleImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/Images/{language}_{game}_title.png"));
    }

    private void UpdatePlayButton()
    {
        if (BfmeRegistryManager.IsInstalled(gameTabs.SelectedIndex))
            launchButton.ButtonState = LaunchButtonState.Launch;
        else
            launchButton.ButtonState = LaunchButtonState.Install;
    }

    private async void UpdateEnabledEnhancements()
    {
        enabledEnhancements.Children.Clear();
        foreach (BfmeWorkshopEntry entry in (await BfmeWorkshopStateManager.GetActiveEnhancements(gameTabs.SelectedIndex)).Values)
            enabledEnhancements.Children.Add(new EnabledEnhancementTile() { WorkshopEntry = entry, Margin = new Thickness(0, 0, 0, 10) });
        activeEnhancementsNullIndicator.Visibility = enabledEnhancements.Children.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }
}