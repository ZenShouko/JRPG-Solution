using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Entities
{
    /// <summary>
    /// Only serves to save the lootbox data,
    /// not for placing lootboxes on the map.
    /// </summary>
    public class Lootbox
    {
        public string Rarity { get; set; }
        public int CollectableOdd { get; set; }
        public int EquipableOdd { get; set; }
        public int CommonOdd { get; set; }
        public int SpecialOdd { get; set; }
        public int CursedOdd { get; set; }
        public int LegendaryOdd { get; set; }
    }
}
