using System;
using System.Windows;
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

    private void OnCancelClicked(object sender, RoutedEventArgs e) => Dismiss();

    private void OnCopyError(object sender, RoutedEventArgs e)
    {
        Clipboard.SetDataObject($"{title.Text}\n{stackTrace.Text}");

        MenuVisualizer.ShowMenu(Application.Current.FindResource("ErrorPopupErrorCopied").ToString() ?? "", copyError, MenuSide.Bottom, colorStyle: ColorStyle.Navy, lifetime: 3000);
    }
}