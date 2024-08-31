using System.Windows.Controls;

namespace AllInOneLauncher.Pages.Primary
{
    /// <summary>
    /// Interaction logic for Patreons.xaml
    /// </summary>
    public partial class Patreons : UserControl
    {
        internal static Patreons Instance = new();

        public Patreons()
        {
            InitializeComponent();
            InitializeWebView();
            PatreonsPage.Source = new("https://bfmelauncherfiles.ravonator.at/LauncherPages/patreons/index.html");
        }

        private async void InitializeWebView()
        {
            await PatreonsPage.EnsureCoreWebView2Async(App.GlobalWebView2Environment);
        }
    }
}