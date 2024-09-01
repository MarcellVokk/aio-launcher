using System;
using System.Linq;
using System.Windows;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using System.Configuration;

namespace AllInOneLauncher
{
    public partial class App : Application
    {
        internal static Mutex? Mutex;
        internal static string[] Args = [];
        private string _mutexName;
        private string _pipeName;

        public App()
        {
            _mutexName = ConfigurationManager.AppSettings["mutexName"] ?? 
                         throw new ArgumentNullException($"mutexName needs to be specified inside the appsettings");
            _pipeName = ConfigurationManager.AppSettings["pipeName"] ?? 
                        throw new ArgumentNullException($"pipeName needs to be specified inside the appsettings");
        }

        public static CoreWebView2Environment? GlobalWebView2Environment { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            Mutex = new Mutex(true, _mutexName, out bool launcherNotOpenAlready);
            bool launcherOpenAlready = !launcherNotOpenAlready;
            Args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            if (launcherOpenAlready)
            {
                using var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
                client.Connect(3000);
                using var writer = new StreamWriter(client);
                writer.WriteLine("SHOW_WINDOW");
                writer.Flush();
                return;
            }

            base.OnStartup(e);

            string parentDirectory = Directory.GetParent(Directory.GetParent(ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath)!.FullName)!.FullName;
            GlobalWebView2Environment = await CoreWebView2Environment.CreateAsync(
                null, 
                Path.Combine(parentDirectory, "temp"));

            StartServer();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void StartServer()
        {
            Task.Run(() =>
            {
                using var server = new NamedPipeServerStream(
                    _pipeName, 
                    PipeDirection.In, 
                    1, 
                    PipeTransmissionMode.Byte, 
                    PipeOptions.Asynchronous);

                while (true)
                {
                    server.WaitForConnection();
                    using var reader = new StreamReader(server);
                    var message = reader.ReadLine();
                    if (message == "SHOW_WINDOW")
                    {
                        Current.Dispatcher.Invoke(() =>
                        {
                            var mainWindow = Current.MainWindow;
                            if (mainWindow != null)
                            {
                                if (mainWindow.WindowState == WindowState.Minimized)
                                {
                                    mainWindow.WindowState = WindowState.Normal;
                                }
                                mainWindow.Activate();
                                mainWindow.Topmost = true;
                                mainWindow.Topmost = false;
                                mainWindow.Focus();
                            }
                        });
                    }

                    server.Disconnect();
                }
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Mutex is not null)
            {
                if (Mutex.WaitOne(TimeSpan.Zero, true))
                    Mutex.ReleaseMutex();

                Mutex.Dispose();
            }

            base.OnExit(e);
        }

        public static void ExitImmediately()
        {
            Mutex?.Dispose();
            Mutex = null;
            Environment.Exit(0);
        }
    }
}