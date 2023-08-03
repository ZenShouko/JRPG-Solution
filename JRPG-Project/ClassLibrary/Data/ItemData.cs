﻿using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class ItemData
    {
        //Strings are always set in UPPER CASE
        public static List<Collectable> ListCollectables = new List<Collectable>();
        public static List<Weapon> ListWeapons = new List<Weapon>();
        public static List<Armour> ListArmours = new List<Armour>();
        public static List<Amulet> ListAmulets = new List<Amulet>();
        public static List<ItemValueFormula> ListValueMultipliers = new List<ItemValueFormula>();
        public static Dictionary<string, double> RarityMultipliers = new Dictionary<string, double>()
        {
            { "COMMON", 1.1 },
            { "SPECIAL", 1.5 },
            { "CURSED", 2 },
            { "LEGENDARY", 2.5 }
        };

        public static void InitializeLists()
        {
            //#Read value multipliers json file
            string json = File.ReadAllText(@"../../Resources/Data/ItemValueMultipliers.json");
            ListValueMultipliers = JsonConvert.DeserializeObject<List<ItemValueFormula>>(json);

            //#Read Collectables json file
            json = File.ReadAllText(@"../../Resources/Data/Collectables.json");
            ListCollectables = JsonConvert.DeserializeObject<List<Collectable>>(json);

            //Generate image for each collectable
            foreach (Collectable collectable in ListCollectables)
            {
                collectable.ItemImage = GetItemImage("Collectables/" + collectable.ImageName);
            }

            //#Read Weapon json file
            json = File.ReadAllText(@"../../Resources/Data/Weapons.json");
            ListWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);

            //Generate image for each weapon & calculate value
            foreach (Weapon weapon in ListWeapons)
            {
                weapon.ItemImage = GetItemImage("Weapons/" + weapon.ImageName);
                CalculateValue(weapon);
            }

            //#Read Armour json file
            json = File.ReadAllText(@"../../Resources/Data/Armours.json");
            ListArmours = JsonConvert.DeserializeObject<List<Armour>>(json);

            //Generate image for each armour & calculate value
            foreach (Armour armour in ListArmours)
            {
                armour.ItemImage = GetItemImage("Armours/" + armour.ImageName);
                CalculateValue(armour);
            }

            //#Read Amulet json file
            json = File.ReadAllText(@"../../Resources/Data/Amulets.json");
            ListAmulets = JsonConvert.DeserializeObject<List<Amulet>>(json);

            //Generate image for each amulet & calculate value
            foreach (Amulet amulet in ListAmulets)
            {
                amulet.ItemImage = GetItemImage("Amulets/" + amulet.ImageName);
                CalculateValue(amulet);
            }
        }

        /// <summary>
        /// Important: Always include the folder name in the imageName!
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static Image GetItemImage(string imageName)
        {
            Image itemImage = new Image();
            itemImage.Source = new BitmapImage(new Uri(@"../../Resources/Assets/" + imageName, UriKind.Relative));
            return itemImage;
        }

        /// <summary>
        /// Can retrieve image for any item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Image GetItemImage(BaseItem item)
        {
            Image itemImage = new Image();

            if (item is Weapon wpn)
            {
                itemImage.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Weapons/" + wpn.ImageName, UriKind.Relative));
            }
            else if (item is Armour arm)
            {
                itemImage.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Armours/" + arm.ImageName, UriKind.Relative));
            }
            else if (item is Amulet amu)
            {
                itemImage.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Amulets/" + amu.ImageName, UriKind.Relative));
            }
            else if (item is Collectable col)
            {
                itemImage.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Collectables/" + col.ImageName, UriKind.Relative));
            }

            return itemImage;
        }

        /// <summary>
        /// Returns name of owner. If no one has this item equipped, returns "/"
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetOwnersName(BaseItem item)
        {
            if (item.UniqueID.Contains("weapon"))
            {
                Weapon wpn = item as Weapon;

                foreach (Character character in Inventory.Team)
                {
                    if (character.Weapon != null && character.Weapon.UniqueID == wpn.UniqueID)
                    {
                        return character.Name;
                    }
                }
            }
            else if (item.UniqueID.Contains("armour"))
            {
                Armour arm = item as Armour;

                foreach (Character character in Inventory.Team)
                {
                    if (character.Armour != null && character.Armour.UniqueID == arm.UniqueID)
                    {
                        return character.Name;
                    }
                }
            }
            else if (item.UniqueID.Contains("amulet"))
            {
                Amulet amu = item as Amulet;

                foreach (Character character in Inventory.Team)
                {
                    if (character.Amulet != null && character.Amulet.UniqueID == amu.UniqueID)
                    {
                        return character.Name;
                    }
                }
            }

            //If no one has this item equipped, return "No one"
            return "/";
        }

        public static string GetItemType(BaseItem item)
        {
            if (item.UniqueID.Contains("weapon") || item.ID.Contains("W"))
            {
                return "Weapon";
            }
            else if (item.UniqueID.Contains("armour") || item.ID.Contains("A"))
            {
                return "Armour";
            }
            else if (item.UniqueID.Contains("amulet") || item.ID.Contains("AM"))
            {
                return "Amulet";
            }
            else if (item.UniqueID.Contains("collectable") || item.ID.Contains("C"))
            {
                return "Collectable";
            }
            else
            {
                return "Not Recognized";
            }
        }


        /// <summary>
        /// Gets item based on UNIQUE ID
        /// </summary>
        public static BaseItem GetItemByUniqueID(string uniqueID)
        {
            if (uniqueID.Contains("weapon"))
            {
                return Inventory.Weapons.Find(x => x.UniqueID == uniqueID);
            }
            else if (uniqueID.Contains("armour"))
            {
                return Inventory.Armours.Find(x => x.UniqueID == uniqueID);
            }
            else if (uniqueID.Contains("amulet"))
            {
                return Inventory.Amulets.Find(x => x.UniqueID == uniqueID);
            }
            else { return null; }
        }

        public static void CalculateValue(BaseItem item)
        {
            /// <summary>
            /// Calculate item value based on item type and item stats. Adds 10*level to value as well.
            /// 

            //Get item value multiplier based on item type
            if (item is Weapon wpn)
            {
                ItemValueFormula multipliers = ListValueMultipliers.Find(x => x.ItemType == "WEAPON");

                //Calculate value based on stats
                double value = (wpn.Stats.DMG * multipliers.DmgMultiplier) + (wpn.Stats.DEF * multipliers.DefMultiplier)
                    + (wpn.Stats.HP * multipliers.HpMultiplier) + (wpn.Stats.SPD * multipliers.SpdMultiplier) +
                    (wpn.Stats.STA * multipliers.StaMultiplier) + (wpn.Stats.STR * multipliers.StrMultiplier) +
                    (wpn.Stats.CRC * multipliers.CrcMultiplier) + (wpn.Stats.CRD * multipliers.CrdMultiplier);

                //Incorporate rarity multiplier
                value *= RarityMultipliers[wpn.Rarity];

                //Add 10*level to value
                value += (10 * wpn.Level);

                //Round value to nearest 5
                value = Math.Round(value / 5) * 5;

                //Set value
                wpn.Value = Convert.ToInt32(value);
            }
            else if (item is Armour arm)
            {
                ItemValueFormula multipliers = ListValueMultipliers.Find(x => x.ItemType == "ARMOUR");

                //Calculate value based on stats
                double value = (arm.Stats.DMG * multipliers.DmgMultiplier) + (arm.Stats.DEF * multipliers.DefMultiplier)
                    + (arm.Stats.HP * multipliers.HpMultiplier) + (arm.Stats.SPD * multipliers.SpdMultiplier) +
                    (arm.Stats.STA * multipliers.StaMultiplier) + (arm.Stats.STR * multipliers.StrMultiplier) +
                    (arm.Stats.CRC * multipliers.CrcMultiplier) + (arm.Stats.CRD * multipliers.CrdMultiplier);

                //Incorporate rarity multiplier
                value *= RarityMultipliers[arm.Rarity];

                //Add 10*level to value
                value += (10 * arm.Level);

                //Round value to nearest 5
                value = Math.Round(value / 5) * 5;

                //Set value
                arm.Value = Convert.ToInt32(value);
            }
            else if (item is Amulet amu)
            {
                ItemValueFormula multipliers = ListValueMultipliers.Find(x => x.ItemType == "ARMOUR");

                //Calculate value based on stats
                double value = (amu.Stats.DMG * multipliers.DmgMultiplier) + (amu.Stats.DEF * multipliers.DefMultiplier)
                    + (amu.Stats.HP * multipliers.HpMultiplier) + (amu.Stats.SPD * multipliers.SpdMultiplier) +
                    (amu.Stats.STA * multipliers.StaMultiplier) + (amu.Stats.STR * multipliers.StrMultiplier) +
                    (amu.Stats.CRC * multipliers.CrcMultiplier) + (amu.Stats.CRD * multipliers.CrdMultiplier);

                //Incorporate rarity multiplier
                value *= RarityMultipliers[amu.Rarity];

                //Add 10*level to value
                value += (10 * amu.Level);

                //Round value to nearest 5
                value = Math.Round(value / 5) * 5;

                //Set value
                amu.Value = Convert.ToInt32(value);
            }
        }
    }
}
