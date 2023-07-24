using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace axcsCheck4Update
{ 
    public enum enVerion
    {
        EUnknown = -1,
        EMajor,
        EMinor,
        EBuild
    }

    public class axMain
    {
        private int _nMajor, _nMinor, _nBuild;

        private string _sXmlConfig, _sNewVersionPath;

        public axMain(string sPath = null)
        {
            if (sPath != null)
                _sXmlConfig = sPath;

            _nMajor = (int)enVerion.EUnknown;
            _nMinor = (int)enVerion.EUnknown;
            _nBuild = (int)enVerion.EUnknown;

            _sNewVersionPath = string.Empty;

            Check4Update();
        }

        private void Check4Update()
        {
            try
            {
                if (_sXmlConfig != null && !string.IsNullOrEmpty(_sXmlConfig))
                {
                    XmlDocument oDom = new XmlDocument();
                    
                    oDom.Load(_sXmlConfig);
                    string str = oDom.SelectSingleNode("//currentVersion/major").InnerText;
                    if (Int32.TryParse(str, out _nMajor))
                    {
                        str = oDom.SelectSingleNode("//currentVersion/minor").InnerText;
                        Int32.TryParse(str, out _nMinor);

                        str = oDom.SelectSingleNode("//currentVersion/build").InnerText;
                        Int32.TryParse(str, out _nBuild);

                        _sNewVersionPath = oDom.SelectSingleNode("//path").InnerText;
                    }
                }
            }
            catch(Exception )
            {
                
            }
        }

        public string GetNewVersionPath()
        {
            return _sNewVersionPath;
        }

        public int GetVersion(enVerion en)
        {
            switch(en)
            {
                case enVerion.EMajor:
                    return _nMajor;

                case enVerion.EMinor:
                    return _nMinor;

                case enVerion.EBuild:
                    return _nBuild;

                default:
                    return (int)enVerion.EUnknown;
            }
        }
    }
}
