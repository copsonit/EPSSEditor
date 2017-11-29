using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath; // for XPathSelectElements
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;

//        http://computermusicresource.com/GM.Percussion.KeyMap.html

namespace EPSSEditor
{


    public class EPSSData
    {
        [XmlElement("Mapping")]
        public Mappings[] mappings { get; set; }
    }

    public class Mappings
    {
        [XmlAnyAttribute]
        public XmlAttribute[] XAttributes { get; set; }
    }


    public class Mapping
    {
        public string note;
        public byte midiNote;
        public string key;
        public string description;
    }

    public class DrumSettingsHelper
    {
        public Mapping[] mappings;

        public void initialize(string drumSettingsFileName)
        {



            List<Mapping> drumMappings = new List<Mapping>();
            //            List<string> output = new List<string>();
            XDocument doc = null;
            try
            {
                doc = XDocument.Load(drumSettingsFileName);


            }
            catch (Exception e)
            {
                MessageBox.Show("drumMapping.xml cannot be loaded. Error:\n" + e.ToString());
            }
            if (doc != null)
            {
                string xml = doc.Declaration.ToString() + doc.ToString(SaveOptions.DisableFormatting);

                System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
                System.IO.StringReader xmlRead = new System.IO.StringReader(xml);

                System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(EPSSData));
                EPSSData res = (EPSSData)xSerializer.Deserialize(xmlRead);

                foreach (var mapping in res.mappings)
                {
                    Mapping m = new Mapping();
                    foreach (XmlAttribute a in mapping.XAttributes)
                    {
                        if (a.Name == "note")
                        {
                            m = new Mapping();
                            m.note = a.Value;
                            m.midiNote = Convert.ToByte(m.note);
                        }
                        else if (a.Name == "key")
                        {
                            m.key = a.Value;
                        }
                        else if (a.Name == "description")
                        {
                            m.description = a.Value;
                            drumMappings.Add(m);
                        }
                    }
                }
            }

            mappings = drumMappings.ToArray();
        }
    }
}

