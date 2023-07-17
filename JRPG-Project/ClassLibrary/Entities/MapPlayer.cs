using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Controls;
using JRPG_Project.ClassLibrary.Entities;
using System.Windows.Media.Imaging;

namespace JRPG_ClassLibrary.Entities
{
    /// <summary>
    /// Represents the player on the map.
    /// </summary>
    public class MapPlayer : MapObject
    {
        public MapPlayer()
        {
            //Will initialize its own icon
            Icon = new Image();

            Icon.BeginInit();
            Icon.Height = 50;
            Icon.Width = 50;
            Icon.Stretch = System.Windows.Media.Stretch.Fill;
            Icon.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/player.png", UriKind.Relative));
            Icon.EndInit();
        }
    }
}
