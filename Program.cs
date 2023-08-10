using System.Diagnostics;

namespace MapDownloader
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            args = new string[] { "https://osu.ppy.sh/beatmaps/4068994" };
            //string appPath = Assembly.GetEntryAssembly().Location;

            //if (Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\MapDownloaderURL\shell\open\command").GetValue(null).ToString() != "\"" + appPath + "\" " + "\"" + "%1" + "\"")
            //{
            //    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\MapDownloaderURL\shell\open\command").SetValue(null, "\"" + appPath + "\" " + "\"" + "%1" + "\"");
            //}

            var adminStatus = RegistryManagement.IsAdministrator();

            if (adminStatus)
            {
                RegistryManagement.CreateRegistry();
                MessageBox.Show("To use this program, set your default browser as MapDownloader.exe and choose your default browser's executable after clicking OK.");
                BrowserManagement.SetBrowser();
                MessageBox.Show("And on this step, choose your osu! folder.");
                DbReader.SetOsuPath();
                return;
            }

            if (args.Length == 0 || adminStatus || !BrowserManagement.IsSet())
            {
                MessageBox.Show("Run the program as administrator for the initial setup.");
                return;
            }

            var browserPath = DbReader.OsuPathKey.GetValue("BrowserPath")?.ToString()!;

            foreach (string arg in args)
            {

                if (!LinkManagement.IsMapLink(arg))
                {
                    Process.Start(browserPath, arg);
                    return;
                }

                if (LinkManagement.GetSetId(arg) == null)
                {
                    MessageBox.Show("The map couldn't be found on chimu.moe.");
                    Process.Start(browserPath, arg);
                    return;
                }

                if (!DbReader.IsDownloaded(arg))
                {
                    DownloadManagement.DownloadMap(arg);

                    return;
                }
                Process.Start(browserPath, arg);
            }
        }
    }
}