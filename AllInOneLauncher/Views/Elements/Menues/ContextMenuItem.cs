using System.Windows;

namespace AllInOneLauncher.Views.Elements.Menues
{
    public abstract class ContextMenuItem
    {
        public abstract FrameworkElement GenerateElement(ContextMenuShell shell);
    }
}
