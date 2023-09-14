using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_ClassLibrary.Entities
{
    public class Stage
    {
        public string Name { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public double TileWidth { get; set; }
        public double TileHeight { get; set; }
        //public Grid Platform { get; set; }
        public Grid VisiblePlatform { get; set; }

        public Grid FoeRadar { get; set; }

        public List<Tile> TileList = new List<Tile>();

        public MapPlayer Player { get; set; } = new MapPlayer();

        /// <summary>
        /// Contains characters as key. Item1 = HP deduction, Item2 = DEF deduction
        /// </summary>
        public Dictionary<Character, (int, int)> TeamHpDefDeduct { get; set; } = new Dictionary<Character, (int, int)>();

        public List<MapFoe> FoeList { get; set; } = new List<MapFoe>();

        public bool IsBattle { get; set; } = false; //Is there a battle going on?

        public MapFoe BattlingWith { get; set; } //Which foe are we battling with?

        public Dictionary<string, (int, int)> Progression { get; set; } = new Dictionary<string, (int, int)>(); //Keep track of which foes have been defeated, and collected lootboxes

        public Stage()
        {

        }
    }
}
