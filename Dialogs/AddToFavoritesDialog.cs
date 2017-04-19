using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrackSoft.RegExplore
{
    partial class AddToFavoritesDialog : Form
    {
        public AddToFavoritesDialog()
        {
            InitializeComponent();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (txtName.TextLength > 0);
        }
    }
}
