using AllInOneLauncher.Elements;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace AllInOneLauncher.Popups
{
    /// <summary>
    /// Interaction logic for SelectNewLocationPopup.xaml
    /// </summary>
    public partial class SelectNewLocationPopup : PopupBody
    {
        private DriveInfo[] Drives = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed && !Properties.Settings.Default.LibraryLocations.OfType<string>().Any(y => Path.GetPathRoot(y) == x.Name)).ToArray();

        public SelectNewLocationPopup()
        {
            InitializeComponent();

            foreach (var drive in Drives) LocationDropdown.Options.Add($"{drive.VolumeLabel} ({drive.Name.Replace(@"\", "")})");
            LocationDropdown.Options.Add(App.Current.FindResource("SelectNewLocationPopupSelectCustom").ToString() ?? "");
        }

        private void ButtonAcceptClicked(object sender, RoutedEventArgs e)
        {
            if (LocationDropdown.Selected == LocationDropdown.Options.Count - 1)
            {
                FolderBrowserDialog ofd = new FolderBrowserDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Submit(ofd.SelectedPath);
                }
            }
            else
            {
                Submit(Path.Combine(Drives[LocationDropdown.Selected].Name, "BfmeLibrary"));
            }
        }

        private void ButtonCancelClicked(object sender, RoutedEventArgs e) => Dismiss();
    }
}
