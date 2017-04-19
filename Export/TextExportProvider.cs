using System.IO;
using CrackSoft.RegExplore.Registry;
using Microsoft.Win32;
using System.Collections.Generic;

namespace CrackSoft.RegExplore.Export
{
    class TextExportProvider: ExportProvider
    {
        Stack<int> counters;
        int counter;

        public TextExportProvider(TextWriter writer) : base(writer) 
        {
            counters = new Stack<int>();
            counter = 1;
        }

        public override void BeginExport() { }

        public override void WriteKeyStart(string key)
        {
            Writer.WriteLine("Key Name:\t{0}", key);            
            counters.Push(counter);
            counter = 1;
        }

        public override void WriteKeyEnd()
        {
            counter = counters.Pop();
            Writer.WriteLine();
        }

        public override void WriteValue(string name, RegistryValueKind kind, object data)
        {
            Writer.WriteLine("Value {0}", counter++);
            Writer.WriteLine("    Name:\t{0}", name);
            Writer.WriteLine("    Type:\t{0}", kind.ToDataType());
            Writer.WriteLine("    Data:\t{0}", RegValue.ToString(kind, data));
            Writer.WriteLine();
        }

        public override void EndExport()
        {
            Writer.WriteLine();
        }
    }
}
