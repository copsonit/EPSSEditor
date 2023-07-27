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
using System.Windows.Forms;

namespace EPSSEditor
{ 
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

 
    public static class  CheckUpdates
    {
        public static void CheckForApplicationUpdate(Form1 mainForm, Version currentVersion, bool inStart = true)
        {
            EPSSEditorAktuell akt = CheckForUpdates(@"https://copson.se/epss/wp-content/uploads/EPSSEditorCurrentVersionInfo2.xml");
            if (akt == null) return;

            int nMajor = akt.aktuell.maj;
            int nMinor = akt.aktuell.min;
            int nBuild = akt.aktuell.bld;
            string version = nMajor + "." + nMinor + "." + nBuild;

            if (inStart)
            {
                string ignore = Properties.Settings.Default.IgnoreVersion;
                if (!String.IsNullOrEmpty(ignore))
                {

                    if (version == ignore)
                    {
                        return;
                    }
                }
            }

            string strPath = akt.folder;

            // Get my own version's numbers
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            //int nAppMajor = fileVersionInfo.FileMajorPart;
            //int nAppMinor = fileVersionInfo.FileMinorPart;
            //int nAppBuild = fileVersionInfo.FileBuildPart;

            int nAppMajor = currentVersion.Major;
            int nAppMinor = currentVersion.Minor;
            int nAppBuild = currentVersion.Build;

            if (nMajor > nAppMajor || (nMajor == nAppMajor && nMinor > nAppMinor) || (nMajor == nAppMajor && nMinor == nAppMinor && nBuild > nAppBuild))
            {
                string link = strPath;
                string updateMsg = "EPSS Editor v" + version + " released.";
                UpdateAvailable form = new UpdateAvailable(mainForm, updateMsg, link, version, inStart);

                form.ShowDialog();
            }
            else if (!inStart)
            {
                MessageBox.Show("You are already running latest.");
            }
        }



        private static EPSSEditorAktuell CheckForUpdates(string _folder)
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
}

