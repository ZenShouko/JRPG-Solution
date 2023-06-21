using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace JRPG_Project.ClassLibrary
{
    public static class Tiles
    {
        private static List<string> tileList = new List<string>()
        {
            "DEF", "SEA"
        };

        public static Border CreateTile(string type)
        {
            //Check if type is available
            if (!tileList.Contains(type))
            {
                return null;
            }

            //Create tile
            Border tile = new Border();
            tile.BorderBrush = Brushes.Black;
            tile.BorderThickness = new Thickness(1);
            tile.CornerRadius = new CornerRadius(2);

            //Switch type
            switch (type)
            {
                case "DEF":
                    {
                        tile.Background = Brushes.Beige;
                        break;
                    }
                case "SEA":
                    {
                        tile.Background = Brushes.DarkSlateBlue;
                        break;
                    }
            }

            return tile;
        }
    }
}
