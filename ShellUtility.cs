using System;
using Microsoft.Win32; // GetDefaultBrowser/RegistryKey

namespace CrackSoft.Utility
{
    static class ShellUtility
    {        
        static string GetDefaultBrowser()
        {
            string keyPath = @"htmlfile\shell\open\command";
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(keyPath);
            string browserPath = key.GetValue(String.Empty).ToString();
            browserPath = browserPath.Split('\"')[1];
            return browserPath;
        }

        public static void OpenWebPage(string url)
        {
            System.Diagnostics.Process.Start(GetDefaultBrowser(), url);
        }
    }
}
