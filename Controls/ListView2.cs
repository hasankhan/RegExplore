using System.Windows.Forms;
using System.Reflection;

namespace CrackSoft.UI.Controls
{    
    class ListView2 : ListView
    {
        public ListView2()
        {
            //DoubleBuffer for smoother scroll and AllPaintingInWmPaint for filtering WM_ERASEBKGND
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //capture OnNotifyMessage for filtering messages
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //filter WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}
