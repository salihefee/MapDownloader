using System.Net;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace MapDownloader
{
    internal class DownloadManagement
    {
        public static async Task DownloadMap(string link)
        {
            var setId = Program.setId;
            var fileName = await LinkManagement.GetFileName(link);

            try
            {
                await DownloadFileAsync(setId!, fileName!);

                Process.Start("explorer.exe", @"C:\Windows\Temp\" + fileName);
            }

            catch (HttpRequestException)
            {
                Process.Start(DownloadCheck.OsuPathKey.GetValue("BrowserPath")!.ToString()!, link);
                Environment.Exit(0);
            }
        }

        public static async Task DownloadFileAsync(string setId, string fileName)
        {
            using var httpClient = new HttpClient();
            var chimuUrl = "https://chimu.moe/d/" + setId;
            var nerinyanUrl = "https://api.nerinyan.moe/d/" + setId;
            var filePath = @"C:\Windows\Temp\" + fileName;

            var response = await httpClient.GetAsync(chimuUrl, HttpCompletionOption.ResponseHeadersRead);

            await using var streamToReadFrom = await response.Content.ReadAsStreamAsync();
            await using var streamToWriteTo = File.Open(filePath, FileMode.Create);
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
        }
    }
}