using System.Windows.Controls;
using System.Windows.Input;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for LibraryTileEmpty.xaml
    /// </summary>
    public partial class LibraryTileEmpty : UserControl
    {
        public LibraryTileEmpty()
        {
            InitializeComponent();
        }

        private void OnEnter(object sender, MouseEventArgs e)
        {
            hoverEffect.Opacity = 1;
        }

        private void OnLeave(object sender, MouseEventArgs e)
        {
            hoverEffect.Opacity = 0;
        }
    }
}
