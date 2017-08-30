using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPSSEditor
{
    public class SoundLoadError
    {
        public SoundLoadError() {}
    }

    public class SoundLoadNoError : SoundLoadError
    {
        public SoundLoadNoError() { }
    }


    public class Sound
    {
        public string path;

        public Sound(string p) {
            path = p;
        }


        public SoundLoadError loadSound(string path)
        {
            return new SoundLoadNoError();
        }

        public string name()
        {
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}
