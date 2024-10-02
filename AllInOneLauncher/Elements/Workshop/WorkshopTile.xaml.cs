using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AllInOneLauncher.Core.Utils;
using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Elements.Menues;
using AllInOneLauncher.Popups;
using BfmeFoundationProject.WorkshopKit.Data;
using BfmeFoundationProject.WorkshopKit.Logic;

namespace AllInOneLauncher.Elements.Workshop;

public partial class WorkshopTile : UserControl
{
    public WorkshopTile()
    {
        InitializeComponent();
        Properties.Settings.Default.SettingsSaving += (s, e) => UpdateType();
    }

    BfmeWorkshopEntryPreview _workshopEntry;
    public BfmeWorkshopEntryPreview WorkshopEntry
    {
        get => _workshopEntry;
        set
        {
            _workshopEntry = value;
            try { icon.Source = new BitmapImage(new Uri(value.ArtworkUrl)); } catch { }
            title.Text = value.Name;
            version.Text = value.Version;
            author.Text = value.Author;
            IsInLibrary = BfmeWorkshopLibraryManager.IsInLibrary(value.Guid);
            UpdateType();
        }
    }

    public bool IsInLibrary
    {
        get => inLibraryIcon.Visibility == Visibility.Visible;
        set => inLibraryIcon.Visibility = value ? Visibility.Visible : Visibility.Hidden;
    }

    private void OnEnter(object sender, MouseEventArgs e) => hoverEffect.Opacity = 1;
    private void OnLeave(object sender, MouseEventArgs e) => hoverEffect.Opacity = 0;

    private void OnClicked(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            AddToLibrary();
        }
        else if (e.ChangedButton == MouseButton.Right)
        {
            MenuVisualizer.ShowMenu(
                menu: [
                    new ContextMenuButtonItem(IsInLibrary ? "Already in library" : "Add to library", !IsInLibrary, clicked: AddToLibrary),
                    new ContextMenuSeparatorItem(),
                    new ContextMenuButtonItem("Copy package GUID", true, clicked: () => Clipboard.SetDataObject(WorkshopEntry.Guid))
                ],
                owner: this,
                side: MenuSide.BottomRight,
                padding: 4,
                tint: true,
                minWidth: 200,
                targetCursor: true);
        }
    }

    private void UpdateType()
    {
        if (WorkshopEntry.Type == 0)
            entryType.Text = Application.Current.FindResource("LibraryTilePatchType").ToString()!;
        else if (WorkshopEntry.Type == 1)
            entryType.Text = Application.Current.FindResource("LibraryTileModType").ToString()!;
        else if (WorkshopEntry.Type == 2)
            entryType.Text = Application.Current.FindResource("LibraryTileEnhancementType").ToString()!;
        else if (WorkshopEntry.Type == 3)
            entryType.Text = Application.Current.FindResource("LibraryTileMapPackType").ToString()!;
    }

    private async void AddToLibrary()
    {
        try
        {
            var entry = await BfmeWorkshopDownloadManager.Download(WorkshopEntry.Guid);

            if (entry.ExternalInstallerUrl() != "")
            {
                var externalInstallerPopup = new ExternalInstallerPopup();
                PopupVisualizer.ShowPopup(externalInstallerPopup);

                try
                {
                    string externalInstallerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BFME Workshop", "External", $"{string.Join("", entry.Name.Select(x => Path.GetInvalidPathChars().Contains(x) ? '_' : x))}-{entry.Guid}", "extinst.exe");
                    if (!Directory.Exists(Path.GetDirectoryName(externalInstallerPath))) Directory.CreateDirectory(Path.GetDirectoryName(externalInstallerPath)!);
                    await HttpUtils.Download(entry.ExternalInstallerUrl(), externalInstallerPath, (progress) => Dispatcher.Invoke(() => externalInstallerPopup.LoadProgress = progress));

                    PopupVisualizer.ShowPopup(new ConfirmPopup("EXTERNAL INSTALLER", "Warning, you are about to run an unofficial third party installer! By clicking continue, you acknowledge that the Launcher does not guarantee the safety of this installer, and is not responsible for any problems or damages that might arrise from it's use."),
                    OnPopupSubmited: (submitedData) => Process.Start(externalInstallerPath));
                    externalInstallerPopup.Dismiss();
                }
                catch
                {
                    externalInstallerPopup.Dismiss();
                    throw;
                }
            }
            else
            {
                BfmeWorkshopLibraryManager.AddOrUpdate(entry);
                IsInLibrary = true;
            }
        }
        catch (Exception ex)
        {
            PopupVisualizer.ShowPopup(new ErrorPopup(ex));
        }
    }
}