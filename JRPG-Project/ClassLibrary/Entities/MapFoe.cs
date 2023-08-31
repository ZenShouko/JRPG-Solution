using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary.Entities
{
    /// <summary>
    /// The object that is used on the platform map.
    /// </summary>
    public class MapFoe : MapObject
    {
        public string IconNames { get; set; } //Might remove
        public bool HasDetectedPlayer { get; set; }
        public string MovementBehaviour { get; set; }
        public List<Character> FoeTeam { get; set; } = new List<Character>();

        public MapFoe()
        {
            //Default icon for foes
            Icon = new Image();
            Icon.BeginInit();
            Icon.Height = 50;
            Icon.Width = 50;
            Icon.Stretch = System.Windows.Media.Stretch.Uniform;
            Icon.Source = HasDetectedPlayer ? new BitmapImage(new Uri(@"../../Resources/Assets/Platform/foe-alert.png", UriKind.Relative)) :
                new BitmapImage(new Uri(@"../../Resources/Assets/Platform/foe-neutral.png", UriKind.Relative));
            Icon.EndInit();
        }
    }
}
