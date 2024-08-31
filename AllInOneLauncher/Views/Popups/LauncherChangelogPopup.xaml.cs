using System.Windows;
using AllInOneLauncher.Views.Elements.Generic;

namespace AllInOneLauncher.Views.Popups
{
    /// <summary>
    /// Interaction logic for LauncherChangelogPopup.xaml
    /// </summary>
    public partial class LauncherChangelogPopup : PopupBody
    {
        public LauncherChangelogPopup()
        {
            InitializeComponent();
        }

        private void ButtonCancelClicked(object sender, RoutedEventArgs e)
        {
            ChangelogPage.Visibility = Visibility.Collapsed;

            Dismiss();
        }
    }
}
