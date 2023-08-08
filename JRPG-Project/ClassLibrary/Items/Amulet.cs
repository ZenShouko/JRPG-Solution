using JRPG_Project.ClassLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Items
{
    public class Amulet : BaseItem, IStatsHolder
    {
        public Stats Stats { get; set; } //Stats of the amulet
        public string CharmID { get; set; } //ID of the charm
        public Charm Charm { get; set; } //Charm of the amulet

        public override void CopyFrom(BaseItem otherItem)
        {
            base.CopyFrom(otherItem);

            if (otherItem is Amulet amu)
            {
                Stats = new Stats()
                {
                    HP = amu.Stats.HP,
                    DEF = amu.Stats.DEF,
                    DMG = amu.Stats.DMG,
                    SPD = amu.Stats.SPD,
                    STA = amu.Stats.STA,
                    STR = amu.Stats.STR,
                    CRC = amu.Stats.CRC,
                    CRD = amu.Stats.CRD,
                    XP = amu.Stats.XP
                };

                CharmID = amu.CharmID;
                ItemImage = ItemData.GetItemImage(amu);

                //Keep track of this one, might cause problems
                Charm = amu.Charm;
            }
        }
    }
}
