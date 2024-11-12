using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for EntryPageScreenshotLibrary.xaml
    /// </summary>
    public partial class EntryPageScreenshotLibrary : UserControl
    {
        public EntryPageScreenshotLibrary()
        {
            InitializeComponent();
        }

        private List<string> _screenshots = [];
        public List<string> Screenshots
        {
            get => _screenshots;
            set
            {
                _screenshots = value;

                images.Children.Clear();
                foreach (var screenshot in value)
                    images.Children.Add(new EntryPageScreenshotItem() { ImageSource = screenshot, Margin = new Thickness(0, 0, 10, 0) });

                if (value.Count != 0)
                    images.Children.OfType<EntryPageScreenshotItem>().Last().Margin = new Thickness(0);

                gradientStart.Color = CurentOffset == 0 ? Colors.White : Colors.Transparent;
                gradientEnd.Color = (CurentOffset == _screenshots.Count - 1 || _screenshots.Count == 0) ? Colors.White : Colors.Transparent;
                buttonBack.Visibility = CurentOffset == 0 ? Visibility.Hidden : Visibility.Visible;
                buttonForward.Visibility = (CurentOffset == _screenshots.Count - 1 || _screenshots.Count == 0) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private int CurentOffset = 0;

        private async void OnBack(object sender, RoutedEventArgs e)
        {
            if (_screenshots.Count == 0)
                return;

            if (Math.Clamp(CurentOffset - 1, 0, _screenshots.Count - 1) != CurentOffset)
            {
                CurentOffset = Math.Clamp(CurentOffset - 1, 0, _screenshots.Count - 1);

                double offsetSize = CurentOffset * (576 + 10);
                offsetSize = Math.Clamp(offsetSize, 0, (_screenshots.Count * (576 + 10)) - this.ActualWidth);

                ThicknessAnimation ta = new ThicknessAnimation() { To = new Thickness(-offsetSize, 0, 0, 0), Duration = TimeSpan.FromSeconds(0.2), EasingFunction = new QuadraticEase() };
                images.BeginAnimation(FrameworkElement.MarginProperty, ta);

                await Task.Delay(TimeSpan.FromSeconds(0.1));

                gradientStart.Color = CurentOffset == 0 ? Colors.White : Colors.Transparent;
                gradientEnd.Color = (CurentOffset == _screenshots.Count - 1 || _screenshots.Count == 0) ? Colors.White : Colors.Transparent;
                buttonBack.Visibility = CurentOffset == 0 ? Visibility.Hidden : Visibility.Visible;
                buttonForward.Visibility = (CurentOffset == _screenshots.Count - 1 || _screenshots.Count == 0) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private async void OnForward(object sender, RoutedEventArgs e)
        {
            if (_screenshots.Count == 0)
                return;

            if (Math.Clamp(CurentOffset + 1, 0, _screenshots.Count - 1) != CurentOffset)
            {
                CurentOffset = Math.Clamp(CurentOffset + 1, 0, _screenshots.Count - 1);

                double offsetSize = CurentOffset * (576 + 10);
                offsetSize = Math.Clamp(offsetSize, 0, (_screenshots.Count * (576 + 10)) - this.ActualWidth);

                ThicknessAnimation ta = new ThicknessAnimation() { To = new Thickness(-offsetSize, 0, 0, 0), Duration = TimeSpan.FromSeconds(0.2), EasingFunction = new QuadraticEase() };
                images.BeginAnimation(FrameworkElement.MarginProperty, ta);

                await Task.Delay(TimeSpan.FromSeconds(0.1));

                gradientStart.Color = CurentOffset == 0 ? Colors.White : Colors.Transparent;
                gradientEnd.Color = (CurentOffset == _screenshots.Count - 1 || _screenshots.Count == 0) ? Colors.White : Colors.Transparent;
                buttonBack.Visibility = CurentOffset == 0 ? Visibility.Hidden : Visibility.Visible;
                buttonForward.Visibility = (CurentOffset == _screenshots.Count - 1 || _screenshots.Count == 0) ? Visibility.Hidden : Visibility.Visible;
            }
        }
    }
}
