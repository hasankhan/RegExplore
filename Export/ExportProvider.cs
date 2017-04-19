using Microsoft.Win32;
using System.IO;

namespace CrackSoft.RegExplore.Export
{
    enum RegExportFormat
    {
        NativeRegFormat,
        XmlFormat,
        TextFormat
    }

    abstract class ExportProvider
    {
        protected TextWriter Writer { get; private set; }

        protected ExportProvider(TextWriter writer)
        {
            Writer = writer;
        }

        abstract public void BeginExport();
        abstract public void WriteKeyStart(string key);
        abstract public void WriteKeyEnd();
        abstract public void WriteValue(string name, RegistryValueKind kind, object data);
        abstract public void EndExport();

        public static ExportProvider Create(RegExportFormat format, StreamWriter writer)
        {            
            ExportProvider provider = null;
            switch (format)
            {
                case RegExportFormat.NativeRegFormat:
                    provider = new NativeExportProvider(writer);
                    break;
                case RegExportFormat.XmlFormat:
                    provider = new XmlExportProvider(writer);
                    break;
                default:
                    provider = new TextExportProvider(writer);
                    break;
            }
            return provider;
        }
    }
}
