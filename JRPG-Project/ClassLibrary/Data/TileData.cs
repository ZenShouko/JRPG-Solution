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
        public static List<Tile> AvailableTiles = new List<Tile>();

        public static void InitializeList()
        {
            string json = File.ReadAllText(@"../../Resources/Data/TileList.json");
            AvailableTiles = JsonConvert.DeserializeObject<List<Tile>>(json);
        }
    }
}
