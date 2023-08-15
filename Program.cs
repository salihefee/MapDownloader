using System.Diagnostics;
using System.IO;
using System.Net.Http;
using HttpClientProgress;
using Newtonsoft.Json;

namespace MapDownloader
{
    internal class Program
    {
        public static string? setId;

        [STAThread]
        static async Task Main(string[] args)
        {
            // Used for debug purposes
            //args = new string[] { "https://osu.ppy.sh/beatmaps/2123199" };

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

            var browserPath = DownloadCheck.OsuPathKey.GetValue("BrowserPath")?.ToString()!;

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
                    MessageBox.Show("The map couldn't be found on chimu.moe.");
                    Process.Start(browserPath, arg);
                    return;
                }

                if (!DownloadCheck.IsDownloaded(arg))
                {
                    async Task DownloadFile(string setId)
                    {
                        using var client = new HttpClient();
                        var chimuUrl = "https://chimu.moe/d/" + setId;
                        string fileName;

                        try
                        {
                            var url = "https://api.chimu.moe/v1/set/" + setId;
                            var chimuResponse = await client.GetStringAsync(url);

                            var m = JsonConvert.DeserializeObject<ChimuSetJSON>(chimuResponse);
                            fileName = LinkManagement.Filter($"{m!.SetId} {m.Artist_Unicode} - {m.Title_Unicode}.osz");
                        }
                        catch (HttpRequestException)
                        {
                            Process.Start(browserPath, arg);
                            return;
                        }

                        var filePath = Path.Combine(Path.GetTempPath(), fileName);

                        var progress = new Progress<float>();
                        progress.ProgressChanged += Progress_ProgressChanged!;

                        using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await client.DownloadDataAsync(chimuUrl, file, progress);
                        }
                        Process.Start("explorer.exe", filePath);
                    }

                    void Progress_ProgressChanged(object sender, float progress)
                    {
                        // Do something with your progress
                        Console.WriteLine(progress);
                    }

                    await DownloadFile(setId);

                    return;
                }

                Process.Start(browserPath, arg);
                return;
            }
        }
    }
}