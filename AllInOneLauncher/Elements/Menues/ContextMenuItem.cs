using System.Windows;

namespace AllInOneLauncher.Elements.Menues
{
    public abstract class ContextMenuItem
    {
        public abstract FrameworkElement GenerateElement(ContextMenuShell shell);
    }
}
