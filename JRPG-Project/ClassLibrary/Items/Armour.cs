using JRPG_Project.ClassLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Items
{
    public class Armour : BaseItem, IStatsHolder
    {
        public Stats Stats { get; set; } //Stats of the armour
        //public Charm Charm { get; set; } //Charm of the armour

        public override void CopyFrom(BaseItem otherItem)
        {
            base.CopyFrom(otherItem);

            if (otherItem is Armour arm)
            {
                arm.Stats = new Stats()
                {
                    HP = arm.Stats.HP,
                    DEF = arm.Stats.DEF,
                    DMG = arm.Stats.DMG,
                    SPD = arm.Stats.SPD,
                    STA = arm.Stats.STA,
                    STR = arm.Stats.STR,
                    CRC = arm.Stats.CRC,
                    CRD = arm.Stats.CRD
                };

                ItemImage = ItemData.GetItemImage("Armours/" + arm.ImageName);
            }
        }
    }
}
