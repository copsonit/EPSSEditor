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
        //private static System.Timers.Timer midPlayerTimer = null;
        //private static System.Threading.Timer midPlayerTimer = null;
        //private static HPTimer midPlayerTimer = null;
        //private static AccurateTimer mTimer1;
        private static MidiTimer midiTimer = null;
        private static IMidiInstrument _midiInstrument = null;
        //private static List<long> times = new List<long>();
        //private static int showTimesFrom = 0;
        //private static int tickMs = 20;
        //private static int tickOrgStep = 16;
        //private static int tickStep = 16;
        private static long tickNum;
        private static long tickLen;
        private static long tickTime;

        private static long startTimeTick;

        //private static object updateLock = new object();

        //public static void StartPlaying(ref Timer timer)
        //{
            /*
            midPlayerTimer = timer;
            midPlayerTimer.Interval = 1; //ms
            midPlayerTimer.Start();
            */
        //}
    


        public static void StartPlaying(Form form)
        {
            //midPlayerTimer = new HPTimer(tickMs);
            //midPlayerTimer.Tick += doHpTick;
            startTimeTick = DateTime.Now.Ticks;

            midiTimer = new MidiTimer();
            midiTimer.Timer += new EventHandler(OnPulse);

            tickNum = 0;
            //long rate = 250000; // Number of us per tick
            long rate = 500000; // 500000 should be 120bpm.... 
            if (_midReader.timeSignature == null) rate *= 4;
            long ticksPerQuarter = _midReader.mf.DeltaTicksPerQuarterNote;
            double playbackSpeed = 1;
            tickLen = (long)((rate / (ticksPerQuarter * playbackSpeed)) * 10.0f);     //len of each tick in 0.1 usecs (or 100 nanosecs)
            tickTime = 0; // Cumulative tick time in 0.1usec.

            //midPlayerTimer = new System.Threading.Timer(UpdateCallback, null, tickMs, tickMs);

            //midPlayerTimer = new System.Timers.Timer();
            //midPlayerTimer.Interval = 20;
            //midPlayerTimer.Elapsed += Elapsed;
            //midPlayerTimer.AutoReset = true;
            //midPlayerTimer.Enabled = true;

            //midPlayerTimer.Tick += n
            //midPlayerTimer.Interval = 20;
            //midPlayerTimer.Start();

            //int delay = 20;
            //mTimer1 = new AccurateTimer(form, new Action(TimerTick1), delay);
            midiTimer.start(1);
        }

        private static void MidPlayerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static void LoadMidFile(string path)
        {
            StopPlaying();
            _midReader.Load(path);
        }


        public static void RegisterInstrument(IMidiInstrument i)
        {
            _midiInstrument = i;
        }


        public static void StopPlaying()
        {
            //if (midPlayerTimer != null) midPlayerTimer.Stop();
            //if (midPlayerTimer != null) { midPlayerTimer.Dispose(); }
            if (midiTimer != null) midiTimer.stop();

        }

        private static void TimerTick1()
        {
            //Tick();
        }

        private static void Elapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            //Tick();
        }


        private static void dotNetTick(object sender, EventArgs e) {
            //Tick();
        }

        public static void doHpTick(Object sender, TickEventArgs args) {
            //Tick();
            //TickStatic();
        }

        public static void UpdateCallback(object state)
        {
            //Tick();
        }


        private static void OnPulse(object sender, EventArgs e)
        {
            Tick();
        }

        public static void TickStatic()
        {
            int tickStep = 2;
            int n = 0;
            int trackTicks = _midReader.trackTicks[n];


            if (trackTicks <= 0)
            {
                long absTime = 0;
                int channel = 1;
                int note = 36;
                int duration = 96;
                MidiEvent midiEvent = new NoteOnEvent(absTime, channel, note, 127, duration);
                if (_midiInstrument != null) _midiInstrument.DoMidiEvent(midiEvent);
                trackTicks = 48;
            } else
            {
                trackTicks -= tickStep;
            }
            _midReader.trackTicks[n] = trackTicks;

        }

       

        public static void Tick()
        {
            {
                //int tickStep = 16;
                //long now = DateTime.Now.Ticks;
                //var watch = System.Diagnostics.Stopwatch.StartNew();



                long now = DateTime.Now.Ticks;
                long runningTime = (now - startTimeTick);
                //long absTimeTick = (DateTime.Now.Ticks - startTimeTick) / 10000; // ms

                bool midiFinished = false;
                while (runningTime > tickTime)
                {

                    for (int n = 0; n < _midReader.mf.Tracks; n++)
                    {
                        //int trackTicks = _midReader.trackTicks[n];
                        //if (trackTicks <= 0)
                        //{
                        int eventPointer = _midReader.eventPointers[n];
                        IList<MidiEvent> events = _midReader.mf.Events[n];

                        while (tickNum >= events[eventPointer].AbsoluteTime)
                        {
                            MidiEvent midiEvent = events[eventPointer];
                            if (_midiInstrument != null) _midiInstrument.DoMidiEvent(midiEvent);

                            eventPointer++;
                            _midReader.eventPointers[n] = eventPointer;
                            if (eventPointer >= events.Count)
                            {
                                midiFinished = true;
                                break;
                            }
                        }
                        if (midiFinished) { break; }

                    }
                    tickNum++;
                    tickTime = tickTime + tickLen;
                    if (midiFinished) { break; }
                }

                if (midiFinished) StopPlaying();
            }

        }


    }



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
