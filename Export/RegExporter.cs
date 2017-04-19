using System.IO;
using CrackSoft.RegExplore.Registry;

namespace CrackSoft.RegExplore.Export
{
    class RegExporter
    {        
        public static void Export(RegKey key, ExportProvider provider)
        {
            provider.BeginExport();
            ExportKey(key, provider);
            provider.EndExport();
        }

        static void ExportKey(RegKey key, ExportProvider provider)
        {
            provider.WriteKeyStart(key.Key.Name);
            ExportValues(key, provider);
            foreach (RegKey subKey in RegExplorer.GetSubKeys(key.Key))
                ExportKey(subKey, provider);
            provider.WriteKeyEnd();
        }

        static void ExportValues(RegKey key, ExportProvider provider)
        {
            foreach (RegValue value in RegExplorer.GetValues(key.Key))
                provider.WriteValue(value.Name, value.Kind, value.Data);
        }
    }
}
