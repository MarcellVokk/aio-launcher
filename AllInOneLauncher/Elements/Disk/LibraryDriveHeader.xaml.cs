using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for DiskDriveHeader.xaml
    /// </summary>
    public partial class LibraryDriveHeader : UserControl
    {
        public LibraryDriveHeader()
        {
            InitializeComponent();
        }

        public string LibraryDriveName
        {
            get => libraryName.Text;
            set => libraryName.Text = value;
        }

        public string LibraryDriveSize
        {
            get => librarySize.Text;
            set => librarySize.Text = value;
        }

        public bool Mini
        {
            get => libraryName.FontSize == 12;
            set
            {
                libraryIcon.Visibility = value ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                libraryName.FontSize = value ? 12 : 14;
                librarySize.FontSize = value ? 11 : 14;
                librarySize.Foreground = value ? Brushes.White : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66FFFFFF"));
            }
        }
    }
}
