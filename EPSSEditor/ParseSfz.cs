using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace EPSSEditor
{


    // First version of sfz reader.
    // Can only parse one key=value pair per line as this was the use case.

    public interface ISfzSection
    {
        void Init(string line);
        bool Parse(string line);



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


        public virtual void Init(string line)
        {
            // No Parameters default
        }

        public virtual bool Parse(string line)
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
                    // TODO, rewrite to use the new method in Region, if we find any issues with the one below.
                    string[] words0;
                    if (line.Contains("sample="))
                    {
                        words0 = line.Split('='); // only sample allowed to contain spaces, dont break on space
                    } else
                    {
                        words0 = line.Split('=', ' ');
                    }

                    List<string> words = new List<string>();
                    foreach(string w in words0)
                    {
                        string v = w.TrimEnd().TrimStart().ToLower();
                        if (v.Length > 0)
                        {
                            // Special case that tries to fix missed linebreak, i.e. numerical value and next parameter written without space
                            if ((words0.Length % 2) != 0 && Char.IsDigit(v[0]) && !Char.IsDigit(v[v.Length-1]))
                            {
                                StringBuilder sb = new StringBuilder();
                                StringBuilder sb2 = new StringBuilder();
                                foreach(var c in v)
                                {
                                    if (Char.IsDigit(c)) sb.Append(c);
                                    else sb2.Append(c);
                                }
                                words.Add(sb.ToString());
                                words.Add(sb2.ToString());
                            } else
                            {
                                words.Add(v);
                            }                           
                        }
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


        public string FilePath(string basePath)
        {
            string file = GetValue("sample");
            if ( file != null)  return System.IO.Path.Combine(basePath, file);
            return null;
        }

    }


    public class SfzRegionSection : SfzBase
    {
        public override void Init(string line)
        {
            for(int i=0;i<line.Length;i++)
            {
                if (line[i] == '>')
                {
                    string afterRegion = line.Substring(i + 1).Trim();
                    if (OccurenceOf(afterRegion, '=') >= 1)
                    {
                        string[] words = afterRegion.Split('=');
                        int wordIndex = 0;
                        string key = words[wordIndex++].TrimEnd().TrimStart().ToLower();
                        while (true)
                        {
                            string value = null;
                            string nextKey = null;

                            string valueAndNextKey = words[wordIndex++].TrimEnd().TrimStart().ToLower();
                            if (wordIndex > (words.Length - 1))
                            {
                                value = valueAndNextKey;
                                nextKey = null;
                            }
                            else
                            {
                                for (int j = valueAndNextKey.Length - 1; j >= 0; j--)
                                {
                                    if (valueAndNextKey[j] == ' ')
                                    {
                                        value = valueAndNextKey.Substring(0, j).Trim();
                                        nextKey = valueAndNextKey.Substring(j, valueAndNextKey.Length - j).Trim();
                                        break;
                                    }
                                }
                            }
                            if (value != null)
                            {
                                variables.Add(key, value);
                                key = nextKey;
                            }
                            if (key == null) break;
                        }

                    }
                }
            }
        }
    }


    public class SfzGenericSection : SfzBase
    {
        public string header;
        public override void Init(string line)
        {
            header = line;
        }
    }


    public class ParseSfz
    {
        public ParseSfz()
        {

        }


        bool IsLineComment(string line)
        {
            if (line.StartsWith("//")) return true;
            return false;
                   
        }

        public List<SfzBase> Parse(string file)
        {
            Dictionary<string, System.Type> dict = new Dictionary<string, System.Type>
            {
                { "<region>", typeof(SfzRegionSection) }
            };

            string[] lines = System.IO.File.ReadAllLines(file);
            
            SfzBase fn = null;
            List<SfzBase> bases = new List<SfzBase>();

            try
            {
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                    bool result = false;

                    if (line.Length > 0 && !IsLineComment(line))
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
                                        fn.Init(line);
                                        result = true;
                                        kwFound = true;
                                        break;
                                    }
                                }
                                if (!kwFound)
                                {
                                    fn = new SfzGenericSection();
                                    fn.Init(line);
                                    result = true;
                                }
                            }
                            else
                            {
                                result = fn.Parse(line);
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return bases;
        }
    }
}
