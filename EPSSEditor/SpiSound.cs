using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EPSSEditor
{
  
    public class SpiSound
    {
        public byte midiChannel;
        public byte midiNote;
        public int soundNumber;

        public Guid soundId;
        public string _name;
        public string _extName;

        public SpiSound() { }

        public SpiSound(ref Sound sound)
        {
            soundId = sound.id();
            _name = sound.name();
            _extName = sound.name() + " MSWav"; // TODO, find more info in sound??
        }

        public string name() { return _name; }

        public string extName() { return _extName; }

        public string description(ref EPSSEditorData data)
        {
            StringBuilder s = new StringBuilder();
            s.Append(midiChannel);
            s.Append("    ");
            s.Append(midiNote);
            s.Append("    ");
            int i = data.getSoundNumberFromGuid(soundId);

            s.Append(i);
            s.Append("    ");
            s.Append(_name);
            return s.ToString();
          
        }

        public long preLength(ref EPSSEditorData data)
        {
            Sound sound = data.getSoundFromSoundId(soundId);

            if (sound != null)
            {
                return sound.parameters().sizeAfterConversion(ref sound);
            }
            return 0;
        }
       
    }
}
