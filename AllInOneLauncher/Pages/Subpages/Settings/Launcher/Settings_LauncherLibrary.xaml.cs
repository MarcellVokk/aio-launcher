using System.Windows.Controls;
using System.IO;
using System;
using System.Windows;
using AllInOneLauncher.Elements;
using System.Collections.Specialized;
using AllInOneLauncher.Logic;
using System.Linq;
using AllInOneLauncher.Popups;

namespace AllInOneLauncher.Pages.Subpages.Settings.Launcher
{
    /// <summary>
    /// Interaktionslogik für LauncherSettings_General.xaml
    /// </summary>
    public partial class Settings_LauncherLibrary : UserControl
    {
        public Settings_LauncherLibrary()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnLoad(object sender, RoutedEventArgs e) => LoadLibraryDrives();

        private void LoadLibraryDrives()
        {
            libraryDrives.Children.Clear();
            int i = 0;
            foreach (string libraryDrive in Properties.Settings.Default.LibraryLocations.OfType<string>().Where(x => x != null))
            {
                if (i != 0) libraryDrives.Children.Add(new Divider());
                libraryDrives.Children.Add(new LibraryDriveElement(libraryDrive));
                i++;
            }
        }

        private void OnAddNewLocationClicked(object sender, RoutedEventArgs e)
        {
            PopupVisualizer.ShowPopup(new SelectNewLocationPopup(),
            OnPopupSubmited: (submitedData) =>
            {
                Properties.Settings.Default.LibraryLocations.Add(submitedData[0]);
                Properties.Settings.Default.Save();
                LoadLibraryDrives();
            });
        }
    }
}