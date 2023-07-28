using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class LevelData
    {
        public static Dictionary<int, (int, Stats)> CharacterXPTable = new Dictionary<int, (int, Stats)>();
        public static Dictionary<int, (int, Stats)> WeaponXPTable = new Dictionary<int, (int, Stats)>();
        public static Dictionary<int, (int, Stats)> ArmourXPTable = new Dictionary<int, (int, Stats)>();
        public static Dictionary<int, (int, Stats)> AmuletXPTable = new Dictionary<int, (int, Stats)>();
        public static void InitializeDictionaries()
        {
            //Characters
            for (int i = 2; i <= 10; i++)
            {
                int xp = (int)(Math.Pow(i + 1, 3) * 1.3);
                CharacterXPTable.Add(i, (xp, CalculateStatIncrease(i)));
            }

            //Weapons
            for (int i = 2; i <= 10; i++)
            {
                int xp = (int)((Math.Pow(i + 1, 3) + (20 * (i - 2)) * 1.2));
                WeaponXPTable.Add(i, (xp, CalculateStatIncrease(i)));
            }

            //Armours
            for (int i = 2; i <= 10; i++)
            {
                int xp = (int)((Math.Pow(i + 1, 3) + 50 * 1.1)) / 2;
                ArmourXPTable.Add(i, (xp, CalculateStatIncrease(i)));
            }

            //Amulets
            for (int i = 2; i <= 10; i++)
            {
                int xp = (int)((Math.Pow(i + 1, 3) * (i * 0.5)));
                AmuletXPTable.Add(i, (xp, CalculateStatIncrease(i)));
            }
        }

        private static Stats CalculateStatIncrease(int level)
        {
            Stats stats = new Stats();
            stats.HP = (int)(level * 1.25) + (int)(level * 1.5);
            stats.DEF = (int)(level * 1.25) + (int)(level * 1.2);
            stats.DMG = (int)(level * 1.25) + (int)(level * 1.3);
            stats.SPD = (int)(level * 1.25) + (int)(level * 1.25);
            stats.CRC = (int)(level * 1.25) + (int)(level * 1.15); 
            stats.CRD = (int)(level * 1.25) + (int)(level * 1.2); stats.CRD = Convert.ToInt32(stats.CRD * 1.5);
            stats.STA = (int)(level * 1.25) + (int)(level * 1.35);
            stats.STR = (int)(level * 1.25) + (int)(level * 1.2);

            return stats;
        }

        private static void LevelUp(Character character)
        {

        }

        private static void LevelUp(Weapon weapon)
        {
            //Cancel if weapon is max level
            if (weapon.Level == WeaponXPTable.Keys.LastOrDefault())
            {
                //Reset xp to max xp
                weapon.Stats.XP = WeaponXPTable[weapon.Level + 1].Item1;
                return;
            }

            //Substract required xp from current xp
            weapon.Stats.XP -= WeaponXPTable[weapon.Level + 1].Item1;

            //Increase stats
            Stats levelUpStats = WeaponXPTable[weapon.Level + 1].Item2;
            weapon.Stats.HP += levelUpStats.HP;
            weapon.Stats.DEF += levelUpStats.DEF;
            weapon.Stats.DMG += levelUpStats.DMG;
            weapon.Stats.SPD += levelUpStats.SPD;
            weapon.Stats.CRC += levelUpStats.CRC;
            weapon.Stats.CRD += levelUpStats.CRD;
            weapon.Stats.STA += levelUpStats.STA;
            weapon.Stats.STR += levelUpStats.STR;

            //Increase level
            weapon.Level++;

            //Reset xp to max xp if weapon is max level
            if (weapon.Level == WeaponXPTable.Keys.LastOrDefault())
            {
                weapon.Stats.XP = WeaponXPTable[weapon.Level].Item1;
            }
        }

        public static void AddXP(Character character, int xp)
        {

        }

        public static void AddXP(Weapon weapon, int xp)
        {
            //Cancel if weapon is max level
            if (weapon.Level == WeaponXPTable.Keys.LastOrDefault()) 
                return;

            //Add xp
            weapon.Stats.XP += xp;

            //Check if weapon can level up
            while (weapon.Level < 10 && weapon.Stats.XP > WeaponXPTable[weapon.Level + 1].Item1)
            {
                LevelUp(weapon);
            }
        }

        public static string GetMaxXpAsString(object item)
        {
            //get null & return null
            if (item == null) return null;

            //What is object?
            if (item is Weapon wpn)
            {
                //Return required xp to level up based on weapon level
                if (WeaponXPTable.TryGetValue(wpn.Level + 1, out var xpTuple))
                {
                    return Convert.ToString(xpTuple.Item1);
                }
                else
                {
                    return "MAX";
                }
            }
            else if (item is Armour arm)
            {
                //Return required xp to level up based on weapon level
                if (ArmourXPTable.TryGetValue(arm.Level + 1, out var xpTuple))
                {
                    return Convert.ToString(xpTuple.Item1);
                }
                else
                {
                    return "MAX";
                }
            }
            else if (item is Amulet amu)
            {
                //Return required xp to level up based on weapon level
                if (AmuletXPTable.TryGetValue(amu.Level + 1, out var xpTuple))
                {
                    return Convert.ToString(xpTuple.Item1);
                }
                else
                {
                    return "MAX";
                }
            }
            else if (item is Character cha)
            {
                throw new NotImplementedException();   
            }

            return "error-404";
        }
    }
}
