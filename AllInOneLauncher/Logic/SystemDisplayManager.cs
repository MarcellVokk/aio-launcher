using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AllInOneLauncher.Logic
{
    public static class SystemDisplayManager
    {
        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true)]
        internal static extern bool EnumDisplaySettings(string? deviceName, int modeNum, ref DEVMODE devMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            internal const int CCHDEVICENAME = 0x20;
            internal const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }

        public static List<string> GetAllSupportedResolutions()
        {
            List<string> allResolutions = [];
            DEVMODE vDevMode = new();
            int i = 0;

            while (EnumDisplaySettings(null, i, ref vDevMode))
            {
                if (vDevMode.dmDisplayFrequency == 60 && vDevMode.dmBitsPerPel == 32 && vDevMode.dmDisplayFixedOutput == 0)
                {
                    string resolution = vDevMode.dmPelsWidth + " " + vDevMode.dmPelsHeight;
                    allResolutions.Add(resolution);
                }

                i++;
            }

            allResolutions = allResolutions
                .Select(r => new { Resolution = r, Width = int.Parse(r.Split(' ')[0]), Height = int.Parse(r.Split(' ')[1]) })
                .OrderBy(r => r.Width)
                .ThenBy(r => r.Height)
                .Select(r => r.Resolution)
                .ToList();

            allResolutions.RemoveRange(0, Math.Min(3, allResolutions.Count));

            return allResolutions;
        }

        public static Size GetPrimaryScreenResolution()
        {
            using Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            return new Size((int)(Screen.PrimaryScreen!.Bounds.Width / (g.DpiX / 96)), (int)(Screen.PrimaryScreen.Bounds.Height / (g.DpiY / 96)));
        }
    }
}