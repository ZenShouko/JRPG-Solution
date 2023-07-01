using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Entities
{
    /// <summary>
    /// Contains the coordinates of an object and also the direction it's going.
    /// </summary>
    public class Coordinates
    {
        //Coordinates of the object
        public int X { get; set; }
        public int Y { get; set; }

        //Direction of the object
        public int DirectionX { get; set; } = 0;
        public int DirectionY { get; set; } = 0;
    }
}
