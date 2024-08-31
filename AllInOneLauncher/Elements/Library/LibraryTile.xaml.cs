using BfmeFoundationProject.WorkshopKit.Data;
using BfmeFoundationProject.WorkshopKit.Logic;
using AllInOneLauncher.Popups;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using AllInOneLauncher.Elements.Menues;
using AllInOneLauncher.Pages.Primary;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AllInOneLauncher.Logic;
using AllInOneLauncher.Data;
using BfmeFoundationProject.RegistryKit;
using BfmeFoundationProject.RegistryKit.Data;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for LibraryTile.xaml
    /// </summary>
    public partial class LibraryTile : UserControl
    {
        public LibraryTile()
        {
            InitializeComponent();
            Properties.Settings.Default.SettingsSaving += (s, e) => UpdateType();

            BfmeWorkshopSyncManager.OnSyncBegin += OnSyncBegin;
            BfmeWorkshopSyncManager.OnSyncUpdate += OnSyncUpdate;
            BfmeWorkshopSyncManager.OnSyncEnd += OnSyncEnd;
        }

        BfmeWorkshopEntryPreview _workshopEntry;
        public BfmeWorkshopEntryPreview WorkshopEntry
        {
            get => _workshopEntry;
            set
            {
                _workshopEntry = value;
                title.Text = value.Name;
                version.Text = value.Version;
                author.Text = value.Author;

                IsHitTestVisible = BfmeRegistryManager.IsInstalled(value.Game);
                content.Opacity = IsHitTestVisible ? 1 : 0.5;
                if (IsHitTestVisible)
                    try { icon.Source = new BitmapImage(new Uri(value.ArtworkUrl)); } catch { }
                else
                    try { icon.Source = new FormatConvertedBitmap(new BitmapImage(new Uri(value.ArtworkUrl)), PixelFormats.Gray16, BitmapPalettes.Gray16, 1); } catch { }

                UpdateType();
                Task.Run(UpdateIsActive);
            }
        }

        BfmeWorkshopEntryMetadata _workshopMetadata;
        public BfmeWorkshopEntryMetadata WorkshopMetadata
        {
            get => _workshopMetadata;
            set => _workshopMetadata = value;
        }

        public bool IsLoading
        {
            get => loadingBar.Visibility == Visibility.Visible;
            set
            {
                loadingBar.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                tags.Visibility = value ? Visibility.Hidden : Visibility.Visible;
                loadingIcon.IsLoading = value;
                isActiveIcon.Visibility = value ? Visibility.Collapsed : Visibility.Visible;

                if (value)
                    LoadProgress = 0;
            }
        }

        public double LoadProgress
        {
            get => (double)GetValue(LoadProgressProperty);
            set
            {
                SetValue(LoadProgressProperty, value);
                progressText.Text = $"{value}%";
            }
        }
        public static readonly DependencyProperty LoadProgressProperty = DependencyProperty.Register("LoadProgress", typeof(double), typeof(LibraryTile), new PropertyMetadata(OnLoadProgressChangedCallBack));
        private static void OnLoadProgressChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            LibraryTile progressBar = (LibraryTile)sender;
            if (progressBar != null)
            {
                DoubleAnimation da = new() { To = (double)e.NewValue / 100d, Duration = TimeSpan.FromSeconds((double)e.NewValue == 0d ? 0d : 0.5d) };
                progressBar.progressGradientStop1.BeginAnimation(GradientStop.OffsetProperty, da, HandoffBehavior.Compose);
                progressBar.progressGradientStop2.BeginAnimation(GradientStop.OffsetProperty, da, HandoffBehavior.Compose);
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e) => Task.Run(CheckForUpdates);
        private void OnEnter(object sender, MouseEventArgs e) => hoverEffect.Opacity = 1;
        private void OnLeave(object sender, MouseEventArgs e) => hoverEffect.Opacity = 0;

        private void OnSyncBegin(BfmeWorkshopEntry entry)
        {
            if (entry.Game != WorkshopEntry.Game)
                return;

            Dispatcher.Invoke(() =>
            {
                IsHitTestVisible = false;
                if (WorkshopEntry.Type == 0 || WorkshopEntry.Type == 1 || WorkshopEntry.Type == 4)
                    IsLoading = entry.Guid == WorkshopEntry.Guid;
            });
        }

        private void OnSyncUpdate(int progress)
        {
            Dispatcher.Invoke(() =>
            {
                if (IsLoading)
                    LoadProgress = progress;
            });
        }

        private void OnSyncEnd()
        {
            UpdateIsActive();
            Dispatcher.Invoke(() =>
            {
                IsLoading = false;
                IsHitTestVisible = BfmeRegistryManager.IsInstalled(WorkshopEntry.Game);
                content.Opacity = IsHitTestVisible ? 1 : 0.5;
                try
                {
                    if (IsHitTestVisible)
                        icon.Source = new BitmapImage(new Uri(WorkshopEntry.ArtworkUrl));
                    else
                        icon.Source = new FormatConvertedBitmap(new BitmapImage(new Uri(WorkshopEntry.ArtworkUrl)), PixelFormats.Gray16, BitmapPalettes.Gray16, 1);
                }
                catch { }
            });
        }

        private void OnClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Sync();
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (WorkshopEntry.Type == 0 || WorkshopEntry.Type == 1 || WorkshopEntry.Type == 4)
                {
                    MenuVisualizer.ShowMenu(
                    menu: [
                        new ContextMenuButtonItem(isActiveIcon.Opacity == 0d ? $"Switch to \"{WorkshopEntry.Name}\"" : "Sync again", true, clicked: () => Sync()),
                        new ContextMenuSeparatorItem(),
                        new ContextMenuSubmenuItem("Sync old version", submenu: WorkshopMetadata.Versions != null ? WorkshopMetadata.Versions.Where(x => x != WorkshopEntry.Version).Reverse<string>().Select(x => new ContextMenuButtonItem(x, true, clicked: () => Sync(x)) as ContextMenuItem).ToList() : [], WorkshopMetadata.Versions != null && WorkshopMetadata.Versions.Count > 1),
                        new ContextMenuSeparatorItem(),
                        new ContextMenuButtonItem("Open keybinds folder", true, clicked: () =>
                        {
                            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BFME Workshop", "Keybinds", $"{WorkshopEntry.GameName()}-{WorkshopEntry.Name}")))
                                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BFME Workshop", "Keybinds", $"{WorkshopEntry.GameName()}-{WorkshopEntry.Name}"));
                            Process.Start(new ProcessStartInfo("explorer.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BFME Workshop", "Keybinds", $"{WorkshopEntry.GameName()}-{WorkshopEntry.Name}")));
                        }),
                        new ContextMenuButtonItem("Open game folder", true, clicked: () => Process.Start(new ProcessStartInfo("explorer.exe", Path.Combine(BfmeRegistryManager.GetKeyValue(WorkshopEntry.Game, BfmeRegistryKey.InstallPath))))),
                        new ContextMenuSeparatorItem(),
                        new ContextMenuButtonItem("Copy package GUID", true, clicked: () => Clipboard.SetDataObject(WorkshopEntry.Guid)),
                        new ContextMenuSeparatorItem(),
                        new ContextMenuButtonItem("Remove from library", true, clicked: RemoveFromLibrary)
                    ],
                    owner: this,
                    side: MenuSide.BottomRight,
                    padding: 4,
                    tint: true,
                    minWidth: 200,
                    targetCursor: true);
                }
                else
                {
                    MenuVisualizer.ShowMenu(
                    menu: [
                        new ContextMenuButtonItem(isActiveIcon.Opacity == 0d ? $"Enable \"{WorkshopEntry.Name}\"" : "Disable", true, clicked: () => Sync()),
                        new ContextMenuSeparatorItem(),
                        new ContextMenuSubmenuItem("Sync old version", submenu: WorkshopMetadata.Versions != null ? WorkshopMetadata.Versions.Where(x => x != WorkshopEntry.Version).Reverse<string>().Select(x => new ContextMenuButtonItem(x, true, clicked: () => Sync(x)) as ContextMenuItem).ToList() : [], WorkshopMetadata.Versions != null && WorkshopMetadata.Versions.Count > 1),
                        new ContextMenuSeparatorItem(),
                        new ContextMenuButtonItem("Copy package GUID", true, clicked: () => Clipboard.SetDataObject(WorkshopEntry.Guid)),
                        new ContextMenuSeparatorItem(),
                        new ContextMenuButtonItem("Remove from library", true, clicked: RemoveFromLibrary)
                    ],
                    owner: this,
                    side: MenuSide.BottomRight,
                    padding: 4,
                    tint: true,
                    minWidth: 200,
                    targetCursor: true);
                }
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
            else if (WorkshopEntry.Type == 4)
                entryType.Text = Application.Current.FindResource("LibraryTileSnapshotType").ToString()!;
        }

        private void UpdateIsActive()
        {
            if (WorkshopEntry.Type == 0 || WorkshopEntry.Type == 1 || WorkshopEntry.Type == 4)
            {
                bool isActive = BfmeWorkshopStateManager.IsPatchActive(WorkshopEntry.Game, WorkshopEntry.Guid);
                Dispatcher.Invoke(() =>
                {
                    activeText.Visibility = Visibility.Visible;
                    isActiveIcon.Opacity = isActive ? 1d : 0d;
                });
            }
            else
            {
                bool isActive = BfmeWorkshopStateManager.IsEnhancementActive(WorkshopEntry.Game, WorkshopEntry.Guid);
                Dispatcher.Invoke(() =>
                {
                    activeText.Visibility = Visibility.Collapsed;
                    isActiveIcon.Opacity = isActive ? 1d : 0d;
                });
            }
        }

        private async void Sync(string version = "")
        {
            try
            {
                if (version == "")
                    await BfmeWorkshopSyncManager.Sync((await BfmeWorkshopLibraryManager.Get(WorkshopEntry.Guid)).Value);
                else
                    await BfmeWorkshopSyncManager.Sync(await BfmeWorkshopDownloadManager.Download($"{WorkshopEntry.Guid}:{version}"));
            }
            catch (BfmeWorkshopEnhancementIncompatibleSyncException ex)
            {
                PopupVisualizer.ShowPopup(new MessagePopup("COMPATIBILITY ERROR", ex.Message));
            }
            catch (Exception ex)
            {
                PopupVisualizer.ShowPopup(new ErrorPopup(ex));
            }
        }

        private async void CheckForUpdates()
        {
            try
            {
                var workshopVersion = await BfmeWorkshopQueryManager.Get(WorkshopEntry.Guid);
                WorkshopMetadata = workshopVersion.metadata;
                if (WorkshopEntry.Version != workshopVersion.entry.Version)
                {
                    Dispatcher.Invoke(() => WorkshopEntry = workshopVersion.entry);
                    BfmeWorkshopLibraryManager.AddOrUpdate(await BfmeWorkshopDownloadManager.Download(workshopVersion.entry.Guid));
                }
            }
            catch { }
        }

        private void RemoveFromLibrary()
        {
            BfmeWorkshopLibraryManager.Remove(WorkshopEntry.Guid);
            Offline.Instance.library.libraryTiles.Children.Remove(this);
        }
    }
}
