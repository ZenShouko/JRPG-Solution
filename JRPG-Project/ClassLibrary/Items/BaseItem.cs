using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary
{
    /// <summary>
    /// Contains the base properties for all items
    ///</summary>
    public abstract class BaseItem
    {
        public string UniqueID { get; set; } //Unique on inventory level
        public string ID { get; set; } //Unique on global level
        public int Level { get; set; } = 1; //Level of the item
        public string Rarity { get; set; } //Rarity of the item
        public string Name { get; set; }
        public string Description { get; set; } //Description of the item
        public string ImageName { get; set; } //Name of the image, used when deserializing
        public int Value { get; set; } //Value of the item

        [JsonIgnore]
        public Image ItemImage { get; set; } //Image of the item

        public override string ToString()
        {
            return $"[{Level}] {Name}";
        }

        /// <summary>
        /// Copies the attributes of another (reference) item.
        /// </summary>
        /// <param name="otherItem"></param>
        public virtual void CopyFrom(BaseItem otherItem)
        {
            UniqueID = otherItem.GetType().Name.ToLower() + Guid.NewGuid().ToString();
            ID = otherItem.ID;
            Level = otherItem.Level;
            Rarity = otherItem.Rarity;
            Name = otherItem.Name;
            Description = otherItem.Description;
            ImageName = otherItem.ImageName;
            Value = otherItem.Value;
        }

        /// <summary>
        /// Levels up item and adds stats.
        /// </summary>
        /// <param name="item"></param>
        public virtual void LevelUp(BaseItem item)
        {
            //Vars
            IStatsHolder obj = item as IStatsHolder;
            Dictionary<int, (int, Stats)> xpTable = new Dictionary<int, (int, Stats)>();

            //Get the correct xp table
            if (item.GetType().Name.ToUpper() == "WEAPON")
                xpTable = LevelData.WeaponXPTable;
            else if (item.GetType().Name.ToUpper() == "ARMOUR")
                xpTable = LevelData.ArmourXPTable;
            else if (item.GetType().Name.ToUpper() == "AMULET")
                xpTable = LevelData.AmuletXPTable;

            //Apply the level up
            item.Level++;

            //Substract the required XP
            obj.Stats.XP -= xpTable[item.Level].Item1;

            //Add the stats to the item
            obj.Stats.HP += xpTable[item.Level].Item2.HP;
            obj.Stats.DEF += xpTable[item.Level].Item2.DEF;
            obj.Stats.DMG += xpTable[item.Level].Item2.DMG;
            obj.Stats.SPD += xpTable[item.Level].Item2.SPD;
            obj.Stats.STA += xpTable[item.Level].Item2.STA;
            obj.Stats.STR += xpTable[item.Level].Item2.STR;
            obj.Stats.CRC += xpTable[item.Level].Item2.CRC;
            obj.Stats.CRD += xpTable[item.Level].Item2.CRD;

            //Reset xp to max if level is max
            if (item.Level == xpTable.Keys.LastOrDefault())
            {
                obj.Stats.XP = xpTable[item.Level].Item1;
            }
            return;

            //Old code
            if (item is Weapon wpn)
            {
                //Increase the level
                wpn.Level++;

                //Substract the required XP
                wpn.Stats.XP -= LevelData.WeaponXPTable[wpn.Level].Item1;

                //Get the stats of the next level
                Stats levelUpStats = LevelData.WeaponXPTable[wpn.Level].Item2;

                //Add the stats to the item
                wpn.Stats.HP += levelUpStats.HP;
                wpn.Stats.DEF += levelUpStats.DEF;
                wpn.Stats.DMG += levelUpStats.DMG;
                wpn.Stats.SPD += levelUpStats.SPD;
                wpn.Stats.STA += levelUpStats.STA;
                wpn.Stats.STR += levelUpStats.STR;
                wpn.Stats.CRC += levelUpStats.CRC;
                wpn.Stats.CRD += levelUpStats.CRD;

                //Reset xp to max if level is max
                if (wpn.Level == LevelData.WeaponXPTable.Keys.LastOrDefault())
                {
                    wpn.Stats.XP = LevelData.WeaponXPTable[wpn.Level].Item1;
                }
            }
            else if (item is Armour arm)
            {
                //Increase the level
                arm.Level++;

                //Substract the required XP
                arm.Stats.XP -= LevelData.ArmourXPTable[arm.Level].Item1;

                //Get the stats of the next level
                Stats levelUpStats = LevelData.ArmourXPTable[arm.Level].Item2;

                //Add the stats to the item
                arm.Stats.HP += levelUpStats.HP;
                arm.Stats.DEF += levelUpStats.DEF;
                arm .Stats.DMG += levelUpStats.DMG;
                arm.Stats.SPD += levelUpStats.SPD;
                arm.Stats.STA += levelUpStats.STA;
                arm.Stats.STR += levelUpStats.STR;
                arm.Stats.CRC += levelUpStats.CRC;
                arm.Stats.CRD += levelUpStats.CRD;

                //Reset xp to max if level is max
                if (arm.Level == LevelData.ArmourXPTable.Keys.LastOrDefault())
                {
                    arm.Stats.XP = LevelData.ArmourXPTable[arm.Level].Item1;
                }
            }
            else if (item is Amulet amu)
            {
                //Increase the level
                amu.Level++;

                //Substract the required XP
                amu.Stats.XP -= LevelData.AmuletXPTable[amu.Level].Item1;

                //Get the stats of the next level
                Stats levelUpStats = LevelData.AmuletXPTable[amu.Level].Item2;

                //Add the stats to the item
                amu.Stats.HP += levelUpStats.HP;
                amu.Stats.DEF += levelUpStats.DEF;
                amu.Stats.DMG += levelUpStats.DMG;
                amu.Stats.SPD += levelUpStats.SPD;
                amu.Stats.STA += levelUpStats.STA;
                amu.Stats.STR += levelUpStats.STR;
                amu.Stats.CRC += levelUpStats.CRC;
                amu.Stats.CRD += levelUpStats.CRD;

                //Reset xp to max if level is max
                if (amu.Level == LevelData.AmuletXPTable.Keys.LastOrDefault())
                {
                    amu.Stats.XP = LevelData.AmuletXPTable[amu.Level].Item1;
                }
            }
        }
    }
}
