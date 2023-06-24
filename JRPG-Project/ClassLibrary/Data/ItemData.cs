using JRPG_Project.ClassLibrary.Entities;
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

        public static void InitializeTables()
        {
            //Read Collectables json file
            string json = File.ReadAllText(@"../../Resources/Data/Collectables.json");
            ListCollectables = JsonConvert.DeserializeObject<List<Collectable>>(json);

            //Generate image for each collectable
            foreach (Collectable collectable in ListCollectables)
            {
                collectable.ItemImage = GetItemImage(collectable.ImageName);
            }
        }

        private static Image GetItemImage(string imageName)
        {
            Image itemImage = new Image();
            itemImage.Source = new BitmapImage(new Uri($@"../../Resources/Images/Items/{imageName}.png", UriKind.Relative));
            return itemImage;
        }
    }
}
