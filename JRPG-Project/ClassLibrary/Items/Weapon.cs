using JRPG_Project.ClassLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary
{
    public class Weapon : BaseItem
    {
        public Stats Stats { get; set; } //Stats of the weapon
        public string Type { get; set; } //Type of weapon
        //public Charm Charm { get; set; } //Charm of the weapon

        public void WeaponCharm()
        {

        }

    }
}
