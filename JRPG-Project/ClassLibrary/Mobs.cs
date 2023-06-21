using JRPG_ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_ClassLibrary
{
    public static class Mobs
    {
        private static List<string> availableMobs = new List<string>()
        {
            "PLAYER", "ENEMY", "ITEM"
        };

        internal static Mob CreateMob(string request)
        {
            //Trim and convert to upper case
            request = request.Trim().ToUpper();

            //Check if request is available
            if (!availableMobs.Contains(request))
            {
                return null;
            }

            Mob mob = new Mob();
            mob.Name = request;


            //Create mob Icon
            Image mobImage = new Image();
            mobImage.BeginInit();

            mobImage.Height = 50;
            mobImage.Width = 50;
            mobImage.Stretch = System.Windows.Media.Stretch.Fill;

            //Switch request
            switch (request)
            {
                case "PLAYER":
                    {
                        if (!File.Exists(@"../../Resources/Assets/Characters/player.png"))
                        {
                            MessageBox.Show("ImageFileNotFound!");
                        }
                        mobImage.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Characters/player.png", UriKind.Relative));
                        break;
                    }
                case "ITEM":
                    {
                        if (!File.Exists(@"../../Resources/Assets/Items/item-box.png"))
                        {
                            MessageBox.Show("ImageFileNotFound!");
                        }
                        mobImage.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Items/item-box.png", UriKind.Relative));
                        break;
                    }
            }

            mobImage.EndInit();
            mob.Icon = mobImage;

            return mob;
        }
    }
}
