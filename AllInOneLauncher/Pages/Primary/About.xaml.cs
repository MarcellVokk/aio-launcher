using System.Windows.Controls;

namespace AllInOneLauncher.Pages.Primary
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        internal static About Instance = new();

        public About()
        {
            InitializeComponent();
        }
    }
}