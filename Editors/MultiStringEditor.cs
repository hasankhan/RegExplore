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
    partial class MultiStringEditor : ValueEditor
    {
        public MultiStringEditor(RegValue value): base(value)
        {
            InitializeComponent();
            txtData.Text = String.Join("\r\n",((string[])value.Data));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveValue(txtData.Text.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
