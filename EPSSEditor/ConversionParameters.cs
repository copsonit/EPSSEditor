using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPSSEditor
{
 
    public class ConversionType
    {
        public virtual string description()
        {
            return "";
        }

        public virtual double compressionFactor()
        {
            return 1.0;
        }
    }


    public class ConversionBitChange : ConversionType
    {
        public int fromBit;
        public int toBit;

        public ConversionBitChange() { }

        public ConversionBitChange(int from, int to)
        {
            fromBit = from;
            toBit = to;
        }

        public override string description()
        {
            StringBuilder sb = new StringBuilder("From: ");

            sb.Append(fromBit);
            sb.Append("-bit to ");
            sb.Append(toBit);
            sb.Append("-bit.");

            return sb.ToString();
        }

        public override double compressionFactor()
        {
            return (double)toBit / (double)fromBit;
        }
    }

  
    public class ConversionFreqChange : ConversionType
    {
        public int fromFreq;
        public int toFreq;

        public ConversionFreqChange() { }

        public ConversionFreqChange(int from, int to)
        {
            fromFreq = from;
            toFreq = to;
        }

        public override string description()
        {
            StringBuilder sb = new StringBuilder("From: ");

            sb.Append(fromFreq);
            sb.Append("Hz to ");
            sb.Append(toFreq);
            sb.Append("Hz.");

            return sb.ToString();
        }

        public override double compressionFactor()
        {
            return (double)toFreq / (double)fromFreq;
        }
    }

 
    public class ConversionChannelChange : ConversionType
    {
        public int fromChannel;
        public int toChannel;

        public ConversionChannelChange() { }

        public ConversionChannelChange(int from, int to)
        {
            fromChannel = from;
            toChannel = to;
        }

        public override string description()
        {
            StringBuilder sb = new StringBuilder("From: ");

            sb.Append(fromChannel);
            sb.Append("Channels to ");
            sb.Append(toChannel);
            sb.Append("Channels.");

            return sb.ToString();
        }

        public override double compressionFactor()
        {
            return (double)toChannel / (double)fromChannel;
        }
    }



    public class ConversionNormalize : ConversionType
    {

        public bool normalize = false;
        public int normalizePercentage = 100;

        public ConversionNormalize() { }
    }


    public class ConversionParameters
    {
//        public List<ConversionType> conversions;
        public ConversionBitChange bits;
        public ConversionFreqChange freq;
        public ConversionChannelChange channel;
        public ConversionNormalize normalize;

        public ConversionParameters()
        {
            //conversions = new List<ConversionType>();
        }


        public bool hasParameters()
        {
            return bits != null && freq != null && channel != null;
        }


        public long sizeAfterConversion(ref Sound snd)
        {
            double factor = 1.0;

            factor = factor * bits.compressionFactor();
            factor = factor * freq.compressionFactor();
            factor = factor * channel.compressionFactor();

            return (long)((double)snd.length * factor);
        }


        public void updateConversions(Sound snd, int toBit, int toFreq)
        {
            if (bits != null && freq != null && channel != null)
            {
                bits.toBit = toBit;
                freq.toFreq = toFreq;
                channel.toChannel = 1;

            } else
            {
                bits = new ConversionBitChange(snd.bitsPerSample, toBit);
                freq = new ConversionFreqChange(snd.samplesPerSecond, toFreq);
                channel = new ConversionChannelChange(snd.channels, 1);
            }
        }


        public void updateNormalize(bool normalize, int normalizePercentage)
        {
            if (this.normalize == null)
            {
                this.normalize = new ConversionNormalize();
            }

            this.normalize.normalize = normalize;
            this.normalize.normalizePercentage = normalizePercentage;
        }


        public string description()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(bits.description());
            sb.Append(freq.description());
            sb.Append(channel.description());


            return sb.ToString();
        }
    }
}
