using JRPG_ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace JRPG_Project.ClassLibrary.Entities
{
    public class Tile
    {
        public string Type { get; set; }
        public Brush TileColor { get; set; }
        public Border TileElement { get; set; }
        public MapFoe Foe { get; set; } = null;
        public MapPlayer Player { get; set; } = null;
        //public MapItem Item { get; set; } = null;
        public string TypeLootbox { get; set; }
        public Coordinates Position { get; set; }
        public bool IsWalkable { get; set; }

        public Tile()
        {

        }
    }
}
