using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{
    interface IMidiInstrument
    {
        void DoMidiEvent(MidiEvent evnt);
    }


    public class MidiInstrument : IMidiInstrument
    {
        public void DoMidiEvent(MidiEvent midiEvent)
        {
            if (MidiEvent.IsNoteOn(midiEvent))
            {
                if (((NoteOnEvent)midiEvent).Velocity == 0)
                {
                    NoteOff(midiEvent.Channel, ((NoteOnEvent)midiEvent).NoteNumber);
                }
                else
                {
                    NoteOn(midiEvent.Channel, ((NoteOnEvent)midiEvent).NoteNumber, ((NoteOnEvent)midiEvent).Velocity);
                }
            }
            else if (MidiEvent.IsNoteOff(midiEvent))
            {
                NoteOff(midiEvent.Channel, ((NoteEvent)midiEvent).NoteNumber);
            }
            else if (midiEvent is PatchChangeEvent pe)
            {
                ProgramChange(pe.Channel+1, pe.Patch);
            }



        }


        virtual public void NoteOn(int channel, int note, int velocity)
        {

        }
        virtual public void NoteOff(int channel, int note)
        {

        }

        virtual public void ProgramChange(int channel, int programChange)
        {

        }
    }
}
