using JRPG_Project.ClassLibrary.Entities;
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

        public static List<Material> ListMaterials = new List<Material>();

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
                collectable.ItemImage = GetItemImage(collectable);
            }

            //#Read Weapon json file
            json = File.ReadAllText(@"../../Resources/Data/Weapons.json");
            ListWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);

            //Generate image for each weapon & calculate value
            foreach (Weapon weapon in ListWeapons)
            {
                weapon.ItemImage = GetItemImage(weapon);
                SetValue(weapon);
            }

            //#Read Armour json file
            json = File.ReadAllText(@"../../Resources/Data/Armours.json");
            ListArmours = JsonConvert.DeserializeObject<List<Armour>>(json);

            //Generate image for each armour & calculate value
            foreach (Armour armour in ListArmours)
            {
                armour.ItemImage = GetItemImage(armour);
                SetValue(armour);
            }

            //#Read Amulet json file
            json = File.ReadAllText(@"../../Resources/Data/Amulets.json");
            ListAmulets = JsonConvert.DeserializeObject<List<Amulet>>(json);

            //Generate image for each amulet & calculate value
            foreach (Amulet amulet in ListAmulets)
            {
                amulet.ItemImage = GetItemImage(amulet);
                SetValue(amulet);
            }

            //#Read Upgrade Materials json file
            json = File.ReadAllText(@"../../Resources/Data/Materials.json");
            ListMaterials = JsonConvert.DeserializeObject<List<Material>>(json);

            //Generate image for each material
            foreach (Material material in ListMaterials)
            {
                material.ItemImage = GetItemImage(material);
            }
        }

        /// <summary>
        /// Important: Always include the folder name in the imageName!
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static Image GetItemImage(string imageName)
        {
            /// Replaced ---
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
            else if (item is Material mat)
            {
                itemImage.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Collectables/" + mat.ImageName, UriKind.Relative));
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

        public static void SetValue(BaseItem item)
        {
            //Set value
            item.Value = GetValue(item);
        }

        public static int GetValue(BaseItem item)
        {
            /// <summary>
            /// Calculate item value based on item type and item stats. Adds 10*level to value as well.
            /// 

            IStatsHolder obj = item as IStatsHolder;
            ItemValueFormula multipliers = ListValueMultipliers.Find(x => x.ItemType == item.GetType().Name.ToUpper());

            //Calculate value based on stats
            double value = (obj.Stats.DMG * multipliers.DmgMultiplier) + (obj.Stats.DEF * multipliers.DefMultiplier)
                + (obj.Stats.HP * multipliers.HpMultiplier) + (obj.Stats.SPD * multipliers.SpdMultiplier) +
                (obj.Stats.STA * multipliers.StaMultiplier) + (obj.Stats.STR * multipliers.StrMultiplier) +
                (obj.Stats.CRC * multipliers.CrcMultiplier) + (obj.Stats.CRD * multipliers.CrdMultiplier);

            //Incorporate rarity multiplier
            value *= RarityMultipliers[item.Rarity];

            //Add 10*level to value
            value += (10 * item.Level);

            //Round value to nearest 5
            value = Math.Round(value / 5) * 5;

            //Set value
            return Convert.ToInt32(value);
        }
    }
}
