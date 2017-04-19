using System.Windows.Forms;
using CrackSoft.RegExplore.Registry;
using Microsoft.Win32;

namespace CrackSoft.RegExplore.Editors
{    
    partial class ValueEditor : Form
    {
        protected RegValue Value { get; private set; }

        private ValueEditor()
        {
            InitializeComponent();
        }

        public ValueEditor(RegValue value):this()
        {            
            Value = value;
            txtName.Text = value.Name;
            txtName.Modified = false;
        }

        protected void SaveValue(object data)
        {
            Microsoft.Win32.Registry.SetValue(Value.ParentKey.Name, Value.Name, data, Value.Kind);
        }
    }
}
