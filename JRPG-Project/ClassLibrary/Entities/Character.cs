﻿using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Entities
{
    public class Character : IStatsHolder
    {
        #region Important Properties
        public int Level { get; set; } = 1;
        public Stats Stats { get; set; } = new Stats();
        public Weapon Weapon { get; set; }
        public Armour Armour { get; set; }
        public Amulet Amulet { get; set; }
        #endregion

        public string EquipmentIDs { get; set; }
        public string ID { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public string ImageName { get; set; }

        [JsonIgnore]
        public Image CharImage { get; set; }

        /// <summary>
        /// Returns the stats of the character with all the equipment stats added.
        /// </summary>
        /// <returns></returns>
        public Stats GetAccumulatedStats()
        {
            Stats accStats = new Stats();

            //Add base stats
            accStats.HP += Stats.HP;
            accStats.DMG += Stats.DMG;
            accStats.DEF += Stats.DEF;
            accStats.SPD += Stats.SPD;
            accStats.STA += Stats.STA;
            accStats.STR += Stats.STR;
            accStats.CRC += Stats.CRC;
            accStats.CRD += Stats.CRD;

            //Add weapon stats
            if (Weapon != null)
            {
                accStats.HP += Weapon.Stats.HP;
                accStats.DMG += Weapon.Stats.DMG;
                accStats.DEF += Weapon.Stats.DEF;
                accStats.SPD += Weapon.Stats.SPD;
                accStats.STA += Weapon.Stats.STA;
                accStats.STR += Weapon.Stats.STR;
                accStats.CRC += Weapon.Stats.CRC;
                accStats.CRD += Weapon.Stats.CRD;
            }

            //Add armour stats
            if (Armour != null)
            {
                accStats.HP += Armour.Stats.HP;
                accStats.DMG += Armour.Stats.DMG;
                accStats.DEF += Armour.Stats.DEF;
                accStats.SPD += Armour.Stats.SPD;
                accStats.STA += Armour.Stats.STA;
                accStats.STR += Armour.Stats.STR;
                accStats.CRC += Armour.Stats.CRC;
                accStats.CRD += Armour.Stats.CRD;
            }

            //Add amulet stats
            if (Amulet != null)
            {
                accStats.HP += Amulet.Stats.HP;
                accStats.DMG += Amulet.Stats.DMG;
                accStats.DEF += Amulet.Stats.DEF;
                accStats.SPD += Amulet.Stats.SPD;
                accStats.STA += Amulet.Stats.STA;
                accStats.STR += Amulet.Stats.STR;
                accStats.CRC += Amulet.Stats.CRC;
                accStats.CRD += Amulet.Stats.CRD;
            }

            return accStats;
        }

        public void LevelUp()
        {
            //Apply the level up
            Level++;

            //Substract the required XP
            Stats.XP -= LevelData.CharacterXPTable[Level].Item1;

            //Add the stats to the item
            Stats.HP += LevelData.CharacterXPTable[Level].Item2.HP;
            Stats.DEF += LevelData.CharacterXPTable[Level].Item2.DEF;
            Stats.DMG += LevelData.CharacterXPTable[Level].Item2.DMG;
            Stats.SPD += LevelData.CharacterXPTable[Level].Item2.SPD;
            Stats.STA += LevelData.CharacterXPTable[Level].Item2.STA;
            Stats.STR += LevelData.CharacterXPTable[Level].Item2.STR;
            Stats.CRC += LevelData.CharacterXPTable[Level].Item2.CRC;
            Stats.CRD += LevelData.CharacterXPTable[Level].Item2.CRD;

            //Reset xp to max if level is max
            if (Level == LevelData.CharacterXPTable.Keys.LastOrDefault())
            {
                Stats.XP = LevelData.CharacterXPTable[Level].Item1;
            }
        }
    }
}
