﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsDesktop;

namespace dnVirtualDesktop
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CheckVersion();

            //Add Excluded windows
            ExcludedWindowCaptions.Add("ASUS_Check");
            ExcludedWindowCaptions.Add("NVIDIA GeForce Overlay");
            ExcludedWindowCaptions.Add("dnVirtualDesktop Settings");

            //Run the main form
            Application.Run(MainForm = new frmMain());

        }

        public static frmMain MainForm;
        public const string version = "1.0.20";

        public static IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();
        public static List<string> WallpaperStyles = new List<string>();
        public static List<string> PinnedApps = new List<string>();
        public static List<Window> windows = new List<Window>();
        public static List<HotkeyItem> hotkeys = new List<HotkeyItem>();
        public static VirtualDesktop[] Desktops = VirtualDesktop.GetDesktops();
        public static List<string> ExcludedWindowCaptions = new List<string>();
        

        public static string IconTheme = "White Box";

        //stats to log
        public static uint PinCount = 0;
        public static uint MoveCount = 0;
        public static uint NavigateCount = 0;

        public static void AddWindowToList(Window win)
        {
            IEnumerable<Window> window = from Window w in Program.windows
                                         where w.Handle == win.Handle
                                         select w;
            if (window.Count() < 1)
            {
                windows.Add(win);
            }
        }

        public static bool IsExcludedWindow(string caption)
        {
            foreach(string s in ExcludedWindowCaptions)
            {
                if(caption == s)
                {
                    return true;
                }
            }

            return false;
        }

        public static void CheckVersion()
        {
            /*
            try
            {
                MainForm.timerCheckVersion.Enabled = false;
            }
            catch { }
            
            string latestversion = GetCurrentVersion();
            if (latestversion != "" && latestversion != version)
            {
                DialogResult result = MessageBox.Show("dnVirtualDesktop " + latestversion + " is available.\r\n" +
                    "You are currently running version "+ version + "\r\n" + 
                    "Would you like to download it now?", "dnVirtualDesktop", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/mzomparelli/dnVirtualDesktop/blob/master/dnVirtualDesktop/bin/Release/dnVirtualDesktop.zip?raw=true");
                    Environment.Exit(0);
                }
            }
            try
            {
                MainForm.timerCheckVersion.Enabled = true;
            }
            catch { }*/
        }

        public static string GetCurrentVersion()
        {
            try
            {
                WSHttpBinding b = new WSHttpBinding(SecurityMode.Transport);
                b.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                b.Name = "WSHttpBinding_IService";
                EndpointAddress address = new EndpointAddress("https://zomp.co/z/ZompWebService.svc");

                using (Zomp.ServiceClient z = new Zomp.ServiceClient(b, address))
                {
                    return z.zVD_CurrentVersion();
                }


            }
            catch (Exception ex)
            {
                return "";
            }


        }

    }
}