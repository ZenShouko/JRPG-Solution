using System;
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
    public class MapItem : MapObject
    {
        public string ReferenceId { get; set; } //Reference to the item/weapon in the database

        public MapItem()
        {
            //Default source for collectable items
            Icon.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/item-box.png", UriKind.Relative));
        }
    }
}
