using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPSSEditor
{
    internal class ControlScrollListener : NativeWindow, IDisposable
    {
        public event ControlScrolledEventHandler ControlScrolled;
        public delegate void ControlScrolledEventHandler(object sender, EventArgs e, int delta, Point pos);

        private const uint WM_HSCROLL = 0x114;
        private const uint WM_VSCROLL = 0x115;
        private const uint WM_MOUSEWHEEL = 0x020A;
        private readonly Control _control;

        public ControlScrollListener(Control control)
        {
            _control = control;
            AssignHandle(control.Handle);
        }

        protected bool Disposed { get; set; }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                // Free other managed objects that implement IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null
            ReleaseHandle();

            Disposed = true;
        }


        protected override void WndProc(ref Message m)
        {
            HandleControlScrollMessages(m);
            base.WndProc(ref m);
        }


        private void HandleControlScrollMessages(Message m)
        {
            //if (m.Msg == WM_HSCROLL | m.Msg == WM_VSCROLL)
            if (m.Msg == WM_MOUSEWHEEL)
            {
                if (ControlScrolled != null)
                {
                    int delta = GET_WHEEL_DELTA_WPARAM(m.WParam);
                    Point pos = new Point(m.LParam.ToInt32());
                    //Console.WriteLine(delta);
                    //Console.WriteLine(pos);
                    ControlScrolled(_control, new EventArgs(), delta, pos);
                }
            }
        }


        internal static ushort HIWORD(IntPtr dwValue)
        {
            return (ushort)((((long)dwValue) >> 0x10) & 0xffff);
        }


        internal static ushort HIWORD(uint dwValue)
        {
            return (ushort)(dwValue >> 0x10);
        }


        internal static int GET_WHEEL_DELTA_WPARAM(uint wParam)
        {
            return (short)HIWORD(wParam);
        }


        internal static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam)
        {
            return (short)HIWORD(wParam);
        }
    }
}
