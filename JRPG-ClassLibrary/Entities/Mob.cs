using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JRPG_ClassLibrary.Entities
{
    public class Mob
    {
        public object ImageObject { get; set; }
        public int CurrentX { get; set; }
        public int CurrentY { get; set; }
        public string Name { get; set; }

        public Mob()
        {
            
        }
    }
}
