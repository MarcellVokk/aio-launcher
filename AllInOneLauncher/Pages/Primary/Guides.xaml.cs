using System.Windows.Controls;

namespace AllInOneLauncher.Pages.Primary;

public partial class Guides : UserControl
{
    internal static Guides Instance = new();

    public Guides()
    {
        InitializeComponent();
        InitializeWebView();
        GuidesPage.Source = new("https://bfmelauncherfiles.ravonator.at/LauncherPages/guides/index.html");
    }

    private async void InitializeWebView()
    {
        await GuidesPage.EnsureCoreWebView2Async(App.GlobalWebView2Environment);
    }
}