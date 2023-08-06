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
        public static int Capacity { get; set; } = 25;
        public static int Coins { get; set; } = 100;
        public static List<Character> Team { get; set; } = new List<Character>();
        public static List<Collectable> Collectables { get; set; } = new List<Collectable>();
        public static List<Armour> Armours { get; set; } = new List<Armour>();
        public static List<Weapon> Weapons { get; set; } = new List<Weapon>();
        public static List<Amulet> Amulets { get; set; } = new List<Amulet>();
        public static Dictionary<string, int> Materials { get; set; } = new Dictionary<string, int>(); //Name, Amount

        /// <summary>
        /// UniqueID, IsCompleted
        /// </summary>
        public static Dictionary<string, bool> MarketRequests { get; set; } = new Dictionary<string, bool>();

        //Misceallaneous
        public static DateTime LastSaveTime { get; set; }
        public static DateTime MarketRefresh { get; set; }
        public static string CurrentTicket { get; set; }

        //Methods
        
    }
}
