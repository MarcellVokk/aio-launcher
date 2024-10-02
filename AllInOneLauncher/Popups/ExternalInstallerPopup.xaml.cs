using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AllInOneLauncher.Core.Managers;
using AllInOneLauncher.Elements.Generic;

namespace AllInOneLauncher.Popups;

public partial class ExternalInstallerPopup : PopupBody
{
    public ExternalInstallerPopup()
    {
        InitializeComponent();
        LoadProgress = 0;
    }

    public double LoadProgress
    {
        get => (double)GetValue(LoadProgressProperty);
        set
        {
            SetValue(LoadProgressProperty, value);
            progressText.Text = $"{value}%";
        }
    }

    public static readonly DependencyProperty LoadProgressProperty = DependencyProperty.Register("LoadProgress",
        typeof(double), typeof(ExternalInstallerPopup), new PropertyMetadata(OnLoadProgressChangedCallBack));

    private static void OnLoadProgressChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ExternalInstallerPopup progressBar = (ExternalInstallerPopup)sender;
        if (progressBar != null)
        {
            DoubleAnimation da = new DoubleAnimation()
            {
                To = (double)e.NewValue / 100d, Duration = TimeSpan.FromSeconds((double)e.NewValue == 0d ? 0d : 0.5d)
            };
            progressBar.progressGradientStop1.BeginAnimation(GradientStop.OffsetProperty, da, HandoffBehavior.Compose);
            progressBar.progressGradientStop2.BeginAnimation(GradientStop.OffsetProperty, da, HandoffBehavior.Compose);
        }
    }
}