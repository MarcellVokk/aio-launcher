using AllInOneLauncher.Elements;
using System.Windows;

namespace AllInOneLauncher.Popups
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
