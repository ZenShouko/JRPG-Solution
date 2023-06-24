using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class ItemData
    {
        public static List<Collectable> ListCollectables = new List<Collectable>();
        public static List<Weapon> ListWeapons = new List<Weapon>();
        public static List<Armour> ListArmours = new List<Armour>();
        public static List<Amulet> ListAmulets = new List<Amulet>();

        public static void InitializeTables()
        {
            //#Read Collectables json file
            string json = File.ReadAllText(@"../../Resources/Data/Collectables.json");
            ListCollectables = JsonConvert.DeserializeObject<List<Collectable>>(json);

            //Generate image for each collectable
            foreach (Collectable collectable in ListCollectables)
            {
                collectable.ItemImage = GetItemImage(collectable.ImageName);
            }

            //#Read Weapon json file
            json = File.ReadAllText(@"../../Resources/Data/Weapons.json");
            ListWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);

            //Generate image for each weapon
            foreach (Weapon weapon in ListWeapons)
            {
                weapon.ItemImage = GetItemImage(weapon.ImageName);
            }

            //#Read Armour json file
            json = File.ReadAllText(@"../../Resources/Data/Armours.json");
            ListArmours = JsonConvert.DeserializeObject<List<Armour>>(json);

            //Generate image for each armour
            foreach (Armour armour in ListArmours)
            {
                armour.ItemImage = GetItemImage(armour.ImageName);
            }

            //TODO #Read Amulet json file

        }

        private static Image GetItemImage(string imageName)
        {
            Image itemImage = new Image();
            itemImage.Source = new BitmapImage(new Uri($@"../../Resources/Images/Items/{imageName}.png", UriKind.Relative));
            return itemImage;
        }
    }
}
