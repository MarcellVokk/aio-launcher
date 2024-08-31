using AllInOneLauncher.Logic;
using AllInOneLauncher.Views.Pages.Primary;
using AllInOneLauncher.Views.Popups;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace AllInOneLauncher.Views.Elements.Generic
{
    public abstract class PopupBody : UserControl
    {
        public Action<string[]>? OnSubmited;
        public Action? ClosePopup;

        public void Submit(params string[] data)
        {
            OnSubmited?.Invoke(data);
            ClosePopup?.Invoke();
        }

        public void Dismiss() => ClosePopup?.Invoke();
    }
}
