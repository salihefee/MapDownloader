using Microsoft.Win32;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace MapDownloader
{
    internal static class BrowserManagement
    {
        public static void SetBrowser()
        {
            var thread = new Thread(() =>
            {
                using var ofd = new OpenFileDialog();
                ofd.Filter = "Exe Files (.exe)|*.exe";
                ofd.Title = "Choose your default browser's executable.";
                string BrowserPath;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                BrowserPath = ofd.FileName;
                var browserPathKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MapDownloader");
                browserPathKey.SetValue("BrowserPath", BrowserPath);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

        }
        public static bool IsSet()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\MapDownloader", "BrowserPath", null) != null;
        }
    }
}
