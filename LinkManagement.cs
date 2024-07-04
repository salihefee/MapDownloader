using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MapDownloader
{
    internal static partial class LinkManagement
    {
        public static string GetId(string link)
        {
            return IsNumberRegex().Match(link).Value;
        }

        public static string Filter(string str)
        {
            return FilterRegex().Replace(str, "");
        }

        public static bool IsMapLink(string link)
        {
            return IsMapLinkRegex().IsMatch(link);
        }

        //public static async Task<string> GetSetId(string link)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, link));
        //        if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 400)
        //            return null;
        //var match = Regex.Match(response.RequestMessage.RequestUri.ToString(), @"[0-9]+");
        //        return match.Success? match.Value : null;
        //    }
        //}

        public static async Task<string?> GetSetId(string url)
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
            };
            using var client = new HttpClient(handler);
            using var message = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await client.SendAsync(message);
            var statusCode = (int)response.StatusCode;

            if (statusCode < 200 || statusCode >= 400)
                return null;

            var match = IsNumberRegex().Match(response.Headers.Location!.ToString());
            return match.Success ? match.Value : null;
        }

        // public static async Task<string?> GetFileName(string link)
        // {
        //     try
        //     {
        //         using var httpClient = new HttpClient();
        //         var url = "https://catboy.best/api/v2/s/" + Program.setId;
        //         var catboyResponse = await httpClient.GetStringAsync(url);
        //
        //         var m = JsonConvert.DeserializeObject<catboySetJSON>(catboyResponse);
        //
        //         return Filter($"{Program.setId} {m!.artist} - {m.title}.osz");
        //     }
        //     catch (HttpRequestException)
        //     {
        //         return null;
        //     }
        // }

        //public static async Task<string?> GetFileHash(string link)
        //{
        //    var id = GetId(link);

        //    try
        //    {
        //        using var httpClient = new HttpClient();
        //        var catboyResponse = await httpClient.GetStringAsync("https://api.chimu.moe/v1/map/" + id);

        //        var m = JsonConvert.DeserializeObject<catboyDiffJSON>(catboyResponse);

        //        return m!.FileMD5;
        //    }
        //    catch (HttpRequestException)
        //    {
        //        return null;
        //    }
        //}

        [GeneratedRegex("[0-9]+")]
        private static partial Regex IsNumberRegex();
        [GeneratedRegex("[<,>\":/\\\\|?*]")]
        private static partial Regex FilterRegex();
        [GeneratedRegex(@"^(https?:\/\/osu\.ppy\.sh\/beatmaps\/[A-Za-z0-9]*)$")]
        private static partial Regex IsMapLinkRegex();
    }

    // internal class catboyDiffJSON
    // {
    //     public string? ParentSetId { get; set; }
    //     public string? FileMD5 { get; set; }
    //     public string? error_code { get; set; }
    // }

    internal class catboySetJSON
    {
        public string? title { get; set; }
        public string? artist { get; set; }
    }
}
