using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Controls;

namespace JRPG_ClassLibrary.Entities
{
    public class Mob
    {
        public Image Icon { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
    }
}
