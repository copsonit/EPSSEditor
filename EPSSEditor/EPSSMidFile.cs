using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{
    public class EPSSMidFile
    {
        public MidiFile mf;

        public Dictionary<int, int> eventPointers;
        public Dictionary<int, int> trackTicks;

        public void Load(string fileName)
        {
            var strictMode = false;
            mf = new MidiFile(fileName, strictMode);
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
