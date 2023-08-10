using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MapDownloader
{
    internal partial class LinkManagement
    {
        public static string GetId(string link)
        {
            return GetIdRegex().Match(link).Value;
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
        //        var match = Regex.Match(response.RequestMessage.RequestUri.ToString(), @"[0-9]+");
        //        return match.Success ? match.Value : null;
        //    }
        //}

        public static async Task<string> GetSetId(string url)
        {
            const string search = "https://osu.ppy.sh/beatmapsets/";
            using var client = new HttpClient();
            using var message = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await client.SendAsync(message);
            var statusCode = (int)response.StatusCode;

            if (statusCode < 200 || statusCode >= 400)
                return null;

            var responseUrl = response.RequestMessage.RequestUri.ToString();
            var startIndex = responseUrl.IndexOf(search, StringComparison.Ordinal) + search.Length;
            var length = responseUrl.IndexOf("#", StringComparison.Ordinal) - search.Length;
            return responseUrl.Substring(startIndex, length);
        }

        public static async Task<string?> GetFileName(string link)
        {
            try
            {
                using var httpClient = new HttpClient();
                var chimuResponse = await httpClient.GetStringAsync("https://api.chimu.moe/v1/set/" + GetSetId(link));

                var m = JsonConvert.DeserializeObject<ChimuSetJSON>(chimuResponse);

                return Filter($"{m.SetId} {m.Artist_Unicode} - {m.Title_Unicode}.osz");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public static async Task<string?> GetFileHash(string link)
        {
            var id = GetId(link);

            try
            {
                using var httpClient = new HttpClient();
                var chimuResponse = await httpClient.GetStringAsync("https://api.chimu.moe/v1/map/" + id);

                var m = JsonConvert.DeserializeObject<ChimuDiffJSON>(chimuResponse);

                return m.FileMD5;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        [GeneratedRegex("[0-9]+")]
        private static partial Regex GetIdRegex();
        [GeneratedRegex("[<>\":/\\\\|?*]")]
        private static partial Regex FilterRegex();
        [GeneratedRegex("^(https?:\\/\\/osu\\.ppy\\.sh\\/beatmaps\\/[A-Za-z0-9]*)$")]
        private static partial Regex IsMapLinkRegex();
    }
    class ChimuDiffJSON
    {
        public string ParentSetId { get; set; }
        public string FileMD5 { get; set; }
        public string error_code { get; set; }
    }

    class ChimuSetJSON
    {
        public string SetId { get; set; }
        public string Title_Unicode { get; set; }
        public string Artist_Unicode { get; set; }
    }
}
