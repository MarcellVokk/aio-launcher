using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for LibraryDriveElement.xaml
    /// </summary>
    public partial class LibraryDriveElement : UserControl
    {
        private static readonly Dictionary<string, DriveInfo> Drives = DriveInfo.GetDrives().ToDictionary(x => x.RootDirectory.FullName);

        public LibraryDriveElement(string libraryPath)
        {
            InitializeComponent();
            LoadLibraryDrive(libraryPath);
        }

        private async void LoadLibraryDrive(string libraryPath)
        {
            DriveInfo drive = Drives[$@"{libraryPath.Split(@":\").First()}:\"];

            header.LibraryDriveName = $"{drive.VolumeLabel} ({drive.Name.Replace(@"\", "")})";
            header.LibraryDriveSize = $"{Math.Floor(drive.AvailableFreeSpace / Math.Pow(1024, 3)):N0} GB {Application.Current.FindResource("SettingsPageLauncherSectionLibraryDriveSizeText")} {Math.Floor(drive.TotalSize / Math.Pow(1024, 3)):N0} GB{Application.Current.FindResource("SettingsPageLauncherSectionLibraryDriveSizeTextSufix").ToString()!.Replace("_", "")}";

            long gamesSize = 0;
            if (Directory.Exists(Path.Combine(libraryPath, "BFME1"))) gamesSize += await Task.Run(() => new DirectoryInfo(Path.Combine(libraryPath, "BFME1")).EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length));
            if (Directory.Exists(Path.Combine(libraryPath, "BFME2"))) gamesSize += await Task.Run(() => new DirectoryInfo(Path.Combine(libraryPath, "BFME2")).EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length));
            if (Directory.Exists(Path.Combine(libraryPath, "ROTWK"))) gamesSize += await Task.Run(() => new DirectoryInfo(Path.Combine(libraryPath, "ROTWK")).EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length));
            this.gamesSize.Text = $"{Math.Floor(gamesSize / Math.Pow(1024, 3)):N0} GB";
            this.gamesBar.Width = (double)gamesSize / (double)drive.TotalSize * this.Width;

            long workshopSize = 0;
            if (Directory.Exists(Path.Combine(libraryPath, "BFME Workshop"))) workshopSize += await Task.Run(() => new DirectoryInfo(Path.Combine(libraryPath, "BFME Workshop")).EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length));
            this.workshopSize.Text = $"{Math.Floor(workshopSize / Math.Pow(1024, 3)):N0} GB";
            this.workshopBar.Width = (double)(gamesSize + workshopSize) / (double)drive.TotalSize * this.Width;

            long nonLauncherSize = drive.TotalSize - drive.AvailableFreeSpace - gamesSize - workshopSize;
            this.nonLauncherSize.Text = $"{Math.Floor(nonLauncherSize / Math.Pow(1024, 3)):N0} GB";
            this.nonLauncherBar.Width = (double)(nonLauncherSize) / (double)drive.TotalSize * this.Width;

            this.freeSize.Text = $"{Math.Floor(drive.AvailableFreeSpace / Math.Pow(1024, 3)):N0} GB";
        }
    }
}