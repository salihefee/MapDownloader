﻿using Microsoft.Win32;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace MapDownloader
{
    internal class BrowserManagement
    {
        public static void SetBrowser()
        {
            Thread thread = new Thread(() =>
            {
                using var ofd = new OpenFileDialog();
                ofd.Filter = "Exe Files (.exe)|*.exe";
                ofd.Title = "Choose your default browser's executable.";
                string BrowserPath;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    BrowserPath = ofd.FileName;
                    RegistryKey browserPathKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MapDownloader");
                    browserPathKey.SetValue("BrowserPath", BrowserPath);
                }
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
