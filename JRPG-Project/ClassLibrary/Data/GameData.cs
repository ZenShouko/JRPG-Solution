using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class GameData
    {
        public static DataSet DB_Game = new DataSet("DB_JRPG");

        public static void InitializeDatabase()
        {
            //Initialize Foe Table and add to database
            FoeData.InitializeTable();
            DB_Game.Tables.Add(FoeData.FoeTable);

            //Initialize Tile Table and add to database
            TileData.InitializeTable();
            DB_Game.Tables.Add(TileData.TileTable);
        }
    }
}
