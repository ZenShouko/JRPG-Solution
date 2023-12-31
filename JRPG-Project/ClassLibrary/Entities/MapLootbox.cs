﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary.Entities
{
    /// <summary>
    /// Class that contains all the items that are on the map
    /// </summary>
    public class MapLootbox : MapObject
    {
        public string Type { get; set; }

        public MapLootbox()
        {
            Icon = new Image();

            //Default size for collectable items
            Icon.BeginInit();
            Icon.Height = 50;
            Icon.Width = 50;
            Icon.Stretch = System.Windows.Media.Stretch.Uniform;
            Icon.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/lootbox.png", UriKind.Relative));
            Icon.EndInit();
        }
    }
}
