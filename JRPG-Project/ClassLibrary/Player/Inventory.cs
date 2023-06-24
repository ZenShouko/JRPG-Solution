using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Player
{
    public static class Inventory
    {
        public static int Capacity { get; set; } = 10;
        public static List<Collectable> Collectables { get; set; } = new List<Collectable>();
        public static List<Armour> Armours { get; set; } = new List<Armour>();
        public static List<Weapon> Weapons { get; set; } = new List<Weapon>();
        public static List<Amulet> Amulets { get; set; } = new List<Amulet>();
    }
}
