using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class LevelData
    {
        //Dictionary of XP needed for each level. Key = Level, Value = (XP, Stats)
        public static Dictionary<int, (int, Stats)> CharacterXPTable = new Dictionary<int, (int, Stats)>();
        public static Dictionary<int, (int, Stats)> WeaponXPTable = new Dictionary<int, (int, Stats)>();
        public static Dictionary<int, (int, Stats)> ArmourXPTable = new Dictionary<int, (int, Stats)>();
        public static Dictionary<int, (int, Stats)> AmuletXPTable = new Dictionary<int, (int, Stats)>();

        public static void InitializeDictionaries()
        {
            //Characters
            for (double i = 2; i <= 9; i++)
            {
                double xp = i * 200 + (i / 2 * 145);
                CharacterXPTable.Add(Convert.ToInt16(i), (Convert.ToInt16(xp), CalculateStatIncrease(Convert.ToInt16(i), "Character")));
            }

            //Weapons
            for (double i = 2; i <= 9; i++)
            {
                double xp = i * 100 * 2.1;
                WeaponXPTable.Add(Convert.ToInt16(i), (Convert.ToInt16(xp), CalculateStatIncrease(Convert.ToInt16(i), "Weapon")));
            }

            //Armours
            for (double i = 2; i <= 9; i++)
            {
                double xp = i * 100 * 1.5;
                ArmourXPTable.Add(Convert.ToInt16(i), (Convert.ToInt16(xp), CalculateStatIncrease(Convert.ToInt16(i), "Armour")));
            }

            //Amulets
            for (double i = 2; i <= 9; i++)
            {
                double xp = i * 180 + (10 - i) * 50;
                AmuletXPTable.Add(Convert.ToInt16(i), (Convert.ToInt16(xp), CalculateStatIncrease(Convert.ToInt16(i), "Amulet")));
            }
        }

        private static Stats CalculateStatIncrease(int level, string type)
        {
            Stats stats = new Stats();

            switch (type)
            {
                case "Character":
                    {
                        stats.HP = level % 2 == 0 ? 10 : 6;
                        stats.DEF = level % 2 == 0 ? 2 : 1;
                        stats.DMG = level % 2 == 0 ? 2 : 1;
                        stats.SPD = level % 2 == 0 ? 6 : 9;
                        stats.STA = level % 2 == 0 ? 6 : 9;
                        stats.STR = level % 2 == 0 ? 3 : 5;
                        stats.CRC = level % 2 == 0 ? 1 : 0;
                        stats.CRD = level % 2 == 0 ? 2 : 1;
                        break;
                    }
                case "Weapon":
                    {
                        stats.DMG = level % 2 == 0 ? 4 : 2;
                        stats.SPD = level % 2 == 0 ? 1 : 2;
                        stats.CRC = level % 2 == 0 ? 0 : 1;
                        stats.CRD = level % 2 == 0 ? 1 : 2;
                        stats.STR = level % 2 == 0 ? 2 : 0;
                        stats.DEF = level % 2 == 0 ? 0 : 1;
                        break;
                    }
                case "Armour":
                    {
                        stats.DEF = 5;
                        stats.HP = level % 2 == 0 ? 1 : 0;
                        stats.STA = level % 2 == 0 ? 2 : 1;
                        stats.STR = level % 2 == 0 ? 1 : 0;
                        break;
                    }
                case "Amulet":
                    {
                        stats.HP = level % 2 == 0 ? 3 : 2;
                        stats.DEF = level % 2 == 0 ? 5 : 3;
                        stats.DMG = level % 2 == 0 ? 2 : 1;
                        stats.SPD = level % 2 == 0 ? 5 : 4;
                        stats.STA = level % 2 == 0 ? 5 : 1;
                        stats.STR = level % 2 == 0 ? 3 : 0;
                        stats.CRC = level % 2 == 0 ? 1 : 0;
                        stats.CRD = level % 2 == 0 ? 2 : 1;
                        break;
                    }
            }

            return stats;
        }

        public static string GetMaxXpAsString(BaseItem item)
        {
            //Return null if item is null
            if (item == null) return null;

            //Get correct xpTable
            Dictionary<int, (int, Stats)> xpTable = GetCorrectLevelDictionary(item);

            //If item is max level, return "MAX"
            if (item.Level == xpTable.Keys.LastOrDefault())
                return "MAX";

            //Return required xp to level up based on weapon level
            return Convert.ToString(xpTable[item.Level + 1].Item1 * ItemData.RarityMultipliers[item.Rarity.ToUpper()]);
        }

        /// <summary>
        /// Returns the correct xp table based on item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Dictionary<int, (int, Stats)> GetCorrectLevelDictionary(BaseItem item)
        {
            if (item is Weapon)
                return WeaponXPTable;
            else if (item is Armour)
                return ArmourXPTable;
            else if (item is Amulet)
                return AmuletXPTable;
            else
                return null;
        }

        #region Add XP

        /// <summary>
        /// Adds xp to a character and levels it up if needed.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="xp"></param>
        public static void AddXP(Character character, int xp)
        {
            if (character.Level == CharacterXPTable.Keys.LastOrDefault())
                return;

            character.Stats.XP += xp;

            while (character.Level < CharacterXPTable.Keys.LastOrDefault() && character.Stats.XP >= CharacterXPTable[character.Level + 1].Item1)
            {
                character.LevelUp();
            }
        }

        /// <summary>
        /// Adds xp to an item and levels it up if needed.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="xp"></param>
        public static void AddXP(BaseItem item, int xp)
        {
            //[V] Get correct xpTable
            Dictionary<int, (int, Stats)> xpTable = GetCorrectLevelDictionary(item);

            //[V] Get stats object of item (to access its stats)
            IStatsHolder statObj = item as IStatsHolder;

            //[!] Cancel if item is max level
            if (item.Level == xpTable.Keys.LastOrDefault())
                return;

            //[+] Add xp
            statObj.Stats.XP += xp;

            //[>>] Upgrade item
            while (item.Level < xpTable.Keys.LastOrDefault() && 
                statObj.Stats.XP >= Convert.ToInt16(xpTable[item.Level + 1].Item1 * ItemData.RarityMultipliers[item.Rarity.ToUpper()]))
            {
                item.LevelUp(item);
            }
        }

        #endregion
    }
}
