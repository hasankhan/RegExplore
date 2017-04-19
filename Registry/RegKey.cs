using System;
using System.Linq;
using Microsoft.Win32;

namespace CrackSoft.RegExplore.Registry
{
    class RegKey : IComparable<RegKey>
    {
        public string Name { get; private set; }
        public RegistryKey Key { get; private set; }
        public string Parent { get; private set; }

        public RegKey(string name, RegistryKey key)
        {            
            Name = name;
            Key = key;
            int index = key.Name.Length-name.Length-1;
            if (index > 0)
                Parent = key.Name.Substring(0, index);
        }

        public RegKey(RegistryKey key):
            this(key.Name.Contains('\\') ? key.Name.Substring(key.Name.LastIndexOf('\\')): key.Name, key) {  }

        public static RegKey Parse(string keyPath)
        {
            return Parse(keyPath, false);
        }

        public static RegKey Parse(string keyPath, bool writable)
        {
            string[] tokens = keyPath.Split(new char[]{'\\'}, 2);
            RegistryKey rootKey = RegUtility.ParseRootKey(tokens[0]);
            if (tokens.Length == 1)
                return new RegKey(rootKey);
            string path = tokens[1];
            string name = keyPath.Substring(keyPath.LastIndexOf('\\') + 1);
            try
            {
                var key = rootKey.OpenSubKey(path, writable);
                if (key == null)
                    return null;
                return new RegKey(name, key);
            }
            catch
            {
                return null;
            }
            
        }

        /* Implemented the IComparable interface
         * because Sort method of List<T> calls the CompareTo function
         * of the objects to compare them and I want the objects
         * of RegKey type to be sorted on basis of key name */
        #region IComparable<RegKey> Members

        public int CompareTo(RegKey other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion
    }
}
