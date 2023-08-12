using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MapDownloader
{
    internal class DownloadCheck
    {
        public static readonly RegistryKey OsuPathKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MapDownloader");

        public static void SetOsuPath()
        {
            Thread thread = new Thread(() =>
            {
                using FolderBrowserDialog fbd = new();
                fbd.Description = "Choose your osu! folder.";
                string osuPath;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    osuPath = fbd.SelectedPath;
                    OsuPathKey.SetValue("osuPath", osuPath);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

        }

        public static bool IsDownloaded(string url)
        {
            string rootPath = OsuPathKey.GetValue("osuPath")!.ToString()! + @"\Songs";
            string[] dirs = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);
            string dirsString = string.Concat(dirs);

            return dirsString.Contains(Program.setId!);
        }
    }
}
