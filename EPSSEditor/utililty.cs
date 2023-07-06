using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPSSEditor
{
    // Utilities and extensions.

    public static class Utility
    {
        public static string ReplaceIllegalCharacters(string text)
        {
            foreach (char lDisallowed in System.IO.Path.GetInvalidFileNameChars())
            {
                text = text.Replace(lDisallowed.ToString(), "");
            }
            foreach (char lDisallowed in System.IO.Path.GetInvalidPathChars())
            {
                text = text.Replace(lDisallowed.ToString(), "");
            }

            return text;
        }
    }
    

    // https://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc
    // JerKimball suggestion.
    
    public static class Ext
    {
        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

        public static string ToPrettySize(this int value, int decimalPlaces = 0)
        {
            return ((long)value).ToPrettySize(decimalPlaces);
        }

        public static string ToPrettySize(this long value, int decimalPlaces = 0)
        {
            var asTb = Math.Round((double)value / OneTb, decimalPlaces);
            var asGb = Math.Round((double)value / OneGb, decimalPlaces);
            var asMb = Math.Round((double)value / OneMb, decimalPlaces);
            var asKb = Math.Round((double)value / OneKb, decimalPlaces);
            string chosenValue = asTb > 1 ? string.Format("{0}Tb", asTb)
                : asGb > 1 ? string.Format("{0}Gb", asGb)
                : asMb > 1 ? string.Format("{0}Mb", asMb)
                : asKb > 1 ? string.Format("{0}Kb", asKb)
                : string.Format("{0}B", Math.Round((double)value, decimalPlaces));
            return chosenValue;
        }
    }


    public static class DateTimeExtensions
    {
        public static ushort ToDosDateTime(this DateTime dateTime)
        {
            uint day = (uint)dateTime.Day;              // Between 1 and 31
            uint month = (uint)dateTime.Month;          // Between 1 and 12
            uint years = (uint)(dateTime.Year - 1980);  // From 1980

            if (years > 127)
                throw new ArgumentOutOfRangeException("Cannot represent the year.");

            uint dosDateTime = 0;
            dosDateTime |= day << (16 - 16);
            dosDateTime |= month << (21 - 16);
            dosDateTime |= years << (25 - 16);

            return unchecked((ushort)dosDateTime);
        }
    }


    public static class ByteStreamExtensions
    {
        public static string FromFixedByteStream(this byte[] s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in s)
            {
                stringBuilder.Append((Char)b);
            }
            return stringBuilder.ToString();
        }
    }

    public static class StringExtensions
    {
        public static byte[] ToFixedByteStream(this string s, int l)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                foreach (char c in s)
                {
                    ms.WriteByte(Convert.ToByte(c));
                    l--;
                    if (l <= 0) break;
                }
                if (l > 0)
                {
                    for (int i = 0; i < l; i++)
                    {
                        ms.WriteByte(0);
                    }
                }
  
                return ms.ToArray();
            }

        }


    }


    public static class BinaryWriterExtensions
    {
        public static void WriteBigEndian(this BinaryWriter w, UInt16 v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            w.Write(bytes);
        }


        public static void WriteBigEndian(this BinaryWriter w, UInt32 v)
        {
            byte[] bytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            w.Write(bytes);
        }
    }


    public static class BinaryReaderExtensions
    {
        public static UInt16 ReadBigEndianUInt16(this BinaryReader br)
        {
            var data = br.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public static UInt32 ReadBigEndianUInt32(this BinaryReader br)
        {
            var data = br.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }
    }
}
