using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for EntryPageScreenshotItem.xaml
    /// </summary>
    public partial class EntryPageScreenshotItem : UserControl
    {
        public EntryPageScreenshotItem()
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
