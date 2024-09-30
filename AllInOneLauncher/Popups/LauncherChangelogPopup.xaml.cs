using System.Windows;
using AllInOneLauncher.Elements.Generic;

namespace AllInOneLauncher.Popups;

public partial class LauncherChangelogPopup : PopupBody
{
    public LauncherChangelogPopup()
    {
        InitializeComponent();
    }

    private void ButtonCancelClicked(object sender, RoutedEventArgs e)
    {
        Dismiss();
    }
}