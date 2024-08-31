using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using AllInOneLauncher.Data;
using AllInOneLauncher.Views.Elements.Generic;

namespace AllInOneLauncher.Views.Pages.Subpages.Offline
{
    /// <summary>
    /// Interaktionslogik für Offline_News.xaml
    /// </summary>
    public partial class OfflineNews : UserControl
    {
        public BfmeGame AvailableBFMEGame { get; set; }

        private static string ChangelogBFME1 = "";
        private static string ChangelogBFME2 = "";
        private static string ChangelogRotwk = "";

        public OfflineNews()
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
                newsPage.Visibility = Visibility.Hidden;
                noConnection.Visibility = Visibility.Visible;
            }

            if (newsPage.CoreWebView2 != null)
            {
                newsPage.NavigateToString(GetNewsPage(BfmeGame.BFME1));
                SetNewsVisibility(PopupVisualizer.CurentPopup == null);
            }
            else
            {
                noConnection.Visibility = Visibility.Visible;
            }

            Load(BfmeGame.BFME1);
        }

        private void SetNewsVisibility(bool isVisible)
        {
            if (isVisible && newsPage.Visibility == Visibility.Hidden)
            {
                newsPagePlaceholder.Visibility = Visibility.Hidden;
                newsPage.Visibility = Visibility.Visible;
            }
            else if (!isVisible && newsPage.Visibility == Visibility.Visible)
            {
                newsPagePlaceholder.Visibility = Visibility.Visible;
                newsPage.Visibility = Visibility.Hidden;
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
                newsPage.Visibility = Visibility.Hidden;
                noConnection.Visibility = Visibility.Visible;
            }
            else
            {
                newsPage.Visibility = Visibility.Visible;
                noConnection.Visibility = Visibility.Hidden;

                await newsPage.EnsureCoreWebView2Async();
                newsPage.NavigateToString(GetNewsPage(game));
                SetNewsVisibility(PopupVisualizer.CurentPopup == null);
            }
        }
    }
}