using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace MapDownloader
{
    internal static class DownloadCheck
    {
        public static readonly RegistryKey OsuPathKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MapDownloader");

        public static void SetOsuPath()
        {
            var thread = new Thread(() =>
            {
                using FolderBrowserDialog fbd = new();
                fbd.Description = "Choose your osu! folder.";
                if (fbd.ShowDialog() != DialogResult.OK) return;
                var osuPath = fbd.SelectedPath;
                OsuPathKey.SetValue("osuPath", osuPath);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

        }

        public static bool IsDownloaded(string setId)
        {
            var rootPath = OsuPathKey.GetValue("osuPath") + @"\Songs";
            var dirs = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);

            foreach (var folder in dirs)
            {
                if (Regex.Match(folder, @"\b" + setId + @"\b").Success) return true;
            }

            return false;
        }
    }
}
