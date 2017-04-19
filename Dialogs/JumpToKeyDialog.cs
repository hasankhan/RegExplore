using System;
using System.Windows.Forms;

namespace CrackSoft.RegExplore
{
    partial class JumpToKeyDialog : Form
    {
        public JumpToKeyDialog()
        {
            InitializeComponent();
        }

        private void JumpToKeyDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                CancelButton.PerformClick();
        }

        private void txtKeyPath_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = txtKeyPath.Text.Length > 0;
        }
    }
}
