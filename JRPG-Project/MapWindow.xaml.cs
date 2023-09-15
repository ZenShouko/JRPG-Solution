using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JRPG_Project
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow()
        {
            InitializeComponent();
            LoadStage();
        }

        private void LoadStage()
        {
            //Set grid column and row count
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();
            for (int i = 0; i < Stages.CurrentStage.TileList.Max(t => t.Position.X) + 1; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < Stages.CurrentStage.TileList.Max(t => t.Position.Y) + 1; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            //Add tiles
            foreach (var tile in Stages.CurrentStage.TileList)
            {
                //Tile element
                Border border = new Border();
                border.BorderBrush = tile.TileElement.BorderBrush;
                border.BorderThickness = tile.TileElement.BorderThickness;
                border.Background = tile.TileElement.Background;
                Grid.SetColumn(border, tile.Position.X);
                Grid.SetRow(border, tile.Position.Y);
                MainGrid.Children.Add(border);

                //Tile content
                //- Lootbox?
                if (tile.TypeLootbox != null)
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/lootbox.png", UriKind.RelativeOrAbsolute));
                    Grid.SetColumn(img, tile.Position.X);
                    Grid.SetRow(img, tile.Position.Y);
                    MainGrid.Children.Add(img);
                }

                //- Foe?
                if (tile.Foe != null)
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/foe-neutral.png", UriKind.RelativeOrAbsolute));
                    Grid.SetColumn(img, tile.Position.X);
                    Grid.SetRow(img, tile.Position.Y);
                    MainGrid.Children.Add(img);
                }

                //- Player?
                if (tile.Player != null)
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/player.png", UriKind.RelativeOrAbsolute));
                    Grid.SetColumn(img, tile.Position.X);
                    Grid.SetRow(img, tile.Position.Y);
                    MainGrid.Children.Add(img);
                }
            }
        }
    }
}
