using System.Diagnostics;

namespace MapDownloader
{
    internal static class Program
    {
        public static string? setId;

        [STAThread]
        // ReSharper disable once RedundantAssignment
        static async Task Main(string[] args)
        {
            // Used for debug purposes
            // args = new [] { "https://osu.ppy.sh/beatmaps/1640908" };

            var browserPath = DownloadCheck.OsuPathKey.GetValue("BrowserPath")?.ToString()!;

            switch (args.Length)
            {
                case 0:
                {
                    var adminStatus = RegistryManagement.IsAdministrator();

                    if (adminStatus)
                    {
                        RegistryManagement.CreateRegistry();
                        MessageBox.Show(
                            "To use this program, set your default browser as MapDownloader.exe and choose your current default browser's executable after clicking OK.");
                        BrowserManagement.SetBrowser();
                        MessageBox.Show("And on this step, choose your osu! folder.");
                        DownloadCheck.SetOsuPath();
                        return;
                    }

                    // I'm aware that the args.Length check has already been done, but I don't know how removing that would affect the result of the logic
                    if (args.Length == 0 || adminStatus || !BrowserManagement.IsSet())
                    {
                        MessageBox.Show("Run the program as administrator for the initial setup.");
                    }

                    break;
                }
                case > 0:
                {
                    var link = args[0];
                    
                    if (!LinkManagement.IsMapLink(link))
                    {
                        Process.Start(browserPath, string.Join(" ", args));
                        return;
                    }

                    setId = await LinkManagement.GetSetId(link);

                    if (setId == null)
                    {
                        Process.Start(browserPath, link);
                        MessageBox.Show("The map couldn't be found on catboy.best.");
                        return;
                    }

                    if (!DownloadCheck.IsDownloaded(setId))
                    {
                        await DownloadManagement.DownloadMap(link);
                        return;
                    }

                    Process.Start(browserPath, link);

                    break;
                }
            }
        }
    }
}