using System;
using System.IO;
using Microsoft.Win32;
using System.Text;

namespace CrackSoft.RegExplore.Export
{
    class NativeExportProvider: ExportProvider
    {
        public NativeExportProvider(TextWriter writer) : base(writer) { }

        public override void BeginExport()
        {
            Writer.WriteLine("Windows Registry Editor Version 5.00");
        }

        public override void WriteKeyStart(string key)
        {
            Writer.WriteLine();
            Writer.WriteLine(String.Format("[{0}]", key));
        }

        public override void WriteKeyEnd() { }

        public override void WriteValue(string name, RegistryValueKind kind, object data)
        {
            string dataString;
            switch (kind)
            {
                case RegistryValueKind.Binary:
                    dataString = String.Format("hex:{0}", GetHexString((byte[])data));
                    break;
                case RegistryValueKind.DWord:
                    dataString = String.Format("dword:{0:x8}", (UInt32)((Int32)data));
                    break;
                case RegistryValueKind.QWord:
                    dataString = String.Format("qword:{0:x16}", (UInt64)((Int64)data));
                    break;
                case RegistryValueKind.ExpandString:
                    dataString = String.Format("hex(2):{0}", GetHexString((string)data));
                    break;
                case RegistryValueKind.MultiString:
                    dataString = String.Format("hex(7):{0}", GetHexString((string[])data));
                    break;
                case RegistryValueKind.String:
                    dataString = String.Format("\"{0}\"", (string)data);
                    break;
                case RegistryValueKind.Unknown:
                default:
                    dataString = String.Empty;
                    break;
            }
            Writer.WriteLine("\"{0}\"={1}", name, dataString);
        }

        private string GetHexString(string[] data)
        {
            if (data.Length == 0)
                return String.Empty;
            StringBuilder output = new StringBuilder(data.Length * 10);
            Array.ForEach<string>(data, str => output.Append(GetHexString(str)).Append(','));
            output.Append("00,00");
            return output.ToString();
        }

        private string GetHexString(string data)
        {
            if (data.Length == 0)
                return String.Empty;
            /* length is calculated as follows
             * 4 char for each character (two 2 digit hex nums)
             * 1 char seperator ','
             * 5 char postfix (null termination 00,00)
             * 1 char string terminator */
            int length = (data.Length * 4) + data.Length * 2 + 5 + 1;
            StringBuilder output = new StringBuilder(length);
            const string format = "{0:x2},{1:x2},";
            Array.ForEach<char>(data.ToCharArray(), chr => output.Append(String.Format(format, (byte)chr, (byte)chr >> 8)));
            output.Append("00,00");
            return output.ToString(0, output.Length);
        }

        private string GetHexString(byte[] data)
        {
            if (data.Length == 0)
                return String.Empty;
            //------slow method----------
            //string[] output = Array.ConvertAll<byte, string>(data, byt => String.Format("{0:x2}", byt));
            //return String.Join(",", output);

            //-------fast method---------
            /* length is calculated as follows
             * 2 char for each byte
             * 1 char seperator ','
             * 1 char string terminator */
            int length = (data.Length * 2) + data.Length + 1;
            StringBuilder output = new StringBuilder(length);
            const string format = "{0:x2},";
            Array.ForEach<byte>(data, byt => output.Append(String.Format(format, byt)));
            return output.ToString(0, output.Length - 1);
        }

        public override void EndExport()
        {
            Writer.WriteLine();
        }
    }
}
