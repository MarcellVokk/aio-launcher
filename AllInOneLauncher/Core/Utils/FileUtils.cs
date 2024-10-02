using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AllInOneLauncher.Core.Utils
{
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
