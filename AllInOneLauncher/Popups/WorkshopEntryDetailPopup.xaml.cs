using System;
using System.Linq;
using System.Windows;
using AllInOneLauncher.Core.Utils;
using System.Windows.Media.Imaging;
using AllInOneLauncher.Elements.Generic;
using BfmeFoundationProject.WorkshopKit.Data;
using BfmeFoundationProject.WorkshopKit.Logic;
using BfmeFoundationProject.BfmeKit;
using System.Globalization;
using AllInOneLauncher.Elements;
using Newtonsoft.Json.Linq;
using BfmeFoundationProject.HttpInstruments;
using System.Diagnostics;
using System.IO;
using AllInOneLauncher.Core.Managers;
using AllInOneLauncher.Data;

namespace AllInOneLauncher.Popups;

public partial class WorkshopEntryDetailPopup : PopupBody
{
    public override ColorStyle ColorStyle => ColorStyle.Acrylic;

    public WorkshopEntryDetailPopup()
    {
        InitializeComponent();
    }

    BfmeWorkshopEntryPreview _workshopEntry;
    public BfmeWorkshopEntryPreview WorkshopEntry
    {
        get => _workshopEntry;
        set
        {
            _workshopEntry = value;
            try { icon.Source = new BitmapImage(new Uri(value.ArtworkUrl)); } catch { }
            screenshots.Screenshots = value.ScreenshotUrls.Count == 0 ? ["", ""] : value.ScreenshotUrls;
            title.Text = value.Name;
            author.Text = value.Author;
            description.Text = value.Description;
            downloads.Text = NumberUtils.IntAsPrettyNumber(value.Metadata.Downloads);
            game.Text = value.GameName();
            IsInLibrary = BfmeWorkshopLibraryManager.IsInLibrary(value.Guid);

            icon_bfme1.Visibility = value.Game == 0 ? Visibility.Visible : Visibility.Collapsed;
            icon_bfme2.Visibility = value.Game == 1 ? Visibility.Visible : Visibility.Collapsed;
            icon_rotwk.Visibility = value.Game == 2 ? Visibility.Visible : Visibility.Collapsed;

            if (value.Type == 0)
                type.Text = Application.Current.FindResource("LibraryTilePatchType").ToString()!;
            else if (value.Type == 1)
                type.Text = Application.Current.FindResource("LibraryTileModType").ToString()!;
            else if (value.Type == 2)
                type.Text = Application.Current.FindResource("LibraryTileEnhancementType").ToString()!;
            else if (value.Type == 3)
                type.Text = Application.Current.FindResource("LibraryTileMapPackType").ToString()!;

            icon_patch.Visibility = value.Type == 0 ? Visibility.Visible : Visibility.Collapsed;
            icon_mod.Visibility = value.Type == 1 ? Visibility.Visible : Visibility.Collapsed;
            icon_enhancement.Visibility = value.Type == 2 ? Visibility.Visible : Visibility.Collapsed;
            icon_mappack.Visibility = value.Type == 3 ? Visibility.Visible : Visibility.Collapsed;

            value.Language = value.Language.Contains("EN") ? value.Language : (value.Language + " EN").TrimStart(' ');
            string curentGameLanguage = BfmeRegistryManager.GameLanguageToLanguageCode(BfmeRegistryManager.GetKeyValue(value.Game, BfmeFoundationProject.BfmeKit.Data.BfmeRegistryKey.Language));
            language_code.Text = value.Language.Contains(curentGameLanguage) ? curentGameLanguage : value.Language.Split(' ')[0];
            language_full.Text = value.Language.Split(' ').Length > 1 ? $"+ {value.Language.Split(' ').Length - 1} More" : BfmeRegistryManager.GameLanguageCodeToLanguage(language_code.Text);

            if (value.Size >= 1_073_741_824)
            {
                size_unit.Text = "GB";
                size_number.Text = Math.Round(value.Size / 1_073_741_824d, 2).ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else if (value.Size >= 1_048_576)
            {
                size_unit.Text = "MB";
                size_number.Text = Math.Round(value.Size / 1_048_576d, 2).ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else
            {
                size_unit.Text = "KB";
                size_number.Text = Math.Round(value.Size / 1024d, 2).ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            }

            changelog.Children.Add(new EntryPageChangelogItem() { Version = value.Version, CreationTime = value.CreationTime, Text = value.Changelog });
        }
    }

    private bool isLoading = false;
    public bool IsLoading
    {
        get => isLoading;
        set
        {
            isLoading = value;
            mainButtonLoadingSpinner.IsLoading = value;
            mainButtonText.Visibility = value ? Visibility.Hidden : Visibility.Visible;
        }
    }

    public bool IsInLibrary
    {
        get => mainButtonPlayText.Visibility == Visibility.Visible;
        set
        {
            mainButtonAddToLibraryText.Visibility = value ? Visibility.Hidden : Visibility.Visible;
            mainButtonPlayText.Visibility = (value && (WorkshopEntry.Type == 0 || WorkshopEntry.Type == 1)) ? Visibility.Visible : Visibility.Hidden;
            mainButtonAlreadyInLibraryText.Visibility = (value && WorkshopEntry.Type != 0 && WorkshopEntry.Type != 1) ? Visibility.Visible : Visibility.Hidden;
            mainButton.Opacity = mainButtonAlreadyInLibraryText.Visibility == Visibility.Visible ? 0.4 : 1;
            mainButton.IsHitTestVisible = mainButtonAlreadyInLibraryText.Visibility == Visibility.Visible ? false : true;
        }
    }

    private async void AddToLibrary()
    {
        try
        {
            IsLoading = true;

            var entry = await BfmeWorkshopDownloadManager.Download(WorkshopEntry.Guid);

            if (entry.ExternalInstallerUrl() != "")
            {
                try
                {
                    string externalInstallerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BFME Workshop", "External", $"{string.Join("", entry.Name.Select(x => Path.GetInvalidPathChars().Contains(x) ? '_' : x))}-{entry.Guid}", "extinst.exe");
                    if (!Directory.Exists(Path.GetDirectoryName(externalInstallerPath))) Directory.CreateDirectory(Path.GetDirectoryName(externalInstallerPath)!);
                    await HttpsInstruments.Download(entry.ExternalInstallerUrl(), externalInstallerPath);

                    Process.Start(externalInstallerPath);
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                BfmeWorkshopLibraryManager.AddOrUpdate(entry);
                IsInLibrary = true;
            }

            IsLoading = false;
        }
        catch (Exception ex)
        {
            PopupVisualizer.ShowPopup(new ErrorPopup(ex));
            Dismiss();
        }

        IsLoading = false;
    }

    private async void Launch()
    {
        IsLoading = true;
        button_close.IsHitTestVisible = false;

        try
        {
            BfmeWorkshopEntry? activeEntry = await BfmeWorkshopManager.GetActivePatch(WorkshopEntry.Game);
            if (activeEntry == null || activeEntry.Value.Guid != WorkshopEntry.Guid)
                await BfmeWorkshopSyncManager.Sync(await BfmeWorkshopDownloadManager.Download(WorkshopEntry.Guid));

            BfmeLaunchManager.LaunchGame((BfmeGame)WorkshopEntry.Game);
        }
        catch (Exception ex)
        {
            PopupVisualizer.ShowPopup(new ErrorPopup(ex));
            Dismiss();
        }

        IsLoading = false;
        button_close.IsHitTestVisible = true;
    }

    private void OnMainButtonClicked(object sender, RoutedEventArgs e)
    {
        if (IsLoading)
            return;

        if (IsInLibrary)
            Launch();
        else
            AddToLibrary();
    }

    private void OnDismiss(object sender, RoutedEventArgs e) => Dismiss();
}