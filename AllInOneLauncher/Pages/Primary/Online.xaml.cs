using AllInOneLauncher.Logic;
using System.Windows;
using System.Windows.Controls;

namespace AllInOneLauncher.Pages.Primary
{
    /// <summary>
    /// Interaction logic for Online.xaml
    /// </summary>
    public partial class Online : UserControl
    {
        internal static Online Instance = new();
        private bool FirstLoad = true;

        public Online()
        {
            InitializeComponent();
        }

        public void Unload() => arena.Unload();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!FirstLoad)
                return;

            FirstLoad = false;
            arena.Load();
        }
    }
}
