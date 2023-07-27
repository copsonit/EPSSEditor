using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{
    public delegate EPSSEditorData GetEPSSEditorDataCallBack();
    public class SpiSoundInstrument : MidiInstrument
    {
        //public EPSSEditorData data;

        private readonly Dictionary<int, CachedSound[]> playingContext;
        private readonly byte[] currentProgram;

        private readonly AudioPlaybackEngine audio;

        private readonly int newFreq;
        //private int testTone;

        //private CachedSound cachedCachedSound;

        private readonly GetEPSSEditorDataCallBack _getEditorDataCallBack;

        public event EventHandler<SpiSoundInstrumentEventArgs> NoteOnEvent;
        public event EventHandler<SpiSoundInstrumentEventArgs> NoteOffEvent;


        public SpiSoundInstrument() { }


        public void Init()
        {
            if (_getEditorDataCallBack != null)
            {
                EPSSEditorData data = _getEditorDataCallBack();
                foreach (SpiSound spi in data.spiSounds)
                {
                    data.EnsureCachedSound(spi, newFreq);
                }
            }
        }

        public SpiSoundInstrument(GetEPSSEditorDataCallBack callback, AudioPlaybackEngine audio, int newFreq)
        {
            playingContext = new Dictionary<int, CachedSound[]>();
            for (int channel = 0; channel < 16; channel++)
            {
                playingContext.Add(channel, new CachedSound[128]);
            }
            //this.data = data;
            this.audio = audio;
            this.newFreq = newFreq;
            _getEditorDataCallBack = callback;
            currentProgram = new byte[16];
            for (int channel = 0; channel < 16; channel++) currentProgram[channel] = 128;

            /*
            SpiSound snd = data.FindSpiSound(1, 1);
            cachedCachedSound = data.cachedSound(snd, newFreq);
            testTone = 1;
            */
            //Sound snd = data.sounds[0];
            //cachedCachedSound = snd.cachedSound();
        }

        public override void NoteOn(int midiChannel, int note, int vel)
        {
            //Console.WriteLine($"Spi got Note on: {midiChannel} {note} {vel}");

            /*
            if (midiChannel == 1 && note == 36)
            {
                cachedCachedSound.pitch = Math.Pow(2, (double)(testTone-1) / 12.0);
                Console.WriteLine($"{cachedCachedSound.pitch}");
                PlaySound(cachedCachedSound, midiChannel, testTone++);
            }
            */
            if (_getEditorDataCallBack != null)
            {
                EPSSEditorData data = _getEditorDataCallBack();

                byte program = currentProgram[midiChannel - 1];
                SpiSound snd = program < 128 ? data.FindSpiSound(128, program, note) : data.FindSpiSound(midiChannel, program, note);
                //                if (program < 128)
                //{

                //SpiSound snd = data.FindSpiSound(midiChannel, program, note);
                if (snd != null)
                {
                    //Console.WriteLine($"Found sound: {snd.name()}");
                    CachedSound cs = data.CachedSound(snd, newFreq, note, vel);

                    PlaySound(cs, midiChannel, note);
                    DoNoteOnEvent(midiChannel, note);
                }
                else
                {
                    Console.WriteLine($"!!!! No sound found for Midi:{midiChannel} Note: {note}");
                }
                //                } else
                //{
                //Console.WriteLine($"Need to find sound for ProgramChange {program} on channel {midiChannel} and note {note}.");
                //
            }
        }

        protected void DoNoteOnEvent(int midiChannel, int note)
        {
            if (NoteOnEvent != null)
            {
                NoteOnEvent(this, new SpiSoundInstrumentEventArgs(midiChannel, note));
            }
        }


        public override void NoteOff(int midiChannel, int note)
        {
            //Console.WriteLine($"Spi got Note off: {midiChannel} {note}");

            CachedSound[] channelMap = playingContext[midiChannel - 1];
            CachedSound oldSnd = channelMap[note];
            if (oldSnd != null)
            {
                //Console.WriteLine("Stopping: {0}", oldSnd.WaveFormat.ToString()); /*audio.StopSound(oldSnd); */
                audio.StopSound(oldSnd);
                channelMap[note] = null;
                DoNoteOffEvent(midiChannel, note);
            }
            playingContext[midiChannel - 1] = channelMap;
        }


        protected void DoNoteOffEvent(int midiChannel, int note)
        {
            if (NoteOffEvent != null)
            {
                NoteOffEvent(this, new SpiSoundInstrumentEventArgs(midiChannel, note));
            }
        }


        public override void ProgramChange(int channel, int programChange)
        {
            currentProgram[channel - 1] = (byte)programChange;
        }


        private void PlaySound(CachedSound snd, int midiChannel, int note)
        {
            CachedSound[] channelMap = playingContext[midiChannel - 1];
            CachedSound oldSnd = channelMap[note];
            if (oldSnd != null)
            {
                //Console.WriteLine("Stopping: {0}", oldSnd.WaveFormat.ToString()) ; /*audio.StopSound(oldSnd); */
                audio.StopSound(oldSnd);
            }

            audio.PlaySound(snd);
            //Console.WriteLine("Starting: {0}", snd.WaveFormat.ToString());
            channelMap[note] = snd;
            playingContext[midiChannel - 1] = channelMap;
        }


        public void AllNotesOff()
        {
            for (int midiChannel = 0; midiChannel < 16; midiChannel++)
            {
                CachedSound[] channelMap = playingContext[midiChannel];
                for (int i = 0; i < 128; i++)
                {
                    CachedSound snd = channelMap[i];
                    if (snd != null)
                    {
                        audio.StopSound(snd);
                        channelMap[i] = null;
                    }
                }
                playingContext[midiChannel] = channelMap;
            }
        }
    }


    public class SpiSoundInstrumentEventArgs : EventArgs
    {
        public int midiChannel { get; private set; }
        public int note { get; private set; }

        public SpiSoundInstrumentEventArgs(int midiChannel, int note)
        {
            this.midiChannel = midiChannel;
            this.note = note;
        }
    }
}
