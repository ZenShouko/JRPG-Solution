using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Data
{
    /// <summary>
    /// Contains all data related to player.
    /// Will be saved to a file.
    /// </summary>
    public class PlayerData
    {
        //Player Team
        public List<Character> Team { get; set; } = new List<Character>();

        //Player Inventory
        public int Capacity { get; set; }
        public List<string> Collectables { get; set; } = new List<string>();
        public List<string> Armours { get; set; } = new List<string>();
        public List<string> Weapons { get; set; } = new List<string>();
        public List<string> Amulets { get; set; } = new List<string>();

        //Misceallaneous
        public DateTime LastSaveTime { get; set; }
    }
}
