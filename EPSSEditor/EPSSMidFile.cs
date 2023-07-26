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
using System.Text.RegularExpressions;
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
        //private static MidiTimer midiTimer = null;
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
            _midReader.Init();
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


        public static bool LoadMidFile(string path)
        {
            StopPlaying();
            bool result = _midReader.Load(path);
            InitPlaying();
            return result;
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


        public static void SpoolTick(int step)
        {
            tickNum = Math.Max(0, tickNum + midiTicksPerThreadTick * step);
            tickNum = Math.Min(_midReader.lastMidiTick, tickNum);

            for (int n = 0; n < _midReader.mf.Tracks; n++)
            {
                int eventPointer = _midReader.eventPointers[n];
                IList<MidiEvent> events = _midReader.mf.Events[n];

                if (step < 0)
                {
                    while (eventPointer >= 0)
                    {
                        MidiEvent midiEvent = events[eventPointer];
                        if (midiEvent.AbsoluteTime <= tickNum) break;
                        eventPointer--;
                    }
                } else
                {
                    while (eventPointer < (events.Count - 1))
                    {
                        MidiEvent midiEvent = events[eventPointer];
                        if (tickNum < midiEvent.AbsoluteTime) break;
                        if (eventPointer < (events.Count-1)) eventPointer++;
                    }
                }
                _midReader.eventPointers[n] = eventPointer;
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
                    while (tickNum >= midiEvent.AbsoluteTime && eventPointer < (events.Count-1))
                    {
                        if (_midiInstrument != null) _midiInstrument.DoMidiEvent(midiEvent);

                        if (eventPointer < (events.Count - 1)) eventPointer++;
                        //if (eventPointer >= events.Count)
                        if (tickNum >= _midReader.lastMidiTick)
                        {                          
                            midiFinished = true;
                            break;
                        }
                        _midReader.eventPointers[n] = eventPointer;
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


     public class EPSSMidFile
    {
        public MidiFile mf;
        public string path;
        public TimeSignatureEvent timeSignature;
        public long lastMidiTick;

        public Dictionary<int, int> eventPointers;
        public Dictionary<int, int> trackTicks;
        public Dictionary<int, int> absoluteTicks;

        public bool Load(string fileName)
        {
            path = fileName;
            try
            {
                var strictMode = false;
                mf = new MidiFile(path, strictMode);
            }catch (Exception e)
            {
                Console.WriteLine("Error in loading MID file:{0}", e.ToString());
                return false;
            }
            timeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
            if (timeSignature != null)
            {
                Console.WriteLine("{0} {1}\r\n", ToMBT(timeSignature.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), timeSignature);
            } else
            {
                Console.WriteLine("No TimeSignatureEvent found in MID file.");
            }

            lastMidiTick = 0;
            for (int n = 0; n < mf.Tracks; n++)
            {
                IList<MidiEvent> events = mf.Events[n];
                MidiEvent lastEvent = events[events.Count - 1];
                lastMidiTick = Math.Max(lastEvent.AbsoluteTime, lastMidiTick);
            }

            return true;
        }


        public void Init()
        {
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
