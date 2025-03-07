using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for PackagePageScreenshotItem.xaml
    /// </summary>
    public partial class PackagePageScreenshotItem : UserControl
    {
        public PackagePageScreenshotItem()
        {
            InitializeComponent();
        }

        private string _imageSource = "";
        public string ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                try { image.Source = new BitmapImage(new Uri(value)); } catch { }
            }
        }
    }
}
