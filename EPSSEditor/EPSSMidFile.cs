using NAudio.MediaFoundation;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EPSSEditor.HPTimer;

namespace EPSSEditor
{
    static class MidPlayer
    {
        private static EPSSMidFile _midReader = new EPSSMidFile();
        //private static Timer midPlayerTimer = null;
        private static HPTimer midPlayerTimer = null;
        private static IMidiInstrument _midiInstrument = null;
        private static List<long> times = new List<long>();
        private static int showTimesFrom = 0;


        public static void StartPlaying(ref Timer timer)
        {
            /*
            midPlayerTimer = timer;
            midPlayerTimer.Interval = 1; //ms
            midPlayerTimer.Start();
            */

        }


        public static void StartPlaying()
        {
            midPlayerTimer = new HPTimer(20);
            midPlayerTimer.Tick += doHpTick;
            //midPlayerTimer = new Timer();
            //midPlayerTimer.Tick += new System.EventHandler(dotNetTick);
            //midPlayerTimer.Interval = 20;
            //midPlayerTimer.Start();
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
            if (midPlayerTimer != null) { midPlayerTimer.Dispose(); }

        }


        private static void dotNetTick(object sender, EventArgs e) {
            Tick();
        }

        public static void doHpTick(Object sender, TickEventArgs args) {
            Tick();
            //TickStatic();
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
            int tickStep = 8;
            //long now = DateTime.Now.Ticks;
            var watch = System.Diagnostics.Stopwatch.StartNew();
    
            for (int n = 0; n < _midReader.mf.Tracks; n++)
            {
                int trackTicks = _midReader.trackTicks[n];
                if (trackTicks <= 0)
                {
                    int eventPointer = _midReader.eventPointers[n];
                    IList<MidiEvent> events = _midReader.mf.Events[n];

                    //                    while (trackTicks <= 0)
                    //{
                    do
                    {
                        if (trackTicks <= 0)
                        {
                            MidiEvent midiEvent = events[eventPointer];

                            if (_midiInstrument != null) _midiInstrument.DoMidiEvent(midiEvent);

                            //if (!MidiEvent.IsNoteOff(midiEvent))
                                                            Console.WriteLine("{0} Delta:{1} MyAbs: {2} {3}\r\n", _midReader.ToMBT(midiEvent.AbsoluteTime, _midReader.mf.DeltaTicksPerQuarterNote, _midReader.timeSignature), midiEvent.DeltaTime, _midReader.absoluteTicks[n], midiEvent);
                            StringBuilder sb = new StringBuilder();
                            sb.Append($"From {showTimesFrom} ");
                            for (int i = showTimesFrom; i < times.Count; i++) {
                                sb.Append(times[i].ToString());
                                if (i < times.Count) sb.Append(" ");
                                showTimesFrom = i;
                            }
                            sb.Append($" To {showTimesFrom}");
                            Console.WriteLine(sb.ToString());
                                // Console.WriteLine("Midi event: {0}", midiEvent);
                            //}

                            eventPointer++;

                            MidiEvent nextMidiEvent = events[eventPointer];
                            trackTicks = nextMidiEvent.DeltaTime;
                        }
                    } while (trackTicks <= 0);
                    _midReader.eventPointers[n] = eventPointer;

                }                
                else
                {
                    trackTicks -= tickStep; // TODO tempo calc
                    int abs = _midReader.absoluteTicks[n];
                    abs += tickStep;
                    _midReader.absoluteTicks[n] = abs;

                }
                _midReader.trackTicks[n] = trackTicks;
            }
            times.Add(watch.ElapsedMilliseconds);
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
