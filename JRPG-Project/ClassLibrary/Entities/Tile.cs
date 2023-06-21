using JRPG_ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Entities
{
    public class Tile
    {
        public string Type { get; set; }
        public Border TileElement { get; set; }
        public Mob MOB { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsWalkable { get; set; }

        public Tile()
        {
            MOB = null;
        }
    }
}
