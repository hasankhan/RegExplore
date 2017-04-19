using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrackSoft.RegExplore.Registry;

namespace CrackSoft.RegExplore.Editors
{
    partial class DWordEditor : ValueEditor
    {
        public DWordEditor(RegValue value): base(value)
        {
            InitializeComponent();
            string data;
            if (value.Kind == Microsoft.Win32.RegistryValueKind.DWord)
                data = ((int)value.Data).ToString("x");
            else
                data = ((long)value.Data).ToString("x");
        }

        private void base_CheckedChanged(object sender, EventArgs e)
        {            
            txtData.HexNumber = rdoHex.Checked;            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Value.Kind == Microsoft.Win32.RegistryValueKind.DWord)
                SaveValue((int)txtData.UIntValue);
            else
                SaveValue((long)txtData.ULongValue);
        }
    }
}
