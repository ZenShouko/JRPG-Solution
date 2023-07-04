using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Entities
{
    public abstract class MapObject
    {
        public Image Icon { get; set; } = new Image();
        public Coordinates Position { get; set; }
    }
}
