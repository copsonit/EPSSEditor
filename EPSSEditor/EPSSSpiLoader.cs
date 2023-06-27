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

        public EPSSSpi Load(Uri path)
        {
            if (path.IsFile)
            {
			    EPSSSpiG0G1 spi = new EPSSSpiG0G1();
                spi.initialize();
				spi.Load(path);
                return spi;
            }
            return null;
        }
    }
}
