using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrackSoft.RegExplore.Export;
using CrackSoft.RegExplore.Registry;
using System.IO;

namespace CrackSoft.RegExplore
{
    partial class ExportDialog : Form
    {
        public ExportDialog()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            RegKey key;
            if ((key = RegKey.Parse(cmbBranch.Text)) == null)
            {
                UIUtility.DisplayError(this, Properties.Resources.Error_InvalidKey, cmbBranch);
                return;
            }

            RegExportFormat format = GetExportFormat();


            Stream output;
            try
            {
                output = File.OpenWrite(txtFile.Text);
            }
            catch
            {
                UIUtility.DisplayError(this, Properties.Resources.Error_FileOpenFail, txtFile);
                return;
            }

            bool success = ExportToFile(key, format, output);

            if (!success)
                UIUtility.DisplayError(this, Properties.Resources.Error_ExportFail);
            else
            {
                UIUtility.InformUser(this, Properties.Resources.Info_ExportSuccess);
                this.Close();
            }
        }

        private bool ExportToFile(RegKey key, RegExportFormat format, Stream output)
        {
            bool success = true;
            using (output)
            {
                DisableControls();
                try
                {
                    using (new BusyCursor(this))
                    {
                        using (StreamWriter writer = new StreamWriter(output))
                        {
                            RegExporter.Export(key, ExportProvider.Create(format, writer));
                        }
                    }
                }
                catch
                {
                    success = false;
                }
                EnableControls();
            }
            return success;
        }

        private RegExportFormat GetExportFormat()
        {
            RegExportFormat format;
            if (saveFileDialog1.FilterIndex == 1)
                format = RegExportFormat.NativeRegFormat;
            else if (saveFileDialog1.FilterIndex == 2)
                format = RegExportFormat.XmlFormat;
            else
                format = RegExportFormat.TextFormat;
            return format;
        }

        private void EnableControls()
        {
            btnBrowse.Enabled =
                btnExport.Enabled =
                btnCancel.Enabled =
                cmbBranch.Enabled = true;
        }

        private void DisableControls()
        {
            btnBrowse.Enabled =
                btnExport.Enabled =
                btnCancel.Enabled =
                cmbBranch.Enabled = false;
        }

        private void cmbBranch_TextChanged(object sender, EventArgs e)
        {
             SetExportButtonState();
        }

        private void SetExportButtonState()
        {
            btnExport.Enabled = (cmbBranch.Text != String.Empty && txtFile.Text != String.Empty);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                txtFile.Text = saveFileDialog1.FileName;
            }
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            SetExportButtonState();
        }
    }
}
