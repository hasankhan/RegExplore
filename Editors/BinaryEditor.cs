using CrackSoft.RegExplore.Registry;
using Be.Windows.Forms;

namespace CrackSoft.RegExplore.Editors
{
    partial class BinaryEditor : ValueEditor
    {
        DynamicByteProvider byteProvider;

        public BinaryEditor(RegValue value): base(value)
        {
            InitializeComponent();
            byteProvider = new DynamicByteProvider((byte[])value.Data);
            txtData.ByteProvider = byteProvider;
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            SaveValue(byteProvider.Bytes.GetBytes());
        }
    }
}
