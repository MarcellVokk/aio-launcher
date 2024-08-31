using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AllInOneLauncher.Logic
{
    internal partial class SystemGameWindowManager
    {
        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrA", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial IntPtr GetWindowLongPtrA(IntPtr hWnd, int nIndex);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrA", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial IntPtr SetWindowLongPtrA(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private const int GWL_STYLE = -16;

        private const long WS_BORDER = 0x00800000L;
        private const long WS_DLGFRAME = 0x00C00000L;
        private const long WS_THICKFRAME = 0x00040000L;
        private const long WS_MINIMIZEBOX = 0x00020000L;
        private const long WS_MAXIMIZEBOX = 0x00010000L;

        private const int SWP_SHOWWINDOW = 0x0040;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int SWP_NOSIZE = 0x0001;

        public static IntPtr FindWindowByClassName(string className, int timeoutMilliseconds = 10000, int pollingInterval = 100)
        {
            IntPtr hWnd = IntPtr.Zero;
            int elapsedTime = 0;

            while (hWnd == IntPtr.Zero && elapsedTime < timeoutMilliseconds)
            {
                hWnd = FindWindowFromClassName(className);
                if (hWnd == IntPtr.Zero)
                {
                    Thread.Sleep(pollingInterval);
                    elapsedTime += pollingInterval;
                }
            }

            return hWnd;
        }

        private static IntPtr FindWindowFromClassName(string className)
        {
            IntPtr hWnd = IntPtr.Zero;

            _ = EnumWindows(delegate (IntPtr hWndTemp, IntPtr lParam)
            {
                StringBuilder sb = new(256);
                _ = GetClassName(hWndTemp, sb, sb.Capacity);
                if (sb.ToString() == className)
                {
                    hWnd = hWndTemp;
                    return false;
                }
                return true;
            }, IntPtr.Zero);

            return hWnd;
        }

        public static bool RemoveWindowBorder(IntPtr hWnd, int positionX = 0, int positionY = 0)
        {
            if (hWnd != IntPtr.Zero)
            {
                IntPtr stylePtr = GetWindowLongPtrA(hWnd, GWL_STYLE);
                long style = stylePtr.ToInt64();

                style &= ~(WS_BORDER | WS_DLGFRAME | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
                _ = SetWindowLongPtrA(hWnd, GWL_STYLE, new IntPtr(style));

                SetWindowPos(hWnd, IntPtr.Zero, positionX, positionY, 0, 0, SWP_SHOWWINDOW | SWP_FRAMECHANGED | SWP_NOSIZE);

                SystemInputManager.SetTargetHWnd(hWnd);
                return true;
            }

            return false;
        }
    }
}
