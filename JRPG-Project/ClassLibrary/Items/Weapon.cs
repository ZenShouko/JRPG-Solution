using JRPG_Project.ClassLibrary.Data;

namespace JRPG_Project.ClassLibrary
{
    public class Weapon : BaseItem, IStatsHolder
    {
        public Stats Stats { get; set; } = new Stats(); //Stats of the weapon
        public string Type { get; set; } //Type of weapon
        //public Charm Charm { get; set; } //Charm of the weapon

        public void WeaponCharm()
        {

        }

        public override void CopyFrom(BaseItem otherItem)
        {
            //Copy default properties
            base.CopyFrom(otherItem);

            //Copy weapon specific properties
            if (otherItem is Weapon wpn)
            {
                Stats = new Stats()
                {
                    HP = wpn.Stats.HP,
                    DMG = wpn.Stats.DMG,
                    DEF = wpn.Stats.DEF,
                    SPD = wpn.Stats.SPD,
                    STA = wpn.Stats.STA,
                    STR = wpn.Stats.STR,
                    CRC = wpn.Stats.CRC,
                    CRD = wpn.Stats.CRD,
                    XP = wpn.Stats.XP
                };

                Type = wpn.Type;
                ItemImage = ItemData.GetItemImage(wpn);
            }
        }
    }
}
