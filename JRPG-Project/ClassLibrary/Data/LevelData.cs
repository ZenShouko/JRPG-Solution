using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;

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
            stats.CRD = (int)(level * 1.25) + (int)(level * 1.2);
            stats.STA = (int)(level * 1.25) + (int)(level * 1.35);
            stats.STR = (int)(level * 1.25) + (int)(level * 1.2);

            return stats;
        }
        private static void LevelUp(Character character)
        {

        }

        public static void AddXP(Character character, int xp)
        {

        }
    }
}
