using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{
    public class Sf2Info
    {
        public SortedDictionary<int, Sf2BankInfo> bankInfo;
        public Sf2Info()
        {
            bankInfo = new SortedDictionary<int, Sf2BankInfo>();
        }


        public void AddPreset(int bank, int patchNumber, string presetName)
        {
            Sf2BankInfo bi;
            if (bankInfo.ContainsKey(bank))
            {
                bi = bankInfo[bank];
            }
            else
            {
                bi = new Sf2BankInfo(bank);
            }

            Sf2Preset p = new Sf2Preset(presetName);
            bi.Add(patchNumber, p);

            if (bankInfo.ContainsKey(bank))
            {
                bankInfo[bank] = bi;
            }
            else
            {
                bankInfo.Add(bank, bi);
            }
        }


        public HashSet<int> Banks()
        {
            HashSet<int> banks = new HashSet<int>();
            foreach (var bank in bankInfo.Values)
            {
                banks.Add(bank.bank);
            }
            return banks;
        }
    }


    public class Sf2BankInfo
    {
        public int bank;
        public SortedDictionary<int, Sf2Preset> presets;
        

        public Sf2BankInfo(int v) {
            bank = v;
            presets = new SortedDictionary<int, Sf2Preset>();
        }

        public void Add(int patchNumber, Sf2Preset p)
        {
            presets.Add(patchNumber, p);
        }
    }

    public class Sf2Preset
    {
        public string name;

        public Sf2Preset(string name)
        {
            this.name = name;
        }
    }
}
