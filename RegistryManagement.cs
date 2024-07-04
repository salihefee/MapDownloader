using System.Security.Principal;
using Microsoft.Win32;

namespace MapDownloader
{
    internal static class RegistryManagement
    {
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void CreateRegistry()
        {
            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\MapDownloader");

            using var capabilities = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\MapDownloader\Capabilities");
            using var urlAssociations = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\MapDownloader\Capabilities\URLAssociations");
            using var registeredApps = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\RegisteredApplications");
            using var MapDownloaderURL = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\MapDownloaderURL");

            capabilities.SetValue("ApplicationDescription", "MapDownloader");
            capabilities.SetValue("ApplicationName", "MapDownloader");

            urlAssociations.SetValue("http", "MapDownloaderURL");
            urlAssociations.SetValue("https", "MapDownloaderURL");

            registeredApps.SetValue("MapDownloader", @"SOFTWARE\MapDownloader\Capabilities");

            MapDownloaderURL.SetValue(null, "MapDownloader Document");
            MapDownloaderURL.SetValue("FriendlyTypeName", "MapDownloader Document");

            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\MapDownloaderURL\shell");
            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\MapDownloaderURL\shell\open");

            var appPath = AppContext.BaseDirectory + @"\MapDownloader.exe";

            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\MapDownloaderURL\shell\open\command").SetValue(null, "\"" + appPath + "\" " + "\"" + "%1" + "\"");
        }

        public static bool Exists()
        {
            return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\MapDownloader\Capabilities", ".http", null) != null;
        }
    }
}
