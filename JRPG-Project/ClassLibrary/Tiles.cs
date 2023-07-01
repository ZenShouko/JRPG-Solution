using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Data;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using System.Windows.Media.Animation;
using JRPG_ClassLibrary;

namespace JRPG_Project.ClassLibrary
{
    public static class Tiles
    {
        public static Border CreateTile(string code)
        {
            //Create tile
            Border tile = new Border();
            tile.BorderBrush = Brushes.Black;
            tile.BorderThickness = new Thickness(1);
            tile.CornerRadius = new CornerRadius(2);
            tile.Background = GetTileColor(code);

            return tile;
        }

        private static Brush GetTileColor(string code)
        {
            //DataRow[] rows = TileData.TileTable.Select($"Code = '{code}'");

            //if (rows.Count() == 0) { return null; }

            //return (Brush)rows[0]["TileColor"];
            return null;
        }

        public static Tile GetTile(int x, int y)
        {
            return Stages.CurrentStage.TileList.Find(t => t.Position.X == x && t.Position.Y == y);
        }
    }
}
