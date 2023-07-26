using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath; // for XPathSelectElements
using System.Xml.Serialization;


namespace EPSSEditor
{ 
    /*
    public enum myVersion
    {
        VUnknown = -1,
        VMajor,
        VMinor,
        VBuild
    }
    */

    
    public class EPSSEditorAktuell
    {
        public EPSSEditorAktuellInst aktuell;
        public string folder;
        public EPSSEditorAktuell() { }
    }

    public class EPSSEditorAktuellInst
    {
        public int maj;
        public int min;
        public int bld;
        public EPSSEditorAktuellInst() { }
    }



    public static class KollaEfterNyVersion
    {
        public static EPSSEditorAktuell KollaEfterNy(string _folder)
        {

            EPSSEditorAktuell akt = null;
            XDocument doc = null;
            try
            {
                doc = XDocument.Load(_folder);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot load xml at {0}", _folder);
                Console.WriteLine("Exception: {0}", e.ToString());
            }
            if (doc != null)
            {
                string xml = doc.Declaration.ToString() + doc.ToString(SaveOptions.DisableFormatting);

                System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
                System.IO.StringReader xmlRead = new System.IO.StringReader(xml);

                System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(EPSSEditorAktuell));
                try
                {
                    akt = (EPSSEditorAktuell)xSerializer.Deserialize(xmlRead);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Wrong format of xml.");
                    Console.WriteLine(e.ToString());
                }
            }

            return akt;
        }
    }

            /*
                if (!string.IsNullOrEmpty(_sXmlConfig))
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


        /*
        public string GetNewVersionPath()
        {
            return _sNewVersionPath;
        }

        public int GetVersion(myVersion v)
        {
            switch(v)
            {
                case myVersion.VMajor:
                    return _nMajor;

                case myVersion.VMinor:
                    return _nMinor;

                case myVersion.VBuild:
                    return _nBuild;

                default:
                    return (int)myVersion.VUnknown;
            }
        }
        
    }
    */
}

