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

        public static CoreWebView2Environment? GlobalWebView2Environment { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            Mutex = new Mutex(true, "17cf5b95-4261-4254-8978-d61580c3b057", out bool launcherNotOpenAlready);
            bool launcherOpenAlready = !launcherNotOpenAlready;
            Args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            if (launcherOpenAlready)
            {
                using var client = new NamedPipeClientStream(".", "8d9d7d24-97d9-4efc-abcc-ccd09f3480bd", PipeDirection.Out);
                client.Connect(3000);
                using var writer = new StreamWriter(client);
                writer.WriteLine("SHOW_WINDOW");
                writer.Flush();
                return;
            }

            base.OnStartup(e);

            string parentDirectory = Directory.GetParent(Directory.GetParent(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath)!.FullName)!.FullName;
            GlobalWebView2Environment = await CoreWebView2Environment.CreateAsync(null, Path.Combine(parentDirectory, "temp"));

            StartServer();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private static void StartServer()
        {
            Task.Run(() =>
            {
                using var server = new NamedPipeServerStream("8d9d7d24-97d9-4efc-abcc-ccd09f3480bd", PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

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