using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

        public int OccurenceOf(string line, char c)
        {
            int n = 0;
            foreach (var ch in line)
            {
                if (ch == c) n++;   
            }
            return n;
        }


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
                if (line.Contains("//"))
                {
                    // Comment, do nothing
                }
                else
                {
                    string[] words0;
                    if (line.Contains("sample="))
                    {
                        words0 = line.Split('=');
                    } else
                    {
                        words0 = line.Split('=', ' ');
                    }

                    List<string> words = new List<string>();
                    foreach(string w in words0)
                    {
                        string v = w.TrimEnd().TrimStart().ToLower();
                        if (v.Length > 0) words.Add(v);
                    }

                    if (words.Count >= 2)
                    {
                        int i = 0;
                        while (true)
                        {
                            variables.Add(words[i].TrimEnd().TrimStart().ToLower(), words[i + 1].TrimEnd().TrimStart().ToLower());
                            i += 2;
                            if (i >= words.Count) break;
                        }

                    }
                    else
                    {
                        // throw (new ArgumentException("Incorrect parameters after <region>."));
                    }
                }
            }
            return true;
        }


        public string GetValue(string variable)
        {
            if (variables.ContainsKey(variable)) { return variables[variable]; }
            return null;
        }

        
    }


    public class SfzRegionSection : SfzBase
    {
        //public string file;


        public string FilePath(string basePath)
        {
            string file = variables["sample"];
            return System.IO.Path.Combine(basePath, file);
        }

        public override void init(string line)
        {
            for(int i=0;i<line.Length;i++)
            {
                if (line[i] == '>')
                {
                    List<string> stringPairsToParse =   new List<string>();

                    string afterRegion = line.Substring(i + 1).Trim();
                    if (OccurenceOf(afterRegion, ' ') == 0)
                    {
                        stringPairsToParse.Add(afterRegion);

                        
                    } else
                    {
                        string[] words = afterRegion.Split(' ');
                        stringPairsToParse = words.ToList();
                    }


                    foreach (var stringPair in stringPairsToParse)
                    {

                        string[] words = stringPair.Split('=');
                        if (words.Length == 2)
                        {
                            string key = words[0].TrimEnd().TrimStart().ToLower();
                            //if (key == "sample")
                            //{
                                string value = words[1].TrimEnd().TrimStart().ToLower();
                                variables.Add(key, value);

                            //}
                        }
                        else
                        {

                            //throw (new ArgumentException("Incorrect parameters after <region>."));
                        }
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


        bool isLineComment(string line)
        {
            if (line.StartsWith("//")) return true;
            return false;
                   
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

                if (line.Length > 0 && !isLineComment(line))
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
            bases.Add(fn);
            return bases;
        }
    }
}
