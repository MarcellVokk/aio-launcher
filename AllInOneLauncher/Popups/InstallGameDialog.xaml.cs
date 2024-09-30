using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using AllInOneLauncher.Elements.Disk;
using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Properties;

namespace AllInOneLauncher.Popups;

public partial class InstallGameDialog : PopupBody
{
    private static readonly Dictionary<string, DriveInfo> Drives =
        DriveInfo.GetDrives().ToDictionary(x => x.RootDirectory.FullName);

    public InstallGameDialog()
    {
        InitializeComponent();

        locations.Children.Clear();
        foreach (var drive in Drives.Values)
        {
            if (!drive.IsReady)
                continue;

            try
            {
                locations.Children.Add(new Selectable()
                {
                    Title = new LibraryDriveHeader()
                    {
                        LibraryDriveName = string.Concat(drive.VolumeLabel, " (", drive.Name.Replace(@"\", ""), ")"),
                        LibraryDriveSize =
                        $"{Math.Floor(drive.AvailableFreeSpace / Math.Pow(1024, 3)):N0} GB {App.Current.FindResource("GenericFree")}",
                        Mini = true
                    },
                    Tag = drive.RootDirectory.FullName,
                    Margin = new Thickness(0, 0, 0, 5),
                    UseLayoutRounding = true,
                    SnapsToDevicePixels = true
                });
            }
            catch { }
        }
    }

    private void ButtonAcceptClicked(object sender, RoutedEventArgs e) => Submit(LanguageDropdown.SelectedValue,
        Selectable.GetSelectedTagInContainer(locations)!.ToString()!);

    private void ButtonCancelClicked(object sender, RoutedEventArgs e) => Dismiss();
}