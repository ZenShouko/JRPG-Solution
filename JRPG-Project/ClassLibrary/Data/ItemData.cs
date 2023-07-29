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

        public static void InitializeLists()
        {
            //#Read Collectables json file
            string json = File.ReadAllText(@"../../Resources/Data/Collectables.json");
            ListCollectables = JsonConvert.DeserializeObject<List<Collectable>>(json);

            //Generate image for each collectable
            foreach (Collectable collectable in ListCollectables)
            {
                collectable.ItemImage = GetItemImage("Collectables/" + collectable.ImageName);
            }

            //#Read Weapon json file
            json = File.ReadAllText(@"../../Resources/Data/Weapons.json");
            ListWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);

            //Generate image for each weapon
            foreach (Weapon weapon in ListWeapons)
            {
                weapon.ItemImage = GetItemImage("Weapons/" + weapon.ImageName);
            }

            //#Read Armour json file
            json = File.ReadAllText(@"../../Resources/Data/Armours.json");
            ListArmours = JsonConvert.DeserializeObject<List<Armour>>(json);

            //Generate image for each armour
            foreach (Armour armour in ListArmours)
            {
                armour.ItemImage = GetItemImage("Armours/" + armour.ImageName);
            }

            //TODO #Read Amulet json file
            json = File.ReadAllText(@"../../Resources/Data/Amulets.json");
            ListAmulets = JsonConvert.DeserializeObject<List<Amulet>>(json);

            //Generate image for each amulet
            foreach (Amulet amulet in ListAmulets)
            {
                amulet.ItemImage = GetItemImage("Amulets/" + amulet.ImageName);
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
    }
}
