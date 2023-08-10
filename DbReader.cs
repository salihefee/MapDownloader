using Microsoft.Win32;
using System.IO;
using System.Text;

namespace MapDownloader
{
    internal class DbReader
    {
        public static readonly RegistryKey OsuPathKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MapDownloader");

        public static bool IsDownloaded(string link)
        {
            var dbPath = OsuPathKey.GetValue("osuPath").ToString();
            var osuDb = File.ReadAllBytes(dbPath + @"\osu!.db");
            var osuDbStr = Encoding.UTF8.GetString(osuDb);
            var fileHash = LinkManagement.GetFileHash(link);
            return osuDbStr.Contains(fileHash + "\v");
        }

        public static void SetOsuPath()
        {
            using FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Choose your osu! folder.";
            string osuPath;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                osuPath = fbd.SelectedPath;
                OsuPathKey.SetValue("osuPath", osuPath);
            }
        }
    }
}
