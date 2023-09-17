using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class CharacterData
    {
        public static List<Character> CharacterList = new List<Character>();

        public static void InitializeList()
        {
            //Read json file
            string json = File.ReadAllText(@"../../Resources/Data/Characters.json");
            CharacterList = JsonConvert.DeserializeObject<List<Character>>(json);

            //Generate image for each character AND equip their default weapon
            foreach (Character character in CharacterList)
                character.CharImage = GetCharacterImage(character.ImageName);
        }

        public static Image GetCharacterImage(string imageName)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Characters/" + imageName, UriKind.Relative));
            return img;
        }

        public static Image GetCharacterImage(Character c)
        {
            Image img = new Image();
            if (c.ID.Contains("F")) //Foe
                img.Source = FoeData.FoeList.FirstOrDefault(x => x.ID == c.ID).CharImage.Source;
            else
                img.Source = CharacterData.CharacterList.FirstOrDefault(x => x.ID == c.ID).CharImage.Source;

            return img;
        }

        public static void UnequipItem(BaseItem item)
        {
            foreach (Character member in Inventory.Team)
            {
                if (member.Weapon != null && member.Weapon.UniqueID == item.UniqueID)
                    member.Weapon = null;
                else if (member.Armour != null && member.Armour.UniqueID == item.UniqueID)
                    member.Armour = null;
                else if (member.Amulet != null && member.Amulet.UniqueID == item.UniqueID)
                    member.Amulet = null;
            }
        }

        public static void EquipItem(BaseItem item, int charIndex)
        {
            //Unequip item to avoid duplicates
            UnequipItem(item);

            if (item is Weapon wpn)
                Inventory.Team[charIndex].Weapon = wpn;
            else if (item is Armour arm)
                Inventory.Team[charIndex].Armour = arm;
            else if (item is Amulet amu)
                Inventory.Team[charIndex].Amulet = amu;
            else
                throw new Exception("Item type not recognized.");
        }

        public static void ClearEquipment(int charIndex, string type)
        {
            //Remove equipment
            if (type.ToUpper() == "WEAPON")
                Inventory.Team[charIndex].Weapon = null;
            else if (type.ToUpper() == "ARMOUR")
                Inventory.Team[charIndex].Armour = null;
            else if (type.ToUpper() == "AMULET")
                Inventory.Team[charIndex].Amulet = null;
        }

        public static int GetValue(Character c)
        {
            //Incorporate stats
            double statValue = (c.Stats.HP * 1.2) + (c.Stats.DEF * 1.5) + (c.Stats.DMG * 1.3) + (c.Stats.SPD * 1.1) + (c.Stats.STA * 1.3) +
                (c.Stats.STR * 1.4) + (c.Stats.CRC * 2) + (c.Stats.CRD * 1.8);
            statValue -= Convert.ToDouble(GetThreatScore(c)) * 0.9;

            statValue = Math.Ceiling(statValue / 2.5);

            return (int)statValue;
        }

        public static int GetThreatScore(Character c)
        {
            Stats stats = c.GetAccumulatedStats();

            // Calculate effective damage based on stamina and stamina scaling
            double staminaRatio = stats.DMG <= 0 ? 0 : stats.STA / stats.DMG;
            double effectiveDamage = stats.DMG;

            if (staminaRatio < 0.25)
            {
                effectiveDamage *= 0.8;
            }
            else if (staminaRatio < 0.5)
            {
                effectiveDamage *= 0.45;
            }
            else
            {
                effectiveDamage *= 0.25;
            }

            // Apply stamina damage scaling
            if (stats.STA < 0.15)
            {
                effectiveDamage *= 0.5;
            }
            else if (stats.STA > 0.6)
            {
                effectiveDamage *= 1; // No effect
            }
            else
            {
                effectiveDamage *= 0.85;
            }

            // Apply defense regeneration
            double defenseRegen = stats.STR * 0.5;

            // Calculate crit damage
            double critDamage = 1.0; // Default, no crit
            if (stats.CRC > 0)
            {
                critDamage = 1.0 + (stats.CRD / 100.0); // Convert CRD to a multiplier
            }

            // Calculate threat score
            double threatScore = effectiveDamage * critDamage + stats.DEF + defenseRegen;

            // Round threat score
            int roundedThreatScore = (int)Math.Round(threatScore);

            return roundedThreatScore;
        }

        /// <summary>
        /// If parameter is null, returns the threat score of the player team.
        /// </summary>
        /// <param name="mf"></param>
        /// <returns></returns>
        public static int GetTeamThreatScore(MapFoe mf)
        {
            if (mf is null)
            {
                //Get threat score of player team
                int threatScore = 0;
                foreach (Character c in Inventory.Team)
                    threatScore += GetThreatScore(c);

                return threatScore;
            }
            else
            {
                //Get threat score of foe team
                int threatScore = 0;
                foreach (Character c in mf.FoeTeam)
                    threatScore += GetThreatScore(c);

                return threatScore;
            }
        }
    }
}