using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{
    public class EPSSSpiLoader
    {
        public string LastError;

        public EPSSSpiLoader() { }

        public EPSSSpi Load(Uri path, out string errorMessage)
        {
            errorMessage = null;
            if (path.IsFile)
            {
			    EPSSSpiG0G1 spi = new EPSSSpiG0G1();
                spi.initialize();
                if (spi.Load(path, out errorMessage) == 0) return spi;
            }
            return null;
        }
    }
}
