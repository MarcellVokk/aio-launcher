using System;
using System.Windows;
using System.Windows.Controls;
using AllInOneLauncher.Elements.Generic;

namespace AllInOneLauncher.Popups;

public partial class ErrorPopup : PopupBody
{
    public ErrorPopup(Exception exception)
    {
        InitializeComponent();
        title.Text = exception.GetType().FullName;
        stackTrace.Text = $"{exception.Message}\n{exception.StackTrace}";
    }

    private void ButtonCancelClicked(object sender, RoutedEventArgs e) => Dismiss();

    private void OnCopyErrorClicked(object sender, RoutedEventArgs e)
    {
        Clipboard.SetDataObject($"{title.Text}\n{stackTrace.Text}");

        if (sender is Button button)
        {
            button.Content = Application.Current.FindResource("ErrorPopupCopyErrorClicked").ToString()!;
        }
    }
}