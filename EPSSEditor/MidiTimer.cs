using NAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{
    public class MidiTimer
    {
        //timer callback delegate
        delegate void TimeProc(uint uID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);

        //p/invoke imports
        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint timeSetEvent(uint uDelay, uint uResolution, TimeProc lpTimeProc, UIntPtr dwUser, uint fuEvent);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern int timeKillEvent(uint uTimerID);

        //const vals from mmsystem.h
        const int TIME_ONESHOT = 0;
        const int TIME_PERIODIC = 1;
        const int TIME_CALLBACK_FUNCTION = 0;
        const int TIME_CALLBACK_EVENT_SET = 16;
        const int TIME_CALLBACK_EVENT_PULSE = 32;

        //-----------------------------------------------------------------------------

        uint id = 0;                    //timer id
        TimeProc timerProc;             //timer callback func        

        public MidiTimer()
        {
            timerProc = timerCallback;          //set callback func
        }

        public void start(uint msec)
        {
            stop();         //stop prev timer

            id = timeSetEvent(msec, 0, timerProc, UIntPtr.Zero, (uint)(TIME_CALLBACK_FUNCTION | TIME_PERIODIC));
            if (id == 0)
                throw new Exception("timeSetEvent error");
            Console.WriteLine("started timer: {0} ", id.ToString());
        }

        public void stop()
        {
            if (id != 0)
            {
                timeKillEvent(id);
                Console.WriteLine("stopped timer: {0}", id.ToString());
                id = 0;
            }
        }

        // timer event & callback func ------------------------------------------------

        public event EventHandler Timer;

        protected virtual void OnTimer(EventArgs e)
        {
            if (Timer != null)
                Timer(this, e);
        }

        void timerCallback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2)
        {
            OnTimer(new EventArgs());
        }
    }

}
