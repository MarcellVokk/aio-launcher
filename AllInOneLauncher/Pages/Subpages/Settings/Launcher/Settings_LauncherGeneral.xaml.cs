using System.Windows.Controls;
using System.IO;
using System;
using System.Windows;
using AllInOneLauncher.Logic;
using WindowsShortcutFactory;

namespace AllInOneLauncher.Pages.Subpages.Settings.Launcher
{
    public partial class Settings_LauncherGeneral : UserControl
    {
        private readonly string desktopShortCutFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Data.Constants.C_LAUNCHER_SHORTCUT_NAME);
        public Settings_LauncherGeneral()
        {
            InitializeComponent();
            DataContext = this;
            UpdateDesktopIconStatus();
        }

        private void UpdateDesktopIconStatus()
        {
            if (File.Exists(desktopShortCutFilePath))
            {
                CreateDesktopIcon.IsToggled = true;
                Properties.Settings.Default.CreateLauncherDesktopIcon = true;
            }
            else
            {
                CreateDesktopIcon.IsToggled = false;
                Properties.Settings.Default.CreateLauncherDesktopIcon = false;
            }
            Properties.Settings.Default.Save();
        }

        private void CreateOrDeleteDesktopShortcut(bool create)
        {
            if (create)
            {
                using var shortcut = new WindowsShortcut
                {
                    Path = Environment.ProcessPath,
                    Description = "All-in-One Lord of the Rings Launcher by Ravo92, MarcellVokk & the Bfme Foundation Project"
                };
                shortcut.Save(desktopShortCutFilePath);
            }
            else
            {
                if (File.Exists(desktopShortCutFilePath))
                {
                    File.Delete(desktopShortCutFilePath);
                }
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            LanguageDropdown.Selected = Properties.Settings.Default.LauncherLanguage;
            CloseToTrayToggle.IsToggled = Properties.Settings.Default.HideToTrayOnClose;
            CreateDesktopIcon.IsToggled = Properties.Settings.Default.CreateLauncherDesktopIcon;
        }

        private void OnLanguageOptionSelected(object sender, EventArgs e)
        {
            LauncherStateManager.Language = LanguageDropdown.Selected;
        }

        private void OnCloseToTraySwitched(object sender, EventArgs e)
        {
            Properties.Settings.Default.HideToTrayOnClose = CloseToTrayToggle.IsToggled;
            Properties.Settings.Default.Save();
        }

        private void OnCreateDesktopIconSwitched(object sender, EventArgs e)
        {
            CreateOrDeleteDesktopShortcut(CreateDesktopIcon.IsToggled);
            Properties.Settings.Default.CreateLauncherDesktopIcon = CreateDesktopIcon.IsToggled;
            Properties.Settings.Default.Save();
        }
    }
}