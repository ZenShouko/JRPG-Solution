using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Entities
{
    /// <summary>
    /// The object that is used on the platform map.
    /// </summary>
    public class MapFoe : MapObject
    {
        public string IconNames { get; set; }
        public bool HasDetectedPlayer { get; set; }
        public string MovementBehaviour { get; set; }
    }
}
