using NAudio.MediaFoundation;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static EPSSEditor.HPTimer;

namespace EPSSEditor
{

    // Rewrite to this as time is unreliable:
    // https://github.com/kohoutech/Kohoutech.MIDI/blob/master/MIDI/Engine/Transport.cs

    // Sound synthesis:
    // https://github.com/sinshu/CSharpSynthProject/tree/master


    static class MidPlayer
    {
        private static EPSSMidFile _midReader = new EPSSMidFile();
        private static MidiTimer midiTimer = null;
        private static IMidiInstrument _midiInstrument = null;

        public static long tickNum;
        private static long midiTicksPerThreadTick;
        private static double timeBarrierFps;

        public static bool isPlaying = false;
        private static TimeBarrier _time;
        private static Thread _thread;

        //public static event EventHandler<MidFileEventArgs> MidiTickEvent;


        public static void InitPlaying()
        {
            tickNum = 0;
            //long rate = 250000; // Number of us per tick
            long rate = 500000; // 500000 should be 120bpm.... 
            if (_midReader.timeSignature == null) rate *= 4;
            long ticksPerQuarter = _midReader.mf.DeltaTicksPerQuarterNote;
            double playbackSpeed = 1;

            long tickLen = (long)((rate / (ticksPerQuarter * playbackSpeed)) * 10.0f);     //len of each tick in 0.1 usecs (or 100 nanosecs)
            double tickLenInS = (double)tickLen / 10000000;
            double fps = 60; // wanted fps around this.
            // Calculate best value of FPS when tickIncrement is even.
            double tickIncrement = (1 / tickLenInS) / fps;
            timeBarrierFps = (tickIncrement * fps) / (int)tickIncrement;
            midiTicksPerThreadTick = (int)tickIncrement;
        }

        public static void StartPlaying()
        {
            isPlaying = true;
            _time = new TimeBarrier(timeBarrierFps); // In FPS
            CreateThread();
        }


        private static void CreateThread()
        {
            _thread = new Thread(Tick) { Name = "EPSS Editor Midi Player Tick" };
            _thread.Start();
        }

        private static void WaitThread()
        {
            if (_thread != null && (_thread.ThreadState == System.Threading.ThreadState.Running || _thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _thread.Join();
            }
        }


        public static void LoadMidFile(string path)
        {
            StopPlaying();
            _midReader.Load(path);
            InitPlaying();
        }


        public static int Denominator()
        {
            if (_midReader != null)
            {

                if (_midReader.timeSignature != null)
                {
                    return _midReader.timeSignature.Denominator;
                }

            }
            return 2;
        }


        public static int Numerator()
        {
            if (_midReader != null)
            {
                if (_midReader.timeSignature != null)
                {
                    return _midReader.timeSignature.Numerator;
                }

            }
            return 4;
        }


        public static long Clocks()
        {
            if (_midReader != null)
            {

                if (_midReader.timeSignature != null)
                {
                    return _midReader.timeSignature.TicksInMetronomeClick;
                }

            }
            return 24;
        }



        public static long NumberOf32()
        {
            if (_midReader != null)
            {

                if (_midReader.timeSignature != null)
                {
                    return _midReader.timeSignature.No32ndNotesInQuarterNote;
                }

            }
            return 8;
        }


        public static long TicksPerQuarter()
        {
            if (_midReader != null)
            {
                return _midReader.mf.DeltaTicksPerQuarterNote;
            }
            return 192;
        }

        public static void RegisterInstrument(IMidiInstrument i)
        {
            _midiInstrument = i;
        }


        public static void StopPlaying()
        {
            if (isPlaying)
            {
                isPlaying = false;
                WaitThread();
            }
        }


        public static void Tick()
        {
            _time.Start();

            bool midiFinished = false;

            while (true)
            {
                for (int n = 0; n < _midReader.mf.Tracks; n++)
                {
                    //int trackTicks = _midReader.trackTicks[n];
                    //if (trackTicks <= 0)
                    //{
                    int eventPointer = _midReader.eventPointers[n];
                    IList<MidiEvent> events = _midReader.mf.Events[n];
                    MidiEvent midiEvent = events[eventPointer];
                    while (tickNum >= midiEvent.AbsoluteTime)
                    {
                        if (_midiInstrument != null) _midiInstrument.DoMidiEvent(midiEvent);

                        eventPointer++;
                        _midReader.eventPointers[n] = eventPointer;
                        if (eventPointer >= events.Count)
                        {
                            midiFinished = true;
                            break;
                        }
                        midiEvent = events[eventPointer];
                    }
                    if (midiFinished) { break; }

                }
                tickNum += midiTicksPerThreadTick;

                if (midiFinished) { break; }
                if (!isPlaying) { break; }

                _time.Wait();
            }

            _time.Stop();
            isPlaying = false;
        }
     }


    public class MidFileEventArgs : EventArgs
    {
        public long tick { get; private set; }

        public MidFileEventArgs(long tick)
        {
            this.tick = tick;
        }
    }


    /*
    class AccurateTimer
    {
        private delegate void TimerEventDel(int id, int msg, IntPtr user, int dw1, int dw2);
        private const int TIME_PERIODIC = 1;
        private const int EVENT_TYPE = TIME_PERIODIC;// + 0x100;  // TIME_KILL_SYNCHRONOUS causes a hang ?!
        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventDel handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);

        Action mAction;
        Form mForm;
        private int mTimerId;
        private TimerEventDel mHandler;  // NOTE: declare at class scope so garbage collector doesn't release it!!!

        public AccurateTimer(Form form, Action action, int delay)
        {
            mAction = action;
            mForm = form;
            timeBeginPeriod(1);
            mHandler = new TimerEventDel(TimerCallback);
            mTimerId = timeSetEvent(delay, 0, mHandler, IntPtr.Zero, EVENT_TYPE);
        }

        public void Stop()
        {
            int err = timeKillEvent(mTimerId);
            timeEndPeriod(1);
            System.Threading.Thread.Sleep(100);// Ensure callbacks are drained
        }

        private void TimerCallback(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (mTimerId != 0)
                mForm.BeginInvoke(mAction);
        }
    
    }
    */


    public class EPSSMidFile
    {
        public MidiFile mf;
        public string path;
        public TimeSignatureEvent timeSignature;

        public Dictionary<int, int> eventPointers;
        public Dictionary<int, int> trackTicks;
        public Dictionary<int, int> absoluteTicks;

        public void Load(string fileName)
        {
            path = fileName;
            var strictMode = false;
            mf = new MidiFile(path, strictMode);
            timeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
            if (timeSignature != null)
            {
                Console.WriteLine("{0} {1}\r\n", ToMBT(timeSignature.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), timeSignature);
            } else
            {
                Console.WriteLine("No TimeSignatureEvent found in MID file.");
            }

            eventPointers = new Dictionary<int, int>();
            trackTicks = new Dictionary<int, int>();
            absoluteTicks = new Dictionary<int, int>();
            for (int n = 0; n < mf.Tracks; n++)
            {
                eventPointers.Add(n, 0);
                trackTicks.Add(n, 0);
                absoluteTicks.Add(n, 0);
            }
        }

        public string ToMBT(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
        {
            int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
            int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
            int ticksPerBeat = ticksPerBar / beatsPerBar;
            long bar = 1 + (eventTime / ticksPerBar);
            long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
            long tick = eventTime % ticksPerBeat;
            return String.Format("{0}:{1}:{2}", bar, beat, tick);
        }

    }
    



}
