using System.Net;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace MapDownloader
{
    internal static class DownloadManagement
    {
        public static async Task DownloadMap(string link)
        {
            var setId = Program.setId;

            try
            {
                await DownloadFileAsync(setId!);
            }

            catch (HttpRequestException)
            {
                Process.Start(DownloadCheck.OsuPathKey.GetValue("BrowserPath")!.ToString()!, link);
                Environment.Exit(0);
            }
        }

        public static async Task DownloadFileAsync(string setId)
        {
            using var httpClient = new HttpClient();
            var catboyUrl = "https://catboy.best/d/" + setId;
            
            var response = await httpClient.GetAsync(catboyUrl, HttpCompletionOption.ResponseHeadersRead);

            var cd = response.Content.Headers.ContentDisposition!.ToString();

            var fileName = new ContentDisposition(cd).FileName;
            
            var filePath = @"C:\Windows\Temp\" + fileName;

            await using var streamToReadFrom = await response.Content.ReadAsStreamAsync();
            await using var streamToWriteTo = File.Open(filePath, FileMode.Create);
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
            
            Process.Start("explorer.exe", filePath);
        }
    }
}