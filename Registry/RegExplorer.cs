using System.Collections.Generic;
using Microsoft.Win32;

namespace CrackSoft.RegExplore.Registry
{
    static class RegExplorer
    {
        public const string RegistryFavoritePath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit\Favorites";
        public static List<RegKey> GetSubKeys(RegistryKey key)
        {
            int subKeyCount = key.SubKeyCount;
            if (subKeyCount == 0)
                return new List<RegKey>();

            List<RegKey> subKeys = new List<RegKey>(subKeyCount);            

            string[] subKeyNames = key.GetSubKeyNames();
            for (int i=0; i<subKeyNames.Length; i++)
                try
                {
                    string keyName = subKeyNames[i];
                    RegKey item = new RegKey(keyName, key.OpenSubKey(keyName));
                    subKeys.Add(item);
                }
                catch { }

            return subKeys;
        }

        public static List<RegValue> GetValues(RegistryKey key)
        {
            int valueCount = key.ValueCount;
            if (valueCount == 0)
                return new List<RegValue>();

            List<RegValue> values = new List<RegValue>(valueCount);
            string[] valueNames = key.GetValueNames();
            for (int i = 0; i < valueNames.Length; i++) 
                values.Add(new RegValue(key, valueNames[i]));

            return values;
        }
    }
}