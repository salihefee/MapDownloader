using System.Diagnostics;

namespace MapDownloader
{
    internal class Program
    {
        public static string? setId;

        [STAThread]
        static async Task Main(string[] args)
        {
            // Used for debug purposes
            //args = new string[] { "https://osu.ppy.sh/beatmaps/3814127" };

            var browserPath = DownloadCheck.OsuPathKey.GetValue("BrowserPath")?.ToString()!;

            if (Process.GetProcessesByName("osu!").Length != 0)
            {
                var adminStatus = RegistryManagement.IsAdministrator();

                if (adminStatus)
                {
                    RegistryManagement.CreateRegistry();
                    MessageBox.Show("To use this program, set your default browser as MapDownloader.exe and choose your current default browser's executable after clicking OK.");
                    BrowserManagement.SetBrowser();
                    MessageBox.Show("And on this step, choose your osu! folder.");
                    DownloadCheck.SetOsuPath();
                    return;
                }

                if (args.Length == 0 || adminStatus || !BrowserManagement.IsSet())
                {
                    MessageBox.Show("Run the program as administrator for the initial setup.");
                    return;
                }

                foreach (string arg in args)
                {
                    if (!LinkManagement.IsMapLink(arg))
                    {
                        Process.Start(browserPath, arg);
                        return;
                    }

                    setId = await LinkManagement.GetSetId(arg);

                    if (setId == null)
                    {
                        Process.Start(browserPath, arg);
                        MessageBox.Show("The map couldn't be found on chimu.moe.");
                        return;
                    }

                    if (!DownloadCheck.IsDownloaded(arg))
                    {
                        await DownloadManagement.DownloadMap(arg);
                        return;
                    }

                    Process.Start(browserPath, arg);
                    return;
                }
            }
            foreach (string arg in args)
            {
                Process.Start(browserPath, arg);
            }
        }
    }
}