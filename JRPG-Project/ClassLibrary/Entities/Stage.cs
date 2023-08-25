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
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        //public Grid Platform { get; set; }
        public Grid VisiblePlatform { get; set; }

        public List<Tile> TileList = new List<Tile>();

        public MapPlayer Player { get; set; } = new MapPlayer();

        public List<Character> Team { get; set; } = new List<Character>();

        public Stage()
        {

        }
    }
}
