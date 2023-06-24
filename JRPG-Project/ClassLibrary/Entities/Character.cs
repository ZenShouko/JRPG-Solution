using JRPG_Project.ClassLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Entities
{
    public class Character
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Stats Stats { get; set; }
        public Weapon Weapon { get; set; }
        public Armour Armour { get; set; }
        public Amulet Amulet { get; set; }

        public Character()
        {
            //Assign default stats
            Stats = new Stats() 
            { 
                HP = 40, DEF = 10, 
                DMG = 5, SPD = 20, 
                CRC = 20, CRD = 25,
                STA = 50, STR = 10
            };
        }
    }
}
