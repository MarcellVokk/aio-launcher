using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AllInOneLauncher.Core.Managers;

public static class TabManager
{
    /// <summary>
    /// Highlights the selected tab and resets the styles for other tabs.
    /// </summary>
    /// <param name="selectedTab">The tab to highlight.</param>
    /// <param name="tabsContainer">The container holding all the tabs.</param>
    public static void HighlightTab(TextBlock selectedTab, UIElementCollection tabsContainer)
    {
        foreach (TextBlock tab in tabsContainer.OfType<TextBlock>())
        {
            if (tab == selectedTab)
            {
                // Highlight the selected tab
                tab.Foreground = new SolidColorBrush(Color.FromRgb(21, 167, 233));
            }
            else
            {
                // Reset the style for other tabs
                tab.Foreground = Brushes.White;
                tab.Style = (Style)Application.Current.FindResource("TextBlockHover");
            }
        }
    }
}