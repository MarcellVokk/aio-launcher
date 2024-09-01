using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AllInOneLauncher.Core.Managers;

namespace AllInOneLauncher.Core.Builder;

internal static class TrayIconContextMenuBuilder
{
    public static ContextMenu BuildContextMenu()
    {
        ContextMenu contextMenu = new()
        {
            Background = Brushes.White
        };

        MenuItem showApplicationItem = new()
        {
            Header = Application.Current.FindResource("TrayIconShowApplication")
        };
        showApplicationItem.Click += (s, e) => LauncherStateManager.Visible = true;
        contextMenu.Items.Add(showApplicationItem);

        MenuItem closeApplicationItem = new()
        {
            Header = Application.Current.FindResource("TrayIconCloseApplication")
        };
        closeApplicationItem.Click += (s, e) => Application.Current.Shutdown();
        contextMenu.Items.Add(closeApplicationItem);

        return contextMenu;
    }
}