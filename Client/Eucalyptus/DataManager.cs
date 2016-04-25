using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eucalyptus
{

    public static class DataManager
    {
        public static void AddAddress(string name, long address)
        {
            File.AppendAllText("addresses.cfg", name + ";" + address + "\n");
        }

        public static long GetAddressByName(string name)
        {
            long id = -1;

            try
            {
                foreach (string line in File.ReadAllLines("addresses.cfg"))
                {
                    string[] args = line.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (args[0].ToLower().Trim() == name.ToLower().Trim()) return long.Parse(args[1]);
                }

            }
            catch (Exception ex)
            {
                //
            }

            return id;
        }

        public static void SetName(string name)
        {
            File.WriteAllText("name.cfg", name.ToLower().Trim());
        }

        public static string GetName()
        {
            return File.ReadAllText("name.cfg");
        }
    }
}
