using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

internal partial class SystemInputManager
{
    private static IntPtr _targetHWnd;

    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
    private static readonly WinEventDelegate _winEventDelegate = new(WinEventProc);

    public static void SetTargetHWnd(IntPtr hWnd)
    {
        _targetHWnd = hWnd;
        _ = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        ActivateWindow(_targetHWnd);
    }

    private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (hwnd == _targetHWnd)
        {
            UpdateClipCursor();
        }
    }

    private static void UpdateClipCursor()
    {
        if (_targetHWnd != IntPtr.Zero)
        {
            if (GetClientRect(_targetHWnd, out RECT clientRect) && clientRect.Width > 0 && clientRect.Height > 0)
            {
                POINT topLeft = new() { X = clientRect.Left, Y = clientRect.Top };
                POINT bottomRight = new() { X = clientRect.Right, Y = clientRect.Bottom };

                ClientToScreen(_targetHWnd, ref topLeft);
                ClientToScreen(_targetHWnd, ref bottomRight);

                RECT screenRect = new()
                {
                    Left = topLeft.X,
                    Top = topLeft.Y,
                    Right = bottomRight.X,
                    Bottom = bottomRight.Y
                };

                ClipCursor(ref screenRect);
                Debug.WriteLine("Cursor clipping reapplied.");
            }
        }
    }

    public static void ActivateWindow(IntPtr handle)
    {
        SetForegroundWindow(handle);
        BringWindowToTop(handle);
    }

    [LibraryImport("user32.dll")]
    private static partial IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetForegroundWindow(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool BringWindowToTop(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ClipCursor(ref RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public readonly int Width => Right - Left;
        public readonly int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    private const uint WINEVENT_OUTOFCONTEXT = 0;
}