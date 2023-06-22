using JRPG_Project.ClassLibrary.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class FoeData
    {
        public static DataTable FoeTable = new DataTable("FoeTable");

        public static void InitializeTable()
        {
            //Create columns
            DataColumn name = new DataColumn("Name", typeof(string));
            DataColumn level = new DataColumn("Level", typeof(int));
            DataColumn hp = new DataColumn("HP", typeof(int));
            DataColumn iconNames = new DataColumn("IconNames", typeof(string));
            DataColumn movementBehaviour = new DataColumn("MovementBehaviour", typeof(string));

            //Add columns to table
            FoeTable.Columns.Add(name);
            FoeTable.Columns.Add(level);
            FoeTable.Columns.Add(hp);
            FoeTable.Columns.Add(iconNames);
            FoeTable.Columns.Add(movementBehaviour);

            //Add rows to table
            ReadFoeData();
        }

        private static void ReadFoeData()
        {
            List<Foe> foes = new List<Foe>();

            //Deserialize JSON file FoeList.json
            string json = File.ReadAllText(@"../../Resources/Data/FoeList.JSON");
            foes = JsonConvert.DeserializeObject<List<Foe>>(json);

            //Add foes to FoeTable
            foreach (Foe foe in foes)
            {
                AddFoe(foe);
            }
        }

        private static void AddFoe(Foe foe)
        {
            FoeTable.Rows.Add(foe.Name, foe.Level, foe.HP, foe.IconNames, foe.MovementBehaviour);
        }
    }
}
