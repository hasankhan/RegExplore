using System;
using System.Windows.Forms;

namespace CrackSoft.RegExplore
{
    class BusyCursor: IDisposable
    {
        Form source;

        public BusyCursor(Form source)
        {
            this.source = source;
            source.Cursor = Cursors.WaitCursor;
        }

        #region IDisposable Members

        public void Dispose()
        {
            source.Cursor = Cursors.Default;
        }

        #endregion
    }
}
