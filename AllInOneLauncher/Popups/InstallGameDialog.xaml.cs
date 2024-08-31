using AllInOneLauncher.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace AllInOneLauncher.Popups
{
    /// <summary>
    /// Interaction logic for InstallGameDialog.xaml
    /// </summary>
    public partial class InstallGameDialog : PopupBody
    {
        private static readonly Dictionary<string, DriveInfo> Drives = DriveInfo.GetDrives().ToDictionary(x => x.RootDirectory.FullName);

        public InstallGameDialog()
        {
            InitializeComponent();

            locations.Children.Clear();
            foreach (string libraryPath in Properties.Settings.Default.LibraryLocations.OfType<string>().Where(x => x != null))
            {
                DriveInfo drive = Drives[$@"{libraryPath.Split(@":\").First()}:\"];

                locations.Children.Add(new Selectable()
                {
                    Title = new LibraryDriveHeader() { LibraryDriveName = string.Concat(drive.VolumeLabel, " (", drive.Name.Replace(@"\", ""), ")"), LibraryDriveSize = $"{Math.Floor(drive.AvailableFreeSpace / Math.Pow(1024, 3)):N0} GB {App.Current.FindResource("GenericFree")}", Mini = true },
                    Tag = libraryPath,
                    Margin = new Thickness(0, 0, 0, 5),
                    UseLayoutRounding = true,
                    SnapsToDevicePixels = true
                });
            }
        }

        private void ButtonAcceptClicked(object sender, RoutedEventArgs e) => Submit(LanguageDropdown.SelectedValue, Selectable.GetSelectedTagInContainer(locations)!.ToString()!);

        private void ButtonCancelClicked(object sender, RoutedEventArgs e) => Dismiss();
    }
}
