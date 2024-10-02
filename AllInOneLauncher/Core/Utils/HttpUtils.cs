using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllInOneLauncher.Core.Utils
{
    internal static class HttpUtils
    {
        private static HttpClient HttpClient = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan };

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

        public static async Task Download(string url, string localPath, Action<int>? OnProgressUpdate = null)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    using (var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                        var totalBytesRead = 0L;
                        var buffer = new byte[4096];
                        var isMoreToRead = true;
                        int progressInPercent = 0;

                        using (var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                        using (var stream = await response.Content.ReadAsStreamAsync())
                            do
                            {
                                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                                if (bytesRead == 0)
                                {
                                    isMoreToRead = false;
                                    continue;
                                }

                                await fileStream.WriteAsync(buffer, 0, bytesRead);

                                totalBytesRead += bytesRead;
                                int newProgressInPercent = (int)(totalBytesRead * 100 / totalBytes);

                                if (progressInPercent != newProgressInPercent)
                                {
                                    progressInPercent = newProgressInPercent;
                                    OnProgressUpdate?.Invoke(newProgressInPercent);
                                }
                            }
                            while (isMoreToRead);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (i == 2) throw new HttpRequestException($"Error while downloading from {url}\n{ex.ToString()}");
                }
            }
        }
    }
}
