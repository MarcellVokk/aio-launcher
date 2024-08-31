using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for PopupVisualizer.xaml
    /// </summary>
    public partial class PopupVisualizer : UserControl
    {
        private static PopupVisualizer? Instance;
        private static readonly List<Action> PopupQueue = [];

        public static event EventHandler? OnPopupOpened;
        public static event EventHandler? OnPopupClosed;

        public static PopupBody? CurentPopup => (Instance != null && Instance.content.Child is PopupBody body && ((PopupBody)Instance.content.Child).ClosePopup != null) ? body : null;

        public PopupVisualizer()
        {
            InitializeComponent();
            Instance = this;
        }

        public static void ShowPopup(PopupBody popup, Action<string[]>? OnPopupSubmited = null, Action? OnPopupClosed = null)
        {
            if (Instance == null)
                return;

            if (Instance.root.IsHitTestVisible)
            {
                PopupQueue.Add(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.22));
                    Instance.root.IsHitTestVisible = false;
                    ShowPopup(popup, OnPopupSubmited);
                });
                return;
            }

            popup.OnSubmited = OnPopupSubmited;
            popup.ClosePopup = () =>
            {
                HidePopup();
                OnPopupClosed?.Invoke();
            };

            if (Instance.content.Child is PopupBody body)
            {
                body.OnSubmited = null;
                body.ClosePopup = null;
            }

            Instance.content.Child = popup;
            Instance.root.IsHitTestVisible = true;
            OnPopupOpened?.Invoke(null, EventArgs.Empty);

            Instance.popupBody.Margin = popup.Margin;
            Instance.popupBody.VerticalAlignment = popup.VerticalAlignment;
            Instance.popupBody.HorizontalAlignment = popup.HorizontalAlignment;

            popup.Margin = new Thickness(0);
            popup.VerticalAlignment = VerticalAlignment.Stretch;
            popup.HorizontalAlignment = HorizontalAlignment.Stretch;

            Instance.popupBody.RenderTransformOrigin = new Point(0.5, 0.5);
            Instance.popupBody.RenderTransform = new ScaleTransform(1, 1);

            DoubleAnimation scaleAnimation = new() { From = 0.4, To = 1, Duration = TimeSpan.FromSeconds(0.2), EasingFunction = new QuadraticEase() };
            Instance.popupBody.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            Instance.popupBody.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            DoubleAnimation opacityAnimation = new() { From = 0, To = 1, Duration = TimeSpan.FromSeconds(0.2), EasingFunction = new QuadraticEase() };
            Instance.root.BeginAnimation(OpacityProperty, opacityAnimation);

            ((Panel)Instance.Parent).Children.OfType<FrameworkElement>().First(x => x.Name == "outerContent").Effect = new BlurEffect() { Radius = 8 };
        }

        public static void HidePopup()
        {
            if (Instance == null)
                return;

            if (Instance.content.Child is not PopupBody)
                return;

            if (PopupQueue.Count > 0)
            {
                Action popup = PopupQueue.First();
                PopupQueue.Remove(popup);
                popup.Invoke();
            }
            else
            {
                Instance.root.IsHitTestVisible = false;
                OnPopupClosed?.Invoke(null, EventArgs.Empty);
            }

            if (Instance.content.Child is PopupBody body)
            {
                body.OnSubmited = null;
                body.ClosePopup = null;
            }

            DoubleAnimation scaleAnimation = new() { To = 0.4, Duration = TimeSpan.FromSeconds(0.2), EasingFunction = new QuadraticEase() };
            Instance.popupBody.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            Instance.popupBody.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            DoubleAnimation opacityAnimation = new() { To = 0, Duration = TimeSpan.FromSeconds(0.2), EasingFunction = new QuadraticEase() };
            Instance.root.BeginAnimation(OpacityProperty, opacityAnimation);

            ((Panel)Instance.Parent).Children.OfType<FrameworkElement>().First(x => x.Name == "outerContent").Effect = null;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //HidePopup();
        }
    }
}