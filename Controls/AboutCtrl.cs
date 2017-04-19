using System;
using System.Windows.Forms;

namespace CrackSoft.UI.Controls
{
    public partial class AboutCtrl : UserControl
    {
        const string COPYRIGHT_TEXT = "Copyright (C) {0} Overrot Inc";
        int copyrightSince = DateTime.Now.Year;
        bool beta;

        public AboutCtrl()
        {
            InitializeComponent();            
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            lblAppName.Text = GetAppName();
            lblVersion.Text = GetAppVersion();
            lblCopyright.Text = GetCopyright();
        }

        string GetCopyright()
        {
            return String.Format(COPYRIGHT_TEXT, copyrightSince);
        }

        string GetAppName()
        {
            return ProductName;
        }

        string GetAppVersion()
        {
            return "Version " + (Beta ? ProductVersion  + " (BETA)" : ProductVersion);
        }

        public int CopyrightSince
        {
            get { return copyrightSince; }
            set
            {
                copyrightSince = value;
                lblCopyright.Text = GetCopyright();
            }
        }

        public bool Beta
        {
            get { return beta; }
            set
            {
                beta = value;
                lblVersion.Text = GetAppVersion();
            }
        }

        private void AboutCtrl_AutoSizeChanged(object sender, EventArgs e)
        {
            AdjustSize();
        }

        private void AdjustSize()
        {
            if (AutoSize)
            {
                int max = 0;
                foreach (Control control in Controls)
                {
                    if (control.Width > max)
                        max = control.Width;
                }
                Width = max;
                Height = lblRights.Top + lblRights.Height;
            }
        }
    }
}
