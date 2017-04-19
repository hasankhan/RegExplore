using System;
using Microsoft.Win32;
using System.Collections.Generic;

namespace CrackSoft.RegExplore.Registry
{
    static class Extensions
    {
        public static object GetDefaultData(this RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.Binary:
                    return new byte[0];
                case RegistryValueKind.DWord:
                    return 0;
                case RegistryValueKind.ExpandString:
                    return String.Empty;
                case RegistryValueKind.MultiString:
                    return new string[0];
                case RegistryValueKind.QWord:
                    return (long)0;
                case RegistryValueKind.String:
                    return String.Empty;
                case RegistryValueKind.Unknown:
                default:
                    return null;
            }
        }

        public static string ToDataType(this RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.Binary:
                    return "REG_BINARY";
                case RegistryValueKind.DWord:
                    return "REG_DWORD";
                case RegistryValueKind.ExpandString:
                    return "REG_EXPAND_SZ";
                case RegistryValueKind.MultiString:
                    return "REG_MULTI_SZ";
                case RegistryValueKind.QWord:
                    return "REG_QWORD";
                case RegistryValueKind.String:
                    return "REG_SZ";
                case RegistryValueKind.Unknown:
                    return "REG_UNKNOWN";
                default:
                    return String.Empty;
            }
        }

        public static string ToHiveName(this RegistryHive regHive)
        {
            switch (regHive)
            {
                case RegistryHive.ClassesRoot:
                    return "HKEY_CLASSES_ROOT";
                case RegistryHive.CurrentConfig:
                    return "HKEY_CURRENT_CONFIG";
                case RegistryHive.CurrentUser:
                    return "HKEY_CURRENT_USER";
                case RegistryHive.DynData:
                    return "HKEY_DYN_DATA";
                case RegistryHive.LocalMachine:
                    return "HKEY_LOCAL_MACHINE";
                case RegistryHive.PerformanceData:
                    return "HKEY_PERFORMANCE_DATA";
                case RegistryHive.Users:
                    return "HKEY_USERS";
                default:
                    return String.Empty;
            }
        }

        public static RegistryKey ToKey(this RegistryHive regHive)
        {
            switch (regHive)
            {
                case RegistryHive.ClassesRoot:
                    return Microsoft.Win32.Registry.ClassesRoot;
                case RegistryHive.CurrentConfig:
                    return Microsoft.Win32.Registry.CurrentConfig;
                case RegistryHive.CurrentUser:
                    return Microsoft.Win32.Registry.CurrentUser;
                case RegistryHive.DynData:
                    return Microsoft.Win32.Registry.DynData;
                case RegistryHive.LocalMachine:
                    return Microsoft.Win32.Registry.LocalMachine;
                case RegistryHive.PerformanceData:
                    return Microsoft.Win32.Registry.PerformanceData;
                case RegistryHive.Users:
                    return Microsoft.Win32.Registry.Users;
                default:
                    return null;
            }
        }
    }
}