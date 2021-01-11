using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPSSEditor
{


    // First version of sfz reader.
    // Can only parse one key=value pair per line as this was the use case.

    public interface ISfzSection
    {
        void init(string line);
        bool parse(string line);



    }

    public class SfzBase : ISfzSection
    {
        public Dictionary<string, string> variables = new Dictionary<string, string>();



        public virtual void init(string line)
        {
            // No Parameters default
        }

        public virtual bool parse(string line)
        {
            if (line.Contains('<') && line.Contains('>'))
            {
                // We found something else....
                return false;
            }
            else
            {
                string[] words = line.Split('=');
                if (words.Length == 2)
                {
                    variables.Add(words[0].TrimEnd().TrimStart().ToLower(), words[1].TrimEnd().TrimStart().ToLower());
                }
                else
                {
                    throw (new ArgumentException("Incorrect parameters after <region>."));
                 }

            }
            return true;
        }
    }


    public class SfzRegionSection : SfzBase
    {
        public string file;


        public string FilePath(string basePath)
        {
            return System.IO.Path.Combine(basePath, file);
        }

        public override void init(string line)
        {
            for(int i=0;i<line.Length;i++)
            {
                if (line[i] == '>')
                {
                    string[] words = line.Substring(i + 1).Split('=');
                    if (words.Length == 2)
                    {
                        if (words[0].TrimEnd().TrimStart().ToLower() == "sample")
                        {
                            file = words[1].TrimEnd().TrimStart().ToLower();
                        }
                    }
                    else
                    {
                        throw (new ArgumentException("Incorrect parameters after <region>."));
                    }
                    break;
                }
            }
        }

    }


    public class SfzGenericSection : SfzBase
    {
        public string header;
        public override void init(string line)
        {
            header = line;
        }
    }


    public class ParseSfz
    {
        public ParseSfz()
        {

        }

        public List<SfzBase> parse(string file)
        {
            Dictionary<string, System.Type> dict = new Dictionary<string, System.Type>();
            dict.Add("<region>", typeof(SfzRegionSection));

            string[] lines = System.IO.File.ReadAllLines(file);

            SfzBase fn = null;
            List<SfzBase> bases = new List<SfzBase>();
            foreach (string line in lines)
            {
                bool result = false;

                if (line.Length > 0)
                {
                    while (!result)
                    {
                        if (fn == null)
                        {
                            bool kwFound = false;
                            foreach (var d in dict)
                            {
                                if (line.Contains(d.Key))
                                {
                                    fn = (SfzBase)Activator.CreateInstance(d.Value);
                                    fn.init(line);
                                    result = true;
                                    kwFound = true;
                                    break;
                                }
                            }
                            if (!kwFound)
                            {
                                fn = new SfzGenericSection();
                                fn.init(line);
                                result = true;
                            }
                        }
                        else
                        {
                            result = fn.parse(line);
                            if (!result)
                            {
                                bases.Add(fn);
                                fn = null;
                            }
                        }
                    }
                }
            }
            return bases;
        }
    }
}
