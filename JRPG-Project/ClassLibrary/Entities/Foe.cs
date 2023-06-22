using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Entities
{
    public class Foe
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public Image Icon { get; set; }
        public string IconNames { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }
        public string MovementBehaviour { get; set; }
    }
}
