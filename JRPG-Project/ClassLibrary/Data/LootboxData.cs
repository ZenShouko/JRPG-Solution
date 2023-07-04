using JRPG_Project.ClassLibrary.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class LootboxData
    {
        public static List<Lootbox> LootboxList = new List<Lootbox>();

        public static void InitializeList()
        {
            //Read lootbox data from file
            string json = File.ReadAllText(@"../../Resources/Data/lootbox.json");
            LootboxList = JsonConvert.DeserializeObject<List<Lootbox>>(json);
        }
    }
}
