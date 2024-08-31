using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for Acrylic.xaml
    /// </summary>
    public partial class Acrylic : UserControl
    {
        public Acrylic()
        {
            InitializeComponent();
        }

        private void Update()
        {
            if (!IsLoaded || this.Background != null || this.FindCommonVisualAncestor(MainWindow.Instance!.windowGrid) == null)
                return;

            Point pos = this.TransformToVisual(MainWindow.Instance!.windowGrid).Transform(new Point(0, 0));
            image.Margin = new Thickness(-pos.X, -pos.Y, 0, 0);
            image.Width = MainWindow.Instance!.windowGrid.ActualWidth;
            image.Height = MainWindow.Instance!.windowGrid.ActualHeight;
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            Update();
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == (object)true)
                Update();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                this.Background = Brushes.Gray;

            Update();
        }
    }
}
