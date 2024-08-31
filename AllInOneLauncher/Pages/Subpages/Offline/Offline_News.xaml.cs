using AllInOneLauncher.Data;
using AllInOneLauncher.Elements;
using System;
using System.Net.Http;
using System.Windows.Controls;

namespace AllInOneLauncher.Pages.Subpages.Offline
{
    /// <summary>
    /// Interaktionslogik für Offline_News.xaml
    /// </summary>
    public partial class Offline_News : UserControl
    {
        public BfmeGame AvailableBFMEGame { get; set; }

        private static string ChangelogBFME1 = "";
        private static string ChangelogBFME2 = "";
        private static string ChangelogRotwk = "";

        public Offline_News()
        {
            InitializeComponent();
            InitPages();
            PopupVisualizer.OnPopupOpened += (s, e) => SetNewsVisibility(false);
            PopupVisualizer.OnPopupClosed += (s, e) => SetNewsVisibility(true);
        }

        private async void InitPages()
        {
            await newsPage.EnsureCoreWebView2Async(App.GlobalWebView2Environment);

            try
            {
                using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(10) };
                ChangelogBFME1 = (await client.GetStringAsync("https://bfmelauncherfiles.ravonator.at/LauncherPages/changelogpages/bfme1/index.html")).Replace("href=\"design.css\"", "href=\"https://bfmelauncherfiles.ravonator.at/LauncherPages/changelogpages/bfme1/design.css\"");
                ChangelogBFME2 = await client.GetStringAsync("https://bfmelauncherfiles.ravonator.at/LauncherPages/changelogpages/bfme2/106/changelog.html");
                ChangelogRotwk = await client.GetStringAsync("https://bfmelauncherfiles.ravonator.at/LauncherPages/changelogpages/rotwk/202/changelog.html");
            }
            catch
            {
                newsPage.Visibility = System.Windows.Visibility.Hidden;
                noConnection.Visibility = System.Windows.Visibility.Visible;
            }

            if (newsPage.CoreWebView2 != null)
            {
                newsPage.NavigateToString(GetNewsPage(BfmeGame.BFME1));
                SetNewsVisibility(PopupVisualizer.CurentPopup == null);
            }
            else
            {
                noConnection.Visibility = System.Windows.Visibility.Visible;
            }

            Load(BfmeGame.BFME1);
        }

        private void SetNewsVisibility(bool isVisible)
        {
            if (isVisible && newsPage.Visibility == System.Windows.Visibility.Hidden)
            {
                newsPagePlaceholder.Visibility = System.Windows.Visibility.Hidden;
                newsPage.Visibility = System.Windows.Visibility.Visible;
            }
            else if (!isVisible && newsPage.Visibility == System.Windows.Visibility.Visible)
            {
                newsPagePlaceholder.Visibility = System.Windows.Visibility.Visible;
                newsPage.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private static string GetNewsPage(BfmeGame game)
        {
            return game switch
            {
                BfmeGame.BFME1 => ChangelogBFME1,
                BfmeGame.BFME2 => ChangelogBFME2,
                BfmeGame.ROTWK => ChangelogRotwk,
                _ => throw new ArgumentOutOfRangeException(nameof(game), game, null)
            };
        }

        public async void Load(BfmeGame game)
        {
            string newsPageContent = GetNewsPage(game);
            if (newsPageContent == "")
            {
                newsPage.Visibility = System.Windows.Visibility.Hidden;
                noConnection.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                newsPage.Visibility = System.Windows.Visibility.Visible;
                noConnection.Visibility = System.Windows.Visibility.Hidden;

                await newsPage.EnsureCoreWebView2Async();
                newsPage.NavigateToString(GetNewsPage(game));
                SetNewsVisibility(PopupVisualizer.CurentPopup == null);
            }
        }
    }
}