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
            obj.Stats.XP -= Convert.ToInt16(xpTable[item.Level].Item1 * ItemData.RarityMultipliers[item.Rarity.ToUpper()]);

            //Add the stats to the item. Multiply by rarity multiplier
            obj.Stats.HP += Convert.ToInt16(xpTable[item.Level].Item2.HP * ItemData.RarityMultipliers[$"{item.Rarity.ToUpper()}"]);
            obj.Stats.DEF += Convert.ToInt16(xpTable[item.Level].Item2.DEF * ItemData.RarityMultipliers[$"{item.Rarity.ToUpper()}"]);
            obj.Stats.DMG += Convert.ToInt16(xpTable[item.Level].Item2.DMG * ItemData.RarityMultipliers[$"{item.Rarity.ToUpper()}"]);
            obj.Stats.SPD += Convert.ToInt16(xpTable[item.Level].Item2.SPD * ItemData.RarityMultipliers[$"{item.Rarity.ToUpper()}"]);
            obj.Stats.STA += Convert.ToInt16(xpTable[item.Level].Item2.STA * ItemData.RarityMultipliers[$"{item.Rarity.ToUpper()}"]);
            obj.Stats.STR += Convert.ToInt16(xpTable[item.Level].Item2.STR * ItemData.RarityMultipliers[$"{item.Rarity.ToUpper()}"]);
            obj.Stats.CRC += Convert.ToInt16(xpTable[item.Level].Item2.CRC * ItemData.RarityMultipliers[$"{item.Rarity.ToUpper()}"]);
            obj.Stats.CRD += Convert.ToInt16(xpTable[item.Level].Item2.CRD * ItemData.RarityMultipliers[$"{item.Rarity.ToUpper()}"]);

            //Reset xp to max if level is max
            if (item.Level == xpTable.Keys.LastOrDefault())
            {
                obj.Stats.XP = Convert.ToInt16(xpTable[item.Level].Item1 * ItemData.RarityMultipliers[item.Rarity.ToUpper()]);
            }
        }
    }
}
