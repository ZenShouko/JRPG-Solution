using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Media;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class TileData
    {
        public static DataTable TileTable { get; set; }

        public static List<string> AvailableTiles = new List<string>();

        public static void InitializeTable()
        {
            TileTable = new DataTable("TileTable");
            TileTable.Columns.Add("Type", typeof(string));
            TileTable.Columns.Add("Code", typeof(string));
            TileTable.Columns.Add("TileColor", typeof(Brush));
            TileTable.Columns.Add("IsWalkable", typeof(bool));
            TileTable.Columns.Add("MOB", typeof(Mob));

            //Read tile data from JSON file
            ReadTileData();
        }

        private static void ReadTileData()
        {
            List<Tile> tileList = new List<Tile>();

            //Get Tile Data from Tilelist.json
            string json = File.ReadAllText(@"../../Resources/Data/TileList.json");
            tileList = JsonConvert.DeserializeObject<List<Tile>>(json);

            //Add tiles to TileTable
            foreach (Tile tile in tileList)
            {
                AddTile(tile);
                AvailableTiles.Add(tile.Code);
            }
        }

        private static void AddTile(Tile tile)
        {
            TileTable.Rows.Add(tile.Type, tile.Code, tile.TileColor, tile.IsWalkable, null);
        }
    }
}
