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
    public class Level
    {
        public string Name { get; set; }
        public int LevelNumber { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public List<Mob> MobList = new List<Mob>();
        public List<Tile> TileList = new List<Tile>();
        public List<string> Collectables = new List<string>();
        public List<Foe> FoeList = new List<Foe>();
        public Grid Playfield { get; set; }

        public Level()
        {

        }
    }
}
