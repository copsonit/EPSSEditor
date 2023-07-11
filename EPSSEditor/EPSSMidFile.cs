using NAudio.MediaFoundation;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPSSEditor
{
    static class MidPlayer
    {
        private static EPSSMidFile _midReader = new EPSSMidFile();
        private static Timer midPlayerTimer = null;

        public static void StartPlaying(ref Timer timer)
        {
            midPlayerTimer = timer;
            midPlayerTimer.Interval = 10; //ms
            //midPlayerTimer.Tag = _midReader;
            midPlayerTimer.Start();

        }


        public static void LoadMidFile(string path)
        {
            StopPlaying();
            _midReader.Load(path);
        }


        public static void StopPlaying()
        {
            if (midPlayerTimer != null) midPlayerTimer.Stop();
        }

        public static void Tick()
        {
            for (int n = 0; n < _midReader.mf.Tracks; n++)
            {
                int trackTicks = _midReader.trackTicks[n];
                if (trackTicks <= 0)
                {
                    int eventPointer = _midReader.eventPointers[n];
                    IList<MidiEvent> events = _midReader.mf.Events[n];

                    while (trackTicks <= 0)
                    {
                        MidiEvent midiEvent = events[eventPointer++];
                        trackTicks = midiEvent.DeltaTime;
                        if (!MidiEvent.IsNoteOff(midiEvent))
                        {
                            Console.WriteLine("Midi event: {0}", midiEvent);
                        }
                    }

                    _midReader.eventPointers[n] = eventPointer;
                }
                else
                {
                    trackTicks -= 5; // TODO tempo calc
                }
                _midReader.trackTicks[n] = trackTicks;
            }
        }
    }



    public class EPSSMidFile
    {
        public MidiFile mf;
        public string path;

        public Dictionary<int, int> eventPointers;
        public Dictionary<int, int> trackTicks;

        public void Load(string fileName)
        {
            path = fileName;
            var strictMode = false;
            mf = new MidiFile(path, strictMode);
            var timeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();

            eventPointers = new Dictionary<int, int>();
            trackTicks = new Dictionary<int, int>();
            for (int n = 0; n < mf.Tracks; n++)
            {
                eventPointers.Add(n, 0);
                trackTicks.Add(n, 0);
            }
        }


        private string ToMBT(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
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
