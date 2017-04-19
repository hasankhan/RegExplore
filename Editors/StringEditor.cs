using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using CrackSoft.RegExplore.Registry;

namespace CrackSoft.RegExplore.Editors
{
    partial class StringEditor : ValueEditor
    {
        public StringEditor(RegValue value): base(value)
        {
            InitializeComponent();
            txtData.Text = value.Data.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveValue(txtData.Text);
        }
    }
}
