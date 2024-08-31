using AllInOneLauncher.Elements;
using AllInOneLauncher.Popups;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AllInOneLauncher.Logic
{
    public static class LauncherUpdateManager
    {
        public static async void CheckForUpdates()
        {
#if DEBUG
            return;
#endif

#pragma warning disable CS0162
            string applicationPath = Environment.ProcessPath ?? "";
#pragma warning restore CS0162

            string curentVersionHash = await Task.Run(() => FileUtils.GetFileMd5Hash(applicationPath));
            string latestVersionHash = await HttpUtils.Get("applications/versionHash", new Dictionary<string, string>() { { "name", "all-in-one-launcher" }, { "version", "main" }, });

            if (curentVersionHash == latestVersionHash)
            {
                if (applicationPath.Contains("_new.exe"))
                {
                    File.Move(applicationPath, applicationPath.Replace("_new.exe", ".exe"), true);
                    LauncherStateManager.Restart();
                }
                else if (File.Exists(applicationPath.Replace(".exe", "_new.exe")))
                {
                    File.Delete(applicationPath.Replace(".exe", "_new.exe"));
                }
                return;
            }

            try
            {
                LauncherUpdatePopup updatePopup = new();
                PopupVisualizer.ShowPopup(updatePopup);

                await HttpUtils.Download($"https://bfmeladder.com/api/applications/build?id=all-in-one-launcher-main", applicationPath.Replace(".exe", "_new.exe"), (progress) => updatePopup.LoadProgress = progress);
                LauncherStateManager.Restart(true);
            }
            catch (Exception ex)
            {
                PopupVisualizer.ShowPopup(new ErrorPopup(ex));
            }
        }
    }

    public static class HttpUtils
    {
        private static readonly HttpClient HttpClient = new() { Timeout = Timeout.InfiniteTimeSpan };

        public static async Task<string> Get(string apiEndpointPath, Dictionary<string, string>? requestParameters = null)
        {
            NameValueCollection requestQueryParameters = System.Web.HttpUtility.ParseQueryString(string.Empty);

            requestParameters?.ToList().ForEach(x => requestQueryParameters.Add(x.Key, x.Value == "" ? "~" : x.Value));

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://bfmeladder.com/api/{apiEndpointPath}{(requestQueryParameters.Count > 0 ? $"?{requestQueryParameters}" : "")}");
            requestMessage.Headers.Add("AuthAccountUuid", "");
            requestMessage.Headers.Add("AuthAccountPassword", "");

            HttpResponseMessage response = await HttpClient.SendAsync(requestMessage);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task Download(string url, string localPath, Action<int> OnProgressUpdate)
        {
            using var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var totalBytesRead = 0L;
            var buffer = new byte[4096];
            var isMoreToRead = true;
            int progressInPercent = 0;

            using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            using var stream = await response.Content.ReadAsStreamAsync();
            do
            {
                var bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0)
                {
                    isMoreToRead = false;
                    continue;
                }

                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));

                totalBytesRead += bytesRead;
                int newProgressInPercent = (int)(totalBytesRead * 100 / totalBytes);

                if (progressInPercent != newProgressInPercent)
                {
                    progressInPercent = newProgressInPercent;
                    OnProgressUpdate.Invoke(newProgressInPercent);
                }
            }
            while (isMoreToRead);
        }
    }

    public static class FileUtils
    {
        public static string GetFileMd5Hash(string path)
        {
            using var md5 = MD5.Create();
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}