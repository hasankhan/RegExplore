using System.Windows.Forms;

namespace CrackSoft.RegExplore
{
    static class UIUtility
    {
        public static void InformUser(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void DisplayError(IWin32Window owner, string message)
        {
            DisplayError(owner, message, null);
        }

        public static void DisplayError(IWin32Window owner, string message, Control source)
        {
            MessageBox.Show(owner, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (source != null)
            {
                if (source is TextBox)
                    ((TextBox)source).SelectAll();
                source.Focus();
            }
        }

        public static void WarnUser(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        public static bool ConfirmAction(IWin32Window owner, string message, string action, bool critical)
        {
            MessageBoxIcon icon = critical ? MessageBoxIcon.Warning : MessageBoxIcon.Question;
            return MessageBox.Show(owner, message, "Confirm " + action, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }
    }
}
