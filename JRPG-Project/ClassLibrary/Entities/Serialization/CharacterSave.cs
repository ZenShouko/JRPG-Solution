using JRPG_Project.ClassLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Entities.Serialization
{
    public class CharacterSave
    {
        public int Level { get; set; } = 1;
        public Stats Stats { get; set; } = new Stats();
        public Weapon Weapon { get; set; }
        public Armour Armour { get; set; }
        public Amulet Amulet { get; set; }
    }
}
